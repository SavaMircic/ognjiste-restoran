import { ChangeDetectionStrategy, Component, computed, inject, signal } from '@angular/core';
import { RouterLink, RouterLinkActive } from '@angular/router';

import { ULOGE_OSOBLJA } from '../../core/modeli/api.modeli';
import { alatiZa } from '../../core/navigacija';
import { AuthServis } from '../../core/servisi/auth.servis';
import { Logo } from '../logo/logo';

@Component({
  selector: 'og-header',
  imports: [RouterLink, RouterLinkActive, Logo],
  changeDetection: ChangeDetectionStrategy.OnPush,
  templateUrl: './header.html',
  styleUrl: './header.scss',
})
export class Header {
  protected readonly auth = inject(AuthServis);

  protected readonly jeOsoblje = computed(() => this.auth.imaUlogu(ULOGE_OSOBLJA));

  protected readonly alati = computed(() => alatiZa(this.auth.uloge()));

  protected readonly otvoren = signal(false);

  protected readonly veze = [
    { putanja: '/meni', naziv: 'Meni' },
    { putanja: '/recenzije', naziv: 'Recenzije' },
    { putanja: '/galerija', naziv: 'Galerija' },
    { putanja: '/nas-tim', naziv: 'Naš tim' },
    { putanja: '/kontakt', naziv: 'Kontakt' },
  ];

  protected prebaci(): void {
    this.otvoren.update((v) => !v);
  }

  protected zatvori(): void {
    this.otvoren.set(false);
  }
}
