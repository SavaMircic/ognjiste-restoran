import { DecimalPipe } from '@angular/common';
import { ChangeDetectionStrategy, Component, computed, inject, signal } from '@angular/core';
import { takeUntilDestroyed, toObservable } from '@angular/core/rxjs-interop';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { RouterLink } from '@angular/router';
import { Observable, catchError, debounceTime, distinctUntilChanged, of, skip } from 'rxjs';

import { RezervnaSlika } from '../../../core/direktive/rezervna-slika.direktiva';
import { greskePoPolju, porukaGreske } from '../../../core/greske/poruke-gresaka';
import { KategorijaMenija, StavkaMenija } from '../../../core/modeli/api.modeli';
import { SlikaCev } from '../../../core/pipes/slika.pipe';
import { AdminMeniServis } from '../../../core/servisi/admin-meni.servis';
import { MeniServis } from '../../../core/servisi/meni.servis';
import { ObavestenjaServis } from '../../../core/servisi/obavestenja.servis';
import { PotvrdaServis } from '../../../core/servisi/potvrda.servis';
import { Paginacija } from '../../zajednicko/paginacija/paginacija';

const PO_STRANI = 20;

@Component({
  selector: 'og-admin-meni',
  imports: [ReactiveFormsModule, RouterLink, DecimalPipe, SlikaCev, RezervnaSlika, Paginacija],
  changeDetection: ChangeDetectionStrategy.OnPush,
  templateUrl: './meni.html',
  styleUrl: './meni.scss',
})
export class AdminMeni {
  private readonly fb = inject(FormBuilder);
  private readonly potvrda = inject(PotvrdaServis);
  private readonly citanje = inject(MeniServis);
  private readonly servis = inject(AdminMeniServis);
  private readonly obavestenja = inject(ObavestenjaServis);

  protected readonly kategorijaId = signal<number | null>(null);
  protected readonly pretraga = signal('');
  protected readonly strana = signal(1);

  protected readonly kategorije = signal<KategorijaMenija[]>([]);
  protected readonly stavke = signal<StavkaMenija[]>([]);
  protected readonly ukupnoStrana = signal(1);
  protected readonly ukupnoZapisa = signal(0);
  protected readonly ucitava = signal(true);
  protected readonly greska = signal<string | null>(null);

  protected readonly menjam = signal<number | null>(null);
  protected readonly cuva = signal(false);
  protected readonly greskaForme = signal<string | null>(null);

  protected readonly menjaDostupnost = signal<number | null>(null);
  protected readonly brisem = signal<number | null>(null);

  protected readonly prazno = computed(
    () => !this.ucitava() && !this.greska() && this.stavke().length === 0,
  );

  protected readonly naslovForme = computed(() => (this.menjam() ? 'Izmena jela' : 'Novo jelo'));

  protected readonly ucitavaOpis = signal(false);

  protected readonly forma = this.fb.nonNullable.group({
    naziv: ['', [Validators.required, Validators.maxLength(150)]],
    opis: ['', [Validators.required, Validators.maxLength(2000)]],
    detaljanOpis: ['', Validators.maxLength(4000)],
    cena: [0, [Validators.required, Validators.min(0.01)]],
    kategorijaId: [0, [Validators.required, Validators.min(1)]],
    popust: [null as number | null, [Validators.min(0), Validators.max(100)]],
  });

  private readonly formaVrednosti = signal(this.forma.getRawValue());

  protected readonly cenaSaPopustom = computed(() => {
    const v = this.formaVrednosti();
    const cena = Number(v.cena) || 0;
    if (!v.popust) return cena;
    return Math.round(cena * (1 - v.popust / 100) * 100) / 100;
  });

  constructor() {
    this.citanje
      .kategorije()
      .pipe(catchError(() => of([] as KategorijaMenija[])))
      .subscribe((k) => this.kategorije.set([...k].sort((a, b) => a.redosled - b.redosled)));

    this.forma.valueChanges
      .pipe(takeUntilDestroyed())
      .subscribe(() => this.formaVrednosti.set(this.forma.getRawValue()));

    toObservable(this.pretraga)
      .pipe(skip(1), debounceTime(300), distinctUntilChanged(), takeUntilDestroyed())
      .subscribe(() => {
        this.strana.set(1);
        this.ucitaj();
      });

    this.ucitaj();
  }

  protected ucitaj(): void {
    this.ucitava.set(true);
    this.greska.set(null);

    this.citanje
      .stavke({
        kategorijaId: this.kategorijaId(),
        pretraga: this.pretraga().trim() || null,
        strana: this.strana(),
        velicinaStrane: PO_STRANI,
      })
      .subscribe({
        next: (odgovor) => {
          this.stavke.set(odgovor.podaci);
          this.ukupnoStrana.set(Math.max(1, odgovor.ukupnoStrana));
          this.ukupnoZapisa.set(odgovor.ukupnoZapisa);
          this.ucitava.set(false);
        },
        error: (g) => {
          this.greska.set(porukaGreske(g, 'Jela nisu učitana.'));
          this.ucitava.set(false);
        },
      });
  }

  protected izaberiKategoriju(vrednost: string): void {
    this.kategorijaId.set(vrednost ? Number(vrednost) : null);
    this.strana.set(1);
    this.ucitaj();
  }

  protected naStranu(strana: number): void {
    this.strana.set(strana);
    this.ucitaj();
  }

  protected novo(): void {
    const prva = this.kategorije()[0]?.id ?? 0;
    this.forma.reset({ naziv: '', opis: '', cena: 0, kategorijaId: prva, popust: null });
    this.greskaForme.set(null);
    this.menjam.set(0);
  }

  protected izmeni(s: StavkaMenija): void {
    this.forma.reset({
      naziv: s.naziv,
      opis: s.opis,
      detaljanOpis: '',
      cena: s.cena,
      kategorijaId: s.kategorijaId,
      popust: s.popust,
    });
    this.greskaForme.set(null);
    this.menjam.set(s.id);

    this.ucitavaOpis.set(true);
    this.citanje.stavka(s.id).subscribe({
      next: (detalj) => {
        this.ucitavaOpis.set(false);
        if (this.menjam() !== s.id) return;
        this.forma.controls.detaljanOpis.setValue(detalj.detaljanOpis ?? '');
      },
      error: () => {
        this.ucitavaOpis.set(false);
        if (this.menjam() !== s.id) return;

        this.odustani();
        this.obavestenja.greska(null, 'Jelo nije učitano za izmenu. Pokušajte ponovo.');
      },
    });
  }

  protected odustani(): void {
    this.menjam.set(null);
    this.greskaForme.set(null);
  }

  protected sacuvaj(): void {
    this.greskaForme.set(null);

    if (this.forma.invalid) {
      this.forma.markAllAsTouched();
      return;
    }

    const v = this.forma.getRawValue();
    const ulaz = {
      naziv: v.naziv.trim(),
      opis: v.opis.trim(),
      detaljanOpis: v.detaljanOpis.trim() || null,
      cena: Number(v.cena),
      kategorijaId: Number(v.kategorijaId),
      popust: v.popust ? Number(v.popust) : null,
    };

    const id = this.menjam();
    this.cuva.set(true);

    const poziv: Observable<unknown> = id
      ? this.servis.izmeniStavku(id, ulaz)
      : this.servis.kreirajStavku(ulaz);

    poziv.subscribe({
      next: () => {
        this.cuva.set(false);
        this.menjam.set(null);
        this.obavestenja.uspeh(
          id ? 'Jelo je izmenjeno.' : 'Jelo je dodato — sada mu postavite sliku.',
        );
        this.ucitaj();
      },
      error: (g) => {
        this.cuva.set(false);
        this.greskaForme.set(porukaGreske(g, 'Jelo nije sačuvano.'));

        for (const [polje, poruke] of Object.entries(greskePoPolju(g))) {
          if (!polje || !poruke?.length) continue;
          const naziv = polje[0].toLowerCase() + polje.slice(1);
          const kontrola = this.forma.get(naziv);
          kontrola?.setErrors({ server: poruke[0] });
          kontrola?.markAsTouched();
        }
      },
    });
  }

  protected prebaciDostupnost(s: StavkaMenija): void {
    this.menjaDostupnost.set(s.id);

    this.servis.promeniDostupnost(s.id, !s.dostupno).subscribe({
      next: () => {
        this.menjaDostupnost.set(null);
        this.stavke.update((sve) =>
          sve.map((x) => (x.id === s.id ? { ...x, dostupno: !s.dostupno } : x)),
        );
      },
      error: (g) => {
        this.menjaDostupnost.set(null);
        this.obavestenja.greska(g, 'Dostupnost nije promenjena.');
      },
    });
  }

  protected async obrisi(s: StavkaMenija): Promise<void> {
    const potvrdjeno = await this.potvrda.pitaj({
      naslov: 'Brisanje jela',
      tekst: `Jelo „${s.naziv}" trajno se uklanja iz menija.`,
      potvrdi: 'Obriši',
    });
    if (!potvrdjeno) return;

    this.brisem.set(s.id);

    this.servis.obrisiStavku(s.id).subscribe({
      next: () => {
        this.brisem.set(null);
        this.obavestenja.uspeh('Jelo je obrisano.');
        this.ucitaj();
      },
      error: (g) => {
        this.brisem.set(null);
        this.obavestenja.greska(g, 'Jelo nije obrisano.');
      },
    });
  }
}
