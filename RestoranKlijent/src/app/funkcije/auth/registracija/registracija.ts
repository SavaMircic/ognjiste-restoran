import { ChangeDetectionStrategy, Component, inject, signal } from '@angular/core';
import { toSignal } from '@angular/core/rxjs-interop';
import { AbstractControl, FormBuilder, ReactiveFormsModule, ValidationErrors, Validators } from '@angular/forms';
import { RouterLink } from '@angular/router';

import { greskePoPolju, porukaGreske } from '../../../core/greske/poruke-gresaka';
import { AuthServis } from '../../../core/servisi/auth.servis';
import { AuthOkvir } from '../okvir/auth-okvir';
import { IME_OBRAZAC, TELEFON_OBRAZAC } from '../../../core/validacija/obrasci';

function lozinkeSePoklapaju(grupa: AbstractControl): ValidationErrors | null {
  const lozinka = grupa.get('lozinka')?.value;
  const potvrda = grupa.get('potvrdaLozinke')?.value;
  return !potvrda || lozinka === potvrda ? null : { nepoklapanje: true };
}

@Component({
  selector: 'og-registracija',
  imports: [ReactiveFormsModule, RouterLink, AuthOkvir],
  changeDetection: ChangeDetectionStrategy.OnPush,
  templateUrl: './registracija.html',
  styleUrl: './registracija.scss',
})
export class Registracija {
  private readonly fb = inject(FormBuilder);
  private readonly auth = inject(AuthServis);

  protected readonly salje = signal(false);
  protected readonly greska = signal<string | null>(null);
  protected readonly gotovo = signal(false);
  protected readonly poslataAdresa = signal('');

  protected readonly forma = this.fb.nonNullable.group(
    {
      ime: [
        '',
        [Validators.required, Validators.minLength(2), Validators.maxLength(100), Validators.pattern(IME_OBRAZAC)],
      ],
      prezime: [
        '',
        [Validators.required, Validators.minLength(2), Validators.maxLength(100), Validators.pattern(IME_OBRAZAC)],
      ],
      email: ['', [Validators.required, Validators.email]],
      brojTelefona: ['', Validators.pattern(TELEFON_OBRAZAC)],
      lozinka: [
        '',
        [Validators.required, Validators.minLength(8), Validators.pattern(/^(?=.*[A-Z])(?=.*[0-9]).+$/)],
      ],
      potvrdaLozinke: ['', Validators.required],
    },
    { validators: lozinkeSePoklapaju },
  );

  private readonly lozinka = toSignal(this.forma.controls.lozinka.valueChanges, { initialValue: '' });

  protected readonly pravila = [
    { tekst: 'Bar 8 karaktera', vazi: (l: string) => l.length >= 8 },
    { tekst: 'Bar jedno veliko slovo', vazi: (l: string) => /[A-Z]/.test(l) },
    { tekst: 'Bar jedan broj', vazi: (l: string) => /[0-9]/.test(l) },
  ];

  protected ispunjeno(vazi: (l: string) => boolean): boolean {
    return vazi(this.lozinka());
  }

  protected posalji(): void {
    this.greska.set(null);

    if (this.forma.invalid) {
      this.forma.markAllAsTouched();
      return;
    }

    this.salje.set(true);
    const v = this.forma.getRawValue();

    this.auth
      .registracija({
        ime: v.ime.trim(),
        prezime: v.prezime.trim(),
        email: v.email.trim(),
        lozinka: v.lozinka,
        brojTelefona: v.brojTelefona.trim() || null,
      })
      .subscribe({
        next: () => {
          this.salje.set(false);
          this.poslataAdresa.set(v.email.trim());
          this.gotovo.set(true);
        },
        error: (g) => {
          this.salje.set(false);
          this.greska.set(porukaGreske(g, 'Registracija nije uspela.'));

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
