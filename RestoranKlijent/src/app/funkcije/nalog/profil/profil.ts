import { ChangeDetectionStrategy, Component, computed, inject, signal } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router } from '@angular/router';

import { SlikaCev } from '../../../core/pipes/slika.pipe';
import { DatumCev } from '../../../core/pipes/datum.pipe';
import { RezervnaSlika } from '../../../core/direktive/rezervna-slika.direktiva';
import { greskePoPolju, porukaGreske } from '../../../core/greske/poruke-gresaka';
import { KorisnikProfil } from '../../../core/modeli/api.modeli';
import { AuthServis } from '../../../core/servisi/auth.servis';
import { ObavestenjaServis } from '../../../core/servisi/obavestenja.servis';
import { ProfilServis } from '../../../core/servisi/profil.servis';
import { IME_OBRAZAC, TELEFON_OBRAZAC } from '../../../core/validacija/obrasci';

const MAKS_BAJTOVA = 2 * 1024 * 1024;
const DOZVOLJENE = ['.jpg', '.jpeg', '.png', '.webp'];

@Component({
  selector: 'og-profil',
  imports: [ReactiveFormsModule, SlikaCev, RezervnaSlika, DatumCev],
  changeDetection: ChangeDetectionStrategy.OnPush,
  templateUrl: './profil.html',
  styleUrl: './profil.scss',
})
export class Profil {
  private readonly fb = inject(FormBuilder);
  private readonly servis = inject(ProfilServis);
  private readonly obavestenja = inject(ObavestenjaServis);
  private readonly auth = inject(AuthServis);
  private readonly router = inject(Router);

  protected readonly profil = signal<KorisnikProfil | null>(null);
  protected readonly ucitava = signal(true);
  protected readonly greska = signal<string | null>(null);

  protected readonly cuvaProfil = signal(false);
  protected readonly cuvaLozinku = signal(false);
  protected readonly saljeSliku = signal(false);
  protected readonly greskaProfila = signal<string | null>(null);
  protected readonly greskaLozinke = signal<string | null>(null);
  protected readonly greskaSlike = signal<string | null>(null);

  protected readonly inicijali = computed(() => {
    const p = this.profil();
    return p ? `${p.ime[0] ?? ''}${p.prezime[0] ?? ''}`.toUpperCase() : '';
  });

  protected readonly formaProfila = this.fb.nonNullable.group({
    ime: ['', [Validators.required, Validators.minLength(2), Validators.maxLength(100), Validators.pattern(IME_OBRAZAC)]],
    prezime: [
      '',
      [Validators.required, Validators.minLength(2), Validators.maxLength(100), Validators.pattern(IME_OBRAZAC)],
    ],
    brojTelefona: ['', Validators.pattern(TELEFON_OBRAZAC)],
  });

  protected readonly formaLozinke = this.fb.nonNullable.group({
    staraLozinka: ['', Validators.required],
    novaLozinka: [
      '',
      [Validators.required, Validators.minLength(8), Validators.pattern(/^(?=.*[A-Z])(?=.*[0-9]).+$/)],
    ],
  });

  constructor() {
    this.ucitaj();
  }

  protected ucitaj(): void {
    this.ucitava.set(true);
    this.greska.set(null);

    this.servis.profil().subscribe({
      next: (p) => {
        this.profil.set(p);
        this.formaProfila.patchValue({
          ime: p.ime,
          prezime: p.prezime,
          brojTelefona: p.brojTelefona ?? '',
        });
        this.ucitava.set(false);
      },
      error: (g) => {
        this.greska.set(porukaGreske(g, 'Profil nije učitan.'));
        this.ucitava.set(false);
      },
    });
  }

  protected sacuvajProfil(): void {
    this.greskaProfila.set(null);

    if (this.formaProfila.invalid) {
      this.formaProfila.markAllAsTouched();
      return;
    }

    this.cuvaProfil.set(true);
    const v = this.formaProfila.getRawValue();

    this.servis
      .izmeniProfil({
        ime: v.ime.trim(),
        prezime: v.prezime.trim(),
        brojTelefona: v.brojTelefona.trim() || null,
      })
      .subscribe({
        next: () => {
          this.cuvaProfil.set(false);
          this.obavestenja.uspeh('Profil je sačuvan.');
          this.ucitaj();
        },
        error: (g) => {
          this.cuvaProfil.set(false);
          this.greskaProfila.set(porukaGreske(g, 'Izmene nisu sačuvane.'));
          this.upisiGreske(this.formaProfila, g);
        },
      });
  }

  protected promeniLozinku(): void {
    this.greskaLozinke.set(null);

    if (this.formaLozinke.invalid) {
      this.formaLozinke.markAllAsTouched();
      return;
    }

    this.cuvaLozinku.set(true);

    this.servis.promeniLozinku(this.formaLozinke.getRawValue()).subscribe({
      next: () => {
        this.cuvaLozinku.set(false);
        this.formaLozinke.reset();
        this.obavestenja.uspeh('Lozinka je promenjena.');
      },
      error: (g) => {
        this.cuvaLozinku.set(false);
        this.greskaLozinke.set(porukaGreske(g, 'Lozinka nije promenjena.'));
        this.upisiGreske(this.formaLozinke, g);
      },
    });
  }

  protected izaberiSliku(dogadjaj: Event): void {
    const ulaz = dogadjaj.target as HTMLInputElement;
    const datoteka = ulaz.files?.[0];
    if (!datoteka) return;

    this.greskaSlike.set(null);

    const ekstenzija = datoteka.name.slice(datoteka.name.lastIndexOf('.')).toLowerCase();
    if (!DOZVOLJENE.includes(ekstenzija)) {
      this.greskaSlike.set(`Dozvoljeni formati: ${DOZVOLJENE.join(', ')}.`);
      ulaz.value = '';
      return;
    }
    if (datoteka.size > MAKS_BAJTOVA) {
      this.greskaSlike.set('Slika ne sme biti veća od 2 MB.');
      ulaz.value = '';
      return;
    }

    this.saljeSliku.set(true);

    this.servis.postaviSliku(datoteka).subscribe({
      next: (o) => {
        this.saljeSliku.set(false);
        this.profil.update((p) => (p ? { ...p, slikaUrl: o.slikaUrl } : p));
        this.obavestenja.uspeh('Profilna slika je promenjena.');
        ulaz.value = '';
      },
      error: (g) => {
        this.saljeSliku.set(false);
        this.greskaSlike.set(porukaGreske(g, 'Slika nije otpremljena.'));
        ulaz.value = '';
      },
    });
  }

  protected odjavi(): void {
    this.auth.odjava();
    void this.router.navigateByUrl('/');
    this.obavestenja.uspeh('Odjavljeni ste.');
  }

  private upisiGreske(forma: { get(n: string): { setErrors(e: object): void; markAsTouched(): void } | null }, g: unknown): void {
    for (const [polje, poruke] of Object.entries(greskePoPolju(g))) {
      if (!polje || !poruke?.length) continue;
      const naziv = polje[0].toLowerCase() + polje.slice(1);
      const kontrola = forma.get(naziv);
      kontrola?.setErrors({ server: poruke[0] });
      kontrola?.markAsTouched();
    }
  }
}
