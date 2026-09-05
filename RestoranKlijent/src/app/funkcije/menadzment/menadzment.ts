import { ChangeDetectionStrategy, Component, inject } from '@angular/core';
import { RouterLink, RouterLinkActive, RouterOutlet } from '@angular/router';

import { AuthServis } from '../../core/servisi/auth.servis';

@Component({
  selector: 'og-menadzment',
  imports: [RouterOutlet, RouterLink, RouterLinkActive],
  changeDetection: ChangeDetectionStrategy.OnPush,
  templateUrl: './menadzment.html',
  styleUrl: './menadzment.scss',
})
export class Menadzment {
  protected readonly auth = inject(AuthServis);

  protected readonly kartice = [
    { putanja: 'izvestaji', naziv: 'Izveštaji' },
    { putanja: 'raspored', naziv: 'Raspored' },
    { putanja: 'zalihe', naziv: 'Zalihe' },
    { putanja: 'bonusi', naziv: 'Bonusi' },
  ];
}
