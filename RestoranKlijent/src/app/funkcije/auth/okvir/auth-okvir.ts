import { ChangeDetectionStrategy, Component, input } from '@angular/core';
import { RouterLink } from '@angular/router';

import { Logo } from '../../../layout/logo/logo';

@Component({
  selector: 'og-auth-okvir',
  imports: [RouterLink, Logo],
  changeDetection: ChangeDetectionStrategy.OnPush,
  templateUrl: './auth-okvir.html',
  styleUrl: './auth-okvir.scss',
})
export class AuthOkvir {
  readonly naslov = input.required<string>();
  readonly podnaslov = input<string>('');
}
