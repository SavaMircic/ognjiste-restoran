import { ChangeDetectionStrategy, Component, OnInit, inject, input, signal } from '@angular/core';
import { toSignal } from '@angular/core/rxjs-interop';
import { AbstractControl, FormBuilder, ReactiveFormsModule, ValidationErrors, Validators } from '@angular/forms';
import { RouterLink } from '@angular/router';

import { greskePoPolju, porukaGreske } from '../../../core/greske/poruke-gresaka';
import { AuthServis } from '../../../core/servisi/auth.servis';
import { AuthOkvir } from '../okvir/auth-okvir';

function lozinkeSePoklapaju(grupa: AbstractControl): ValidationErrors | null {
  const nova = grupa.get('novaLozinka')?.value;
  const potvrda = grupa.get('potvrda')?.value;
  return !potvrda || nova === potvrda ? null : { nepoklapanje: true };
}

@Component({
  selector: 'og-reset-lozinke',
  imports: [ReactiveFormsModule, RouterLink, AuthOkvir],
  changeDetection: ChangeDetectionStrategy.OnPush,
  templateUrl: './reset-lozinke.html',
  styleUrl: './reset-lozinke.scss',
})
export class ResetLozinke implements OnInit {
  private readonly fb = inject(FormBuilder);
  private readonly auth = inject(AuthServis);

  readonly email = input<string>();
  readonly token = input<string>();

  protected readonly salje = signal(false);
  protected readonly greska = signal<string | null>(null);
  protected readonly gotovo = signal(false);

  protected readonly nepotpunLink = signal(false);

  protected readonly forma = this.fb.nonNullable.group(
    {
      novaLozinka: [
        '',
        [Validators.required, Validators.minLength(8), Validators.pattern(/^(?=.*[A-Z])(?=.*[0-9]).+$/)],
      ],
      potvrda: ['', Validators.required],
    },
    { validators: lozinkeSePoklapaju },
  );

  private readonly lozinka = toSignal(this.forma.controls.novaLozinka.valueChanges, { initialValue: '' });

  protected readonly pravila = [
    { tekst: 'Bar 8 karaktera', vazi: (l: string) => l.length >= 8 },
    { tekst: 'Bar jedno veliko slovo', vazi: (l: string) => /[A-Z]/.test(l) },
    { tekst: 'Bar jedan broj', vazi: (l: string) => /[0-9]/.test(l) },
  ];

  ngOnInit(): void {
    if (!this.email() || !this.token()) {
      this.nepotpunLink.set(true);
      this.forma.disable();
    }
  }

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

    this.auth
      .resetujLozinku({
        email: this.email() ?? '',
        token: this.token() ?? '',
        novaLozinka: this.forma.getRawValue().novaLozinka,
      })
      .subscribe({
        next: () => {
          this.salje.set(false);
          this.gotovo.set(true);
        },
        error: (g) => {
          this.salje.set(false);

          this.greska.set(porukaGreske(g, 'Lozinka nije promenjena.'));

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
