import { ChangeDetectionStrategy, Component, inject, signal } from '@angular/core';
import { RouterLink } from '@angular/router';
import { catchError, of } from 'rxjs';

import { Postavke } from '../../core/modeli/api.modeli';
import { AuthServis } from '../../core/servisi/auth.servis';
import { SadrzajServis } from '../../core/servisi/sadrzaj.servis';
import { Logo } from '../logo/logo';

@Component({
  selector: 'og-footer',
  imports: [RouterLink, Logo],
  changeDetection: ChangeDetectionStrategy.OnPush,
  templateUrl: './footer.html',
  styleUrl: './footer.scss',
})
export class Footer {
  private readonly sadrzaj = inject(SadrzajServis);

  protected readonly auth = inject(AuthServis);
  protected readonly godina = new Date().getFullYear();
  protected readonly postavke = signal<Postavke | null>(null);

  constructor() {
    this.sadrzaj
      .postavke()
      .pipe(catchError(() => of(null)))
      .subscribe((p) => p && this.postavke.set(p));
  }
}
