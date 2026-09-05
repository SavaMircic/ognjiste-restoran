import { ChangeDetectionStrategy, Component, computed, inject, signal } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';

import { RezervnaSlika } from '../../../core/direktive/rezervna-slika.direktiva';
import { greskePoPolju, porukaGreske } from '../../../core/greske/poruke-gresaka';
import { GalerijaSlika } from '../../../core/modeli/api.modeli';
import { SlikaCev } from '../../../core/pipes/slika.pipe';
import { AdminSadrzajServis } from '../../../core/servisi/admin-sadrzaj.servis';
import { ObavestenjaServis } from '../../../core/servisi/obavestenja.servis';
import { PotvrdaServis } from '../../../core/servisi/potvrda.servis';

const MAKS_BAJTOVA = 2 * 1024 * 1024;
const EKSTENZIJE = ['.jpg', '.jpeg', '.png', '.webp'];

@Component({
  selector: 'og-admin-galerija',
  imports: [ReactiveFormsModule, SlikaCev, RezervnaSlika],
  changeDetection: ChangeDetectionStrategy.OnPush,
  templateUrl: './galerija.html',
  styleUrl: './galerija.scss',
})
export class AdminGalerija {
  private readonly fb = inject(FormBuilder);
  private readonly potvrda = inject(PotvrdaServis);
  private readonly servis = inject(AdminSadrzajServis);
  private readonly obavestenja = inject(ObavestenjaServis);

  protected readonly slike = signal<GalerijaSlika[]>([]);
  protected readonly ucitava = signal(true);
  protected readonly greska = signal<string | null>(null);

  protected readonly salje = signal(false);
  protected readonly menjam = signal<number | null>(null);
  protected readonly cuva = signal(false);
  protected readonly brisem = signal<number | null>(null);
  protected readonly greskaForme = signal<string | null>(null);

  protected readonly aktivnih = computed(() => this.slike().filter((s) => s.aktivan).length);

  protected readonly oblasti = computed(() =>
    [...new Set(this.slike().map((s) => s.grupa).filter(Boolean))].sort((a, b) =>
      a.localeCompare(b, 'sr'),
    ),
  );

  protected readonly formaNove = this.fb.nonNullable.group({
    naslov: ['', [Validators.required, Validators.maxLength(150)]],
    grupa: ['', [Validators.required, Validators.maxLength(100)]],
    opis: ['', Validators.maxLength(500)],
  });

  protected readonly forma = this.fb.nonNullable.group({
    naslov: ['', [Validators.required, Validators.maxLength(150)]],
    grupa: ['', [Validators.required, Validators.maxLength(100)]],
    opis: ['', Validators.maxLength(500)],
    redosled: [0, [Validators.required, Validators.min(0), Validators.max(999)]],
    aktivan: [true],
  });

  constructor() {
    this.ucitaj();
  }

  protected ucitaj(): void {
    this.ucitava.set(true);
    this.greska.set(null);

    this.servis.galerija().subscribe({
      next: (s) => {
        this.slike.set([...s].sort((a, b) => a.redosled - b.redosled));
        this.ucitava.set(false);
      },
      error: (g) => {
        this.greska.set(porukaGreske(g, 'Galerija nije učitana.'));
        this.ucitava.set(false);
      },
    });
  }

  protected dodaj(dogadjaj: Event): void {
    const ulaz = dogadjaj.target as HTMLInputElement;
    const datoteka = this.uzmiDatoteku(ulaz);
    if (!datoteka) return;

    if (this.formaNove.invalid) {
      this.formaNove.markAllAsTouched();
      this.obavestenja.info('Naslov je obavezan pre otpremanja.');
      return;
    }

    const v = this.formaNove.getRawValue();
    this.salje.set(true);

    this.servis.dodajUGaleriju(datoteka, v.naslov.trim(), v.grupa.trim(), v.opis.trim() || null).subscribe({
      next: (slika) => {
        this.salje.set(false);
        this.formaNove.reset({ naslov: '', grupa: v.grupa, opis: '' });
        this.slike.update((sve) => [...sve, slika].sort((a, b) => a.redosled - b.redosled));
        this.obavestenja.uspeh('Slika je dodata u galeriju.');
      },
      error: (g) => {
        this.salje.set(false);
        this.obavestenja.greska(g, 'Slika nije dodata.');
      },
    });
  }

  protected izmeni(s: GalerijaSlika): void {
    this.forma.reset({
      naslov: s.naslov,
      grupa: s.grupa,
      opis: s.opis ?? '',
      redosled: s.redosled,
      aktivan: s.aktivan,
    });
    this.greskaForme.set(null);
    this.menjam.set(s.id);
  }

  protected odustani(): void {
    this.menjam.set(null);
    this.greskaForme.set(null);
  }

  protected sacuvaj(): void {
    const id = this.menjam();
    if (!id) return;

    this.greskaForme.set(null);

    if (this.forma.invalid) {
      this.forma.markAllAsTouched();
      return;
    }

    const v = this.forma.getRawValue();
    this.cuva.set(true);

    this.servis
      .izmeniSlikuGalerije(id, {
        naslov: v.naslov.trim(),
        grupa: v.grupa.trim(),
        opis: v.opis.trim() || null,
        redosled: Number(v.redosled),
        aktivan: v.aktivan,
      })
      .subscribe({
        next: () => {
          this.cuva.set(false);
          this.menjam.set(null);
          this.obavestenja.uspeh('Slika je izmenjena.');
          this.ucitaj();
        },
        error: (g) => {
          this.cuva.set(false);
          this.greskaForme.set(porukaGreske(g, 'Izmena nije sačuvana.'));

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

  protected prebaciAktivnost(s: GalerijaSlika): void {
    this.cuva.set(true);

    this.servis
      .izmeniSlikuGalerije(s.id, {
        naslov: s.naslov,
        grupa: s.grupa,
        opis: s.opis,
        redosled: s.redosled,
        aktivan: !s.aktivan,
      })
      .subscribe({
        next: () => {
          this.cuva.set(false);
          this.slike.update((sve) =>
            sve.map((x) => (x.id === s.id ? { ...x, aktivan: !s.aktivan } : x)),
          );
        },
        error: (g) => {
          this.cuva.set(false);
          this.obavestenja.greska(g, 'Vidljivost nije promenjena.');
        },
      });
  }

  protected async obrisi(s: GalerijaSlika): Promise<void> {
    const potvrdjeno = await this.potvrda.pitaj({
      naslov: 'Brisanje slike',
      tekst: `Slika „${s.naslov}" trajno se uklanja iz galerije.`,
      potvrdi: 'Obriši',
    });
    if (!potvrdjeno) return;

    this.brisem.set(s.id);

    this.servis.obrisiSlikuGalerije(s.id).subscribe({
      next: () => {
        this.brisem.set(null);
        this.slike.update((sve) => sve.filter((x) => x.id !== s.id));
        this.obavestenja.uspeh('Slika je obrisana.');
      },
      error: (g) => {
        this.brisem.set(null);
        this.obavestenja.greska(g, 'Slika nije obrisana.');
      },
    });
  }

  private uzmiDatoteku(ulaz: HTMLInputElement): File | null {
    const datoteka = ulaz.files?.[0] ?? null;
    ulaz.value = '';

    if (!datoteka) return null;

    const tacka = datoteka.name.lastIndexOf('.');
    const ekstenzija = tacka === -1 ? '' : datoteka.name.slice(tacka).toLowerCase();

    if (!EKSTENZIJE.includes(ekstenzija)) {
      this.obavestenja.greska(null, `Dozvoljeni formati: ${EKSTENZIJE.join(', ')}.`);
      return null;
    }

    if (datoteka.size > MAKS_BAJTOVA) {
      this.obavestenja.greska(null, 'Slika ne može biti veća od 2 MB.');
      return null;
    }

    return datoteka;
  }
}
