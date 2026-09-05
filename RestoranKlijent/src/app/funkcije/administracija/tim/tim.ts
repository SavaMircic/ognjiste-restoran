import { ChangeDetectionStrategy, Component, computed, inject, signal } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';

import { RezervnaSlika } from '../../../core/direktive/rezervna-slika.direktiva';
import { greskePoPolju, porukaGreske } from '../../../core/greske/poruke-gresaka';
import { Zaposleni } from '../../../core/modeli/api.modeli';
import { SlikaCev } from '../../../core/pipes/slika.pipe';
import { AdminKorisniciServis } from '../../../core/servisi/admin-korisnici.servis';
import { AdminSadrzajServis } from '../../../core/servisi/admin-sadrzaj.servis';
import { ObavestenjaServis } from '../../../core/servisi/obavestenja.servis';
import { nazivUloge } from '../../../core/tekst/uloge';

const MAKS_BAJTOVA = 2 * 1024 * 1024;
const EKSTENZIJE = ['.jpg', '.jpeg', '.png', '.webp'];

@Component({
  selector: 'og-admin-tim',
  imports: [ReactiveFormsModule, SlikaCev, RezervnaSlika],
  changeDetection: ChangeDetectionStrategy.OnPush,
  templateUrl: './tim.html',
  styleUrl: './tim.scss',
})
export class AdminTim {
  private readonly fb = inject(FormBuilder);
  private readonly zaposleniServis = inject(AdminKorisniciServis);
  private readonly servis = inject(AdminSadrzajServis);
  private readonly obavestenja = inject(ObavestenjaServis);

  protected readonly zaposleni = signal<Zaposleni[]>([]);
  protected readonly ucitava = signal(true);
  protected readonly greska = signal<string | null>(null);

  protected readonly menjam = signal<number | null>(null);
  protected readonly cuva = signal(false);
  protected readonly saljeSliku = signal<number | null>(null);
  protected readonly greskaForme = signal<string | null>(null);

  protected readonly nazivUloge = nazivUloge;

  protected readonly naSajtu = computed(() => this.zaposleni().filter((z) => z.prikaziNaSajtu).length);

  protected readonly forma = this.fb.nonNullable.group({
    biografija: ['', Validators.maxLength(1000)],
    linkedInUrl: [''],
    instagramUrl: [''],
    redosled: [0, [Validators.required, Validators.min(0), Validators.max(999)]],
    prikaziNaSajtu: [false],
  });

  constructor() {
    this.ucitaj();
  }

  protected ucitaj(): void {
    this.ucitava.set(true);
    this.greska.set(null);

    this.zaposleniServis.pretraziZaposlene({ aktivan: true, velicinaStrane: 100 }).subscribe({
      next: (odgovor) => {
        this.zaposleni.set(
          [...odgovor.podaci].sort(
            (a, b) => a.redosled - b.redosled || a.prezime.localeCompare(b.prezime, 'sr'),
          ),
        );
        this.ucitava.set(false);
      },
      error: (g) => {
        this.greska.set(porukaGreske(g, 'Tim nije učitan.'));
        this.ucitava.set(false);
      },
    });
  }

  protected izmeni(z: Zaposleni): void {
    this.forma.reset({
      biografija: z.biografija ?? '',
      linkedInUrl: z.linkedInUrl ?? '',
      instagramUrl: z.instagramUrl ?? '',
      redosled: z.redosled,
      prikaziNaSajtu: z.prikaziNaSajtu,
    });
    this.greskaForme.set(null);
    this.menjam.set(z.id);
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
      .izmeniProfilNaSajtu(id, {
        biografija: v.biografija.trim() || null,
        linkedInUrl: v.linkedInUrl.trim() || null,
        instagramUrl: v.instagramUrl.trim() || null,
        redosled: Number(v.redosled),
        prikaziNaSajtu: v.prikaziNaSajtu,
      })
      .subscribe({
        next: () => {
          this.cuva.set(false);
          this.menjam.set(null);
          this.obavestenja.uspeh('Javni profil je sačuvan.');
          this.ucitaj();
        },
        error: (g) => {
          this.cuva.set(false);
          this.greskaForme.set(porukaGreske(g, 'Profil nije sačuvan.'));

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

  protected prebaciPrikaz(z: Zaposleni): void {
    this.cuva.set(true);

    this.servis
      .izmeniProfilNaSajtu(z.id, {
        biografija: z.biografija,
        linkedInUrl: z.linkedInUrl,
        instagramUrl: z.instagramUrl,
        redosled: z.redosled,
        prikaziNaSajtu: !z.prikaziNaSajtu,
      })
      .subscribe({
        next: () => {
          this.cuva.set(false);
          this.zaposleni.update((sve) =>
            sve.map((x) => (x.id === z.id ? { ...x, prikaziNaSajtu: !z.prikaziNaSajtu } : x)),
          );
        },
        error: (g) => {
          this.cuva.set(false);
          this.obavestenja.greska(g, 'Vidljivost nije promenjena.');
        },
      });
  }

  protected postaviSliku(z: Zaposleni, dogadjaj: Event): void {
    const ulaz = dogadjaj.target as HTMLInputElement;
    const datoteka = this.uzmiDatoteku(ulaz);
    if (!datoteka) return;

    this.saljeSliku.set(z.id);

    this.servis.postaviSlikuZaposlenog(z.id, datoteka).subscribe({
      next: ({ slikaUrl }) => {
        this.saljeSliku.set(null);
        this.zaposleni.update((sve) => sve.map((x) => (x.id === z.id ? { ...x, slikaUrl } : x)));
        this.obavestenja.uspeh('Fotografija je postavljena.');
      },
      error: (g) => {
        this.saljeSliku.set(null);
        this.obavestenja.greska(g, 'Fotografija nije postavljena.');
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
