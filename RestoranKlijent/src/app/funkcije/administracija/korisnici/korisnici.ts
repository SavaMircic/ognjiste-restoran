import { ChangeDetectionStrategy, Component, computed, inject, signal } from '@angular/core';
import { takeUntilDestroyed, toObservable } from '@angular/core/rxjs-interop';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { RouterLink } from '@angular/router';
import { Observable, debounceTime, distinctUntilChanged, skip } from 'rxjs';

import { greskePoPolju, porukaGreske } from '../../../core/greske/poruke-gresaka';
import { KorisnikAdmin } from '../../../core/modeli/api.modeli';
import { DatumCev } from '../../../core/pipes/datum.pipe';
import { AdminKorisniciServis } from '../../../core/servisi/admin-korisnici.servis';
import { ObavestenjaServis } from '../../../core/servisi/obavestenja.servis';
import { PotvrdaServis } from '../../../core/servisi/potvrda.servis';
import { izServera, uUtcIso } from '../../../core/tekst/vreme';
import { Paginacija } from '../../zajednicko/paginacija/paginacija';

const PO_STRANI = 20;

interface OtvorenaAkcija {
  id: string;
  ime: string;
  tip: 'blokada' | 'zabrana';
}

@Component({
  selector: 'og-admin-korisnici',
  imports: [ReactiveFormsModule, RouterLink, DatumCev, Paginacija],
  changeDetection: ChangeDetectionStrategy.OnPush,
  templateUrl: './korisnici.html',
  styleUrl: './korisnici.scss',
})
export class AdminKorisnici {
  private readonly fb = inject(FormBuilder);
  private readonly potvrda = inject(PotvrdaServis);
  private readonly servis = inject(AdminKorisniciServis);
  private readonly obavestenja = inject(ObavestenjaServis);

  protected readonly pretraga = signal('');
  protected readonly blokiran = signal<boolean | null>(null);
  protected readonly strana = signal(1);

  protected readonly korisnici = signal<KorisnikAdmin[]>([]);
  protected readonly ukupnoStrana = signal(1);
  protected readonly ukupnoZapisa = signal(0);
  protected readonly ucitava = signal(true);
  protected readonly greska = signal<string | null>(null);

  protected readonly akcija = signal<OtvorenaAkcija | null>(null);
  protected readonly salje = signal(false);
  protected readonly greskaForme = signal<string | null>(null);
  protected readonly uToku = signal<string | null>(null);

  protected readonly prazno = computed(
    () => !this.ucitava() && !this.greska() && this.korisnici().length === 0,
  );

  protected readonly formaBlokade = this.fb.nonNullable.group({
    razlog: ['', [Validators.required, Validators.maxLength(500)]],
    datumDo: [''],
  });

  protected readonly formaZabrane = this.fb.nonNullable.group({
    brojDana: [7, [Validators.required, Validators.min(1), Validators.max(365)]],
    razlog: ['', [Validators.required, Validators.maxLength(500)]],
  });

  constructor() {
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

    this.servis
      .pretrazi({
        pretraga: this.pretraga().trim() || null,
        blokiran: this.blokiran(),
        strana: this.strana(),
        velicinaStrane: PO_STRANI,
      })
      .subscribe({
        next: (odgovor) => {
          this.korisnici.set(odgovor.podaci);
          this.ukupnoStrana.set(Math.max(1, odgovor.ukupnoStrana));
          this.ukupnoZapisa.set(odgovor.ukupnoZapisa);
          this.ucitava.set(false);
        },
        error: (g) => {
          this.greska.set(porukaGreske(g, 'Korisnici nisu učitani.'));
          this.ucitava.set(false);
        },
      });
  }

  protected filtrirajStatus(vrednost: string): void {
    this.blokiran.set(vrednost === '' ? null : vrednost === 'true');
    this.strana.set(1);
    this.ucitaj();
  }

  protected naStranu(strana: number): void {
    this.strana.set(strana);
    this.ucitaj();
  }

  protected otvoriBlokadu(k: KorisnikAdmin): void {
    this.formaBlokade.reset({ razlog: '', datumDo: '' });
    this.greskaForme.set(null);
    this.akcija.set({ id: k.id, ime: `${k.ime} ${k.prezime}`, tip: 'blokada' });
  }

  protected otvoriZabranu(k: KorisnikAdmin): void {
    this.formaZabrane.reset({ brojDana: 7, razlog: '' });
    this.greskaForme.set(null);
    this.akcija.set({ id: k.id, ime: `${k.ime} ${k.prezime}`, tip: 'zabrana' });
  }

  protected zatvoriAkciju(): void {
    this.akcija.set(null);
    this.greskaForme.set(null);
  }

  protected posalji(): void {
    const a = this.akcija();
    if (!a) return;

    this.greskaForme.set(null);
    const forma: FormGroup = a.tip === 'blokada' ? this.formaBlokade : this.formaZabrane;

    if (forma.invalid) {
      forma.markAllAsTouched();
      return;
    }

    this.salje.set(true);

    let poziv: Observable<unknown>;
    if (a.tip === 'blokada') {
      const v = this.formaBlokade.getRawValue();
      poziv = this.servis.blokiraj(a.id, {
        razlog: v.razlog.trim(),
        datumDo: v.datumDo ? uUtcIso(v.datumDo) : null,
      });
    } else {
      const v = this.formaZabrane.getRawValue();
      poziv = this.servis.zabraniKomentarisanje(a.id, {
        brojDana: Number(v.brojDana),
        razlog: v.razlog.trim(),
      });
    }

    poziv.subscribe({
      next: () => {
        this.salje.set(false);
        this.akcija.set(null);
        this.obavestenja.uspeh(
          a.tip === 'blokada' ? 'Nalog je blokiran.' : 'Komentarisanje je zabranjeno.',
        );
        this.ucitaj();
      },
      error: (g) => {
        this.salje.set(false);
        this.greskaForme.set(porukaGreske(g, 'Mera nije primenjena.'));

        for (const [polje, poruke] of Object.entries(greskePoPolju(g))) {
          if (!polje || !poruke?.length) continue;
          const naziv = polje[0].toLowerCase() + polje.slice(1);
          const kontrola = forma.get(naziv);
          kontrola?.setErrors({ server: poruke[0] });
          kontrola?.markAsTouched();
        }
      },
    });
  }

  protected async odblokiraj(k: KorisnikAdmin): Promise<void> {
    const potvrdjeno = await this.potvrda.pitaj({
      naslov: 'Ukidanje blokade',
      tekst: `${k.ime} ${k.prezime} će ponovo moći da se prijavi.`,
      potvrdi: 'Ukini blokadu',
      opasno: false,
    });
    if (!potvrdjeno) return;

    this.uToku.set(k.id);

    this.servis.odblokiraj(k.id).subscribe({
      next: () => {
        this.uToku.set(null);
        this.obavestenja.uspeh('Blokada je ukinuta.');
        this.ucitaj();
      },
      error: (g) => {
        this.uToku.set(null);
        this.obavestenja.greska(g, 'Blokada nije ukinuta.');
      },
    });
  }

  protected async ukiniZabranu(k: KorisnikAdmin): Promise<void> {
    const potvrdjeno = await this.potvrda.pitaj({
      naslov: 'Ukidanje zabrane komentarisanja',
      tekst: `${k.ime} ${k.prezime} će ponovo moći da piše recenzije.`,
      potvrdi: 'Ukini zabranu',
      opasno: false,
    });
    if (!potvrdjeno) return;

    this.uToku.set(k.id);

    this.servis.ukiniZabranuKomentarisanja(k.id).subscribe({
      next: () => {
        this.uToku.set(null);
        this.obavestenja.uspeh('Zabrana komentarisanja je ukinuta.');
        this.ucitaj();
      },
      error: (g) => {
        this.uToku.set(null);
        this.obavestenja.greska(g, 'Zabrana nije ukinuta.');
      },
    });
  }

  protected zabranaVazi(k: KorisnikAdmin): boolean {
    const doKada = izServera(k.zabranaKomentarisanjaDo);
    return !!doKada && doKada.getTime() > Date.now();
  }
}
