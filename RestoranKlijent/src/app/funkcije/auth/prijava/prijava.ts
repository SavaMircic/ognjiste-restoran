import { ChangeDetectionStrategy, Component, computed, inject, input, signal } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';

import { jeAuthStrana } from '../../../core/guards/gost.guard';
import { pocetnaZaUlogu } from '../../../core/navigacija';
import { greskePoPolju, porukaGreske } from '../../../core/greske/poruke-gresaka';
import { AuthServis } from '../../../core/servisi/auth.servis';
import { ObavestenjaServis } from '../../../core/servisi/obavestenja.servis';
import { AuthOkvir } from '../okvir/auth-okvir';

@Component({
  selector: 'og-prijava',
  imports: [ReactiveFormsModule, RouterLink, AuthOkvir],
  changeDetection: ChangeDetectionStrategy.OnPush,
  templateUrl: './prijava.html',
  styleUrl: './prijava.scss',
})
export class Prijava {
  private readonly fb = inject(FormBuilder);
  private readonly auth = inject(AuthServis);
  private readonly router = inject(Router);
  private readonly obavestenja = inject(ObavestenjaServis);

  readonly povratak = input<string>();

  protected readonly salje = signal(false);
  protected readonly greska = signal<string | null>(null);

  protected readonly nijePotvrdjen = computed(() => (this.greska() ?? '').includes('nije potvrđena'));

  protected readonly forma = this.fb.nonNullable.group({
    email: ['', [Validators.required, Validators.email]],
    lozinka: ['', Validators.required],
  });

  private pocetna(): string {
    return this.auth.radniRezim() ? pocetnaZaUlogu(this.auth.uloge()) : '/';
  }

  private odrediste(): string {
    const cilj = this.povratak() ?? '';

    const unutarSajta = cilj.startsWith('/') && !cilj.startsWith('//');
    if (!unutarSajta) return this.pocetna();

    return jeAuthStrana(cilj) ? this.pocetna() : cilj;
  }

  protected posalji(): void {
    this.greska.set(null);

    if (this.forma.invalid) {
      this.forma.markAllAsTouched();
      return;
    }

    this.salje.set(true);
    const { email, lozinka } = this.forma.getRawValue();

    this.auth.prijava({ email: email.trim(), lozinka }).subscribe({
      next: (odgovor) => {
        this.salje.set(false);

        void this.router.navigateByUrl(this.odrediste()).then((uspelo) => {
          this.obavestenja.uspeh(`Dobrodošli, ${odgovor.ime}.`);

          if (!uspelo) void this.router.navigateByUrl(this.pocetna());
        });
      },
      error: (g) => {
        this.salje.set(false);
        this.greska.set(porukaGreske(g, 'Prijava nije uspela.'));

        for (const [polje, poruke] of Object.entries(greskePoPolju(g))) {
          const naziv = polje ? polje[0].toLowerCase() + polje.slice(1) : '';
          const kontrola = this.forma.get(naziv);
          if (kontrola && poruke?.length) {
            kontrola.setErrors({ server: poruke[0] });
            kontrola.markAsTouched();
          }
        }
      },
    });
  }
}
