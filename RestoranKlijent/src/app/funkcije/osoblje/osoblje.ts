import { ChangeDetectionStrategy, Component, computed, inject } from '@angular/core';
import { RouterLink, RouterLinkActive, RouterOutlet } from '@angular/router';

import { ULOGE_SMENE } from '../../core/modeli/api.modeli';
import { alatiZa } from '../../core/navigacija';
import { AuthServis } from '../../core/servisi/auth.servis';
import { nazivUloge } from '../../core/tekst/uloge';

@Component({
  selector: 'og-osoblje',
  imports: [RouterOutlet, RouterLink, RouterLinkActive],
  changeDetection: ChangeDetectionStrategy.OnPush,
  templateUrl: './osoblje.html',
  styleUrl: './osoblje.scss',
})
export class Osoblje {
  protected readonly auth = inject(AuthServis);

  protected readonly mojeUloge = computed(() => this.auth.uloge().map(nazivUloge).join(', '));

  protected readonly radiSmenu = computed(() => this.auth.imaUlogu(ULOGE_SMENE));

  protected readonly kartice = computed(() => {
    const zaSve = [{ putanja: 'raspored', naziv: 'Moj raspored' }];
    const zaSmenu = this.radiSmenu()
      ? [
          { putanja: 'statistika', naziv: 'Moj učinak' },
          { putanja: 'istorija', naziv: 'Istorija rada' },
        ]
      : [];

    return [
      ...zaSve,
      ...zaSmenu,
      { putanja: 'bonusi', naziv: 'Bonusi' },
      { putanja: 'prijave', naziv: 'Prijave problema' },
    ];
  });

  protected readonly alati = computed(() => alatiZa(this.auth.uloge()));
}
