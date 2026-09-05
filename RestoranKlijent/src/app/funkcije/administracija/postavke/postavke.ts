import { ChangeDetectionStrategy, Component, inject, signal } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';

import { greskePoPolju, porukaGreske } from '../../../core/greske/poruke-gresaka';
import { AdminSadrzajServis } from '../../../core/servisi/admin-sadrzaj.servis';
import { ObavestenjaServis } from '../../../core/servisi/obavestenja.servis';

@Component({
  selector: 'og-admin-postavke',
  imports: [ReactiveFormsModule],
  changeDetection: ChangeDetectionStrategy.OnPush,
  templateUrl: './postavke.html',
  styleUrl: './postavke.scss',
})
export class AdminPostavke {
  private readonly fb = inject(FormBuilder);
  private readonly servis = inject(AdminSadrzajServis);
  private readonly obavestenja = inject(ObavestenjaServis);

  protected readonly ucitava = signal(true);
  protected readonly greska = signal<string | null>(null);
  protected readonly cuva = signal(false);
  protected readonly greskaForme = signal<string | null>(null);

  protected readonly forma = this.fb.nonNullable.group({
    adresa: ['', [Validators.required, Validators.maxLength(200)]],
    telefon: ['', [Validators.required, Validators.maxLength(50)]],
    email: ['', [Validators.required, Validators.email]],
    radnoVreme: ['', [Validators.required, Validators.maxLength(200)]],
    opisRestorana: ['', Validators.maxLength(2000)],
    geoSirina: [null as number | null, [Validators.min(-90), Validators.max(90)]],
    geoDuzina: [null as number | null, [Validators.min(-180), Validators.max(180)]],
    facebookUrl: [''],
    instagramUrl: [''],
  });

  constructor() {
    this.ucitaj();
  }

  protected ucitaj(): void {
    this.ucitava.set(true);
    this.greska.set(null);

    this.servis.postavke().subscribe({
      next: (p) => {
        this.forma.reset({
          adresa: p.adresa,
          telefon: p.telefon,
          email: p.email,
          radnoVreme: p.radnoVreme,
          opisRestorana: p.opisRestorana,
          geoSirina: p.geoSirina,
          geoDuzina: p.geoDuzina,
          facebookUrl: p.facebookUrl ?? '',
          instagramUrl: p.instagramUrl ?? '',
        });
        this.ucitava.set(false);
      },
      error: (g) => {
        this.greska.set(porukaGreske(g, 'Postavke nisu učitane.'));
        this.ucitava.set(false);
      },
    });
  }

  protected sacuvaj(): void {
    this.greskaForme.set(null);

    if (this.forma.invalid) {
      this.forma.markAllAsTouched();
      return;
    }

    const v = this.forma.getRawValue();
    this.cuva.set(true);

    this.servis
      .izmeniPostavke({
        adresa: v.adresa.trim(),
        telefon: v.telefon.trim(),
        email: v.email.trim(),
        radnoVreme: v.radnoVreme.trim(),
        opisRestorana: v.opisRestorana.trim(),
        geoSirina: v.geoSirina ?? null,
        geoDuzina: v.geoDuzina ?? null,
        facebookUrl: v.facebookUrl.trim() || null,
        instagramUrl: v.instagramUrl.trim() || null,
      })
      .subscribe({
        next: () => {
          this.cuva.set(false);
          this.obavestenja.uspeh('Postavke su sačuvane — vidljive su odmah na sajtu.');
        },
        error: (g) => {
          this.cuva.set(false);
          this.greskaForme.set(porukaGreske(g, 'Postavke nisu sačuvane.'));

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
}
