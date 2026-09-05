import { ChangeDetectionStrategy, Component, inject } from '@angular/core';
import { RouterLink, RouterLinkActive, RouterOutlet } from '@angular/router';

import { AuthServis } from '../../core/servisi/auth.servis';

@Component({
  selector: 'og-administracija',
  imports: [RouterOutlet, RouterLink, RouterLinkActive],
  changeDetection: ChangeDetectionStrategy.OnPush,
  templateUrl: './administracija.html',
  styleUrl: './administracija.scss',
})
export class Administracija {
  protected readonly auth = inject(AuthServis);

  protected readonly kartice = [
    { putanja: 'rezervacije', naziv: 'Rezervacije' },
    { putanja: 'poruke', naziv: 'Poruke' },
    { putanja: 'korisnici', naziv: 'Korisnici' },
    { putanja: 'zaposleni', naziv: 'Zaposleni' },
    { putanja: 'recenzije', naziv: 'Recenzije' },
    { putanja: 'meni', naziv: 'Jela' },
    { putanja: 'kategorije', naziv: 'Kategorije' },
    { putanja: 'galerija', naziv: 'Galerija' },
    { putanja: 'tim', naziv: 'Naš tim' },
    { putanja: 'postavke', naziv: 'Postavke' },
    { putanja: 'dnevnik', naziv: 'Dnevnik' },
  ];
}
