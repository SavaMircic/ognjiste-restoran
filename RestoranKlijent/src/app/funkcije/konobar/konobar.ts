import { ChangeDetectionStrategy, Component, inject } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { RouterLink, RouterLinkActive, RouterOutlet } from '@angular/router';

import { AuthServis } from '../../core/servisi/auth.servis';
import { KuhinjaVezaServis } from '../../core/servisi/kuhinja-veza.servis';
import { ObavestenjaServis } from '../../core/servisi/obavestenja.servis';

@Component({
  selector: 'og-konobar',
  imports: [RouterOutlet, RouterLink, RouterLinkActive],
  changeDetection: ChangeDetectionStrategy.OnPush,
  templateUrl: './konobar.html',
  styleUrl: './konobar.scss',
})
export class Konobar {
  private readonly obavestenja = inject(ObavestenjaServis);
  protected readonly auth = inject(AuthServis);
  protected readonly veza = inject(KuhinjaVezaServis);

  protected readonly kartice = [
    { putanja: 'stolovi', naziv: 'Stolovi' },
    { putanja: 'rezervacije', naziv: 'Rezervacije danas' },
  ];

  constructor() {
    void this.veza.povezi();

    this.veza.stavkaSpremna.pipe(takeUntilDestroyed()).subscribe((s) => {
      this.obavestenja.info(`Sto ${s.brojStola}: ${s.nazivStavke} je spremno.`);
    });
  }
}
