import { ChangeDetectionStrategy, Component, computed, inject } from '@angular/core';
import { RouterLink, RouterLinkActive, RouterOutlet } from '@angular/router';

import { AuthServis } from '../../core/servisi/auth.servis';

@Component({
  selector: 'og-nalog',
  imports: [RouterOutlet, RouterLink, RouterLinkActive],
  changeDetection: ChangeDetectionStrategy.OnPush,
  templateUrl: './nalog.html',
  styleUrl: './nalog.scss',
})
export class Nalog {
  protected readonly auth = inject(AuthServis);

  protected readonly jeGost = computed(() => this.auth.imaUlogu(['Korisnik']));

  protected readonly kartice = computed(() => {
    const osnovne = [{ putanja: 'profil', naziv: 'Profil' }];

    return this.jeGost()
      ? [
          ...osnovne,
          { putanja: 'rezervacije', naziv: 'Rezervacije' },
          { putanja: 'recenzije', naziv: 'Moje recenzije' },
          { putanja: 'omiljena', naziv: 'Omiljena jela' },
          { putanja: 'poruke', naziv: 'Poruke' },
        ]
      : osnovne;
  });
}
