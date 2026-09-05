import { ChangeDetectionStrategy, Component, computed, inject, signal } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { Observable } from 'rxjs';

import { greskePoPolju, porukaGreske } from '../../../core/greske/poruke-gresaka';
import { KategorijaMenija, Odrediste } from '../../../core/modeli/api.modeli';
import { AdminMeniServis } from '../../../core/servisi/admin-meni.servis';
import { MeniServis } from '../../../core/servisi/meni.servis';
import { ObavestenjaServis } from '../../../core/servisi/obavestenja.servis';
import { PotvrdaServis } from '../../../core/servisi/potvrda.servis';

@Component({
  selector: 'og-admin-kategorije',
  imports: [ReactiveFormsModule],
  changeDetection: ChangeDetectionStrategy.OnPush,
  templateUrl: './kategorije.html',
  styleUrl: './kategorije.scss',
})
export class AdminKategorije {
  private readonly fb = inject(FormBuilder);
  private readonly potvrda = inject(PotvrdaServis);
  private readonly citanje = inject(MeniServis);
  private readonly servis = inject(AdminMeniServis);
  private readonly obavestenja = inject(ObavestenjaServis);

  protected readonly kategorije = signal<KategorijaMenija[]>([]);
  protected readonly ucitava = signal(true);
  protected readonly greska = signal<string | null>(null);

  protected readonly menjam = signal<number | null>(null);
  protected readonly cuva = signal(false);
  protected readonly brisem = signal<number | null>(null);
  protected readonly greskaForme = signal<string | null>(null);

  protected readonly odredista: Odrediste[] = ['Kuhinja', 'Sank'];

  protected readonly naslovForme = computed(() =>
    this.menjam() ? 'Izmena kategorije' : 'Nova kategorija',
  );

  protected readonly forma = this.fb.nonNullable.group({
    naziv: ['', [Validators.required, Validators.maxLength(100)]],
    opis: ['', Validators.maxLength(500)],
    redosled: [0, [Validators.required, Validators.min(0)]],
    odrediste: ['Kuhinja' as Odrediste, Validators.required],
  });

  constructor() {
    this.ucitaj();
  }

  protected ucitaj(): void {
    this.ucitava.set(true);
    this.greska.set(null);

    this.citanje.kategorije().subscribe({
      next: (k) => {
        this.kategorije.set([...k].sort((a, b) => a.redosled - b.redosled));
        this.ucitava.set(false);
      },
      error: (g) => {
        this.greska.set(porukaGreske(g, 'Kategorije nisu učitane.'));
        this.ucitava.set(false);
      },
    });
  }

  protected nova(): void {
    const sledeci = this.kategorije().reduce((max, k) => Math.max(max, k.redosled), 0) + 1;

    this.forma.reset({ naziv: '', opis: '', redosled: sledeci, odrediste: 'Kuhinja' });
    this.greskaForme.set(null);
    this.menjam.set(0);
  }

  protected izmeni(k: KategorijaMenija): void {
    this.forma.reset({
      naziv: k.naziv,
      opis: k.opis ?? '',
      redosled: k.redosled,
      odrediste: k.odrediste,
    });
    this.greskaForme.set(null);
    this.menjam.set(k.id);
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
      opis: v.opis.trim() || null,
      redosled: v.redosled,
      odrediste: v.odrediste,
    };

    const id = this.menjam();
    this.cuva.set(true);

    const poziv: Observable<unknown> = id
      ? this.servis.izmeniKategoriju(id, ulaz)
      : this.servis.kreirajKategoriju(ulaz);

    poziv.subscribe({
      next: () => {
        this.cuva.set(false);
        this.menjam.set(null);
        this.obavestenja.uspeh(id ? 'Kategorija je izmenjena.' : 'Kategorija je dodata.');
        this.ucitaj();
      },
      error: (g) => {
        this.cuva.set(false);
        this.greskaForme.set(porukaGreske(g, 'Kategorija nije sačuvana.'));
        this.prikaziGreskePolja(g);
      },
    });
  }

  protected async obrisi(k: KategorijaMenija): Promise<void> {
    const potvrdjeno = await this.potvrda.pitaj({
      naslov: 'Brisanje kategorije',
      tekst: `Kategorija „${k.naziv}" trajno se uklanja iz menija.`,
      potvrdi: 'Obriši',
    });
    if (!potvrdjeno) return;

    this.brisem.set(k.id);

    this.servis.obrisiKategoriju(k.id).subscribe({
      next: () => {
        this.brisem.set(null);
        this.obavestenja.uspeh('Kategorija je obrisana.');
        this.ucitaj();
      },
      error: (g) => {
        this.brisem.set(null);
        this.obavestenja.greska(g, 'Kategorija nije obrisana.');
      },
    });
  }

  private prikaziGreskePolja(g: unknown): void {
    for (const [polje, poruke] of Object.entries(greskePoPolju(g))) {
      if (!polje || !poruke?.length) continue;
      const naziv = polje[0].toLowerCase() + polje.slice(1);
      const kontrola = this.forma.get(naziv);
      kontrola?.setErrors({ server: poruke[0] });
      kontrola?.markAsTouched();
    }
  }
}
