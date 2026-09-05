import { ChangeDetectionStrategy, Component, inject, signal } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { RouterLink } from '@angular/router';

import { porukaGreske } from '../../../core/greske/poruke-gresaka';
import { AuthServis } from '../../../core/servisi/auth.servis';
import { AuthOkvir } from '../okvir/auth-okvir';

@Component({
  selector: 'og-posalji-ponovo',
  imports: [ReactiveFormsModule, RouterLink, AuthOkvir],
  changeDetection: ChangeDetectionStrategy.OnPush,
  templateUrl: './posalji-ponovo.html',
  styleUrl: './posalji-ponovo.scss',
})
export class PosaljiPonovo {
  private readonly fb = inject(FormBuilder);
  private readonly auth = inject(AuthServis);

  protected readonly salje = signal(false);
  protected readonly greska = signal<string | null>(null);
  protected readonly gotovo = signal(false);
  protected readonly odgovor = signal('');

  protected readonly forma = this.fb.nonNullable.group({
    email: ['', [Validators.required, Validators.email]],
  });

  protected posalji(): void {
    this.greska.set(null);

    if (this.forma.invalid) {
      this.forma.markAllAsTouched();
      return;
    }

    this.salje.set(true);

    this.auth.posaljiPonovoPotvrdu({ email: this.forma.getRawValue().email.trim() }).subscribe({
      next: (o) => {
        this.salje.set(false);
        this.odgovor.set(o.poruka ?? '');
        this.gotovo.set(true);
      },
      error: (g) => {
        this.salje.set(false);
        this.greska.set(porukaGreske(g, 'Slanje nije uspelo.'));
      },
    });
  }
}
