import {
  ChangeDetectionStrategy,
  Component,
  OnInit,
  computed,
  inject,
  input,
  signal,
} from '@angular/core';
import { RouterLink } from '@angular/router';

import { porukaGreske } from '../../../core/greske/poruke-gresaka';
import { AuthServis } from '../../../core/servisi/auth.servis';
import { AuthOkvir } from '../okvir/auth-okvir';

type Stanje = 'radi' | 'uspeh' | 'greska' | 'nedostajuPodaci';

@Component({
  selector: 'og-potvrda-email',
  imports: [RouterLink, AuthOkvir],
  changeDetection: ChangeDetectionStrategy.OnPush,
  templateUrl: './potvrda-email.html',
  styleUrl: './potvrda-email.scss',
})
export class PotvrdaEmail implements OnInit {
  private readonly auth = inject(AuthServis);

  readonly korisnikId = input<string>();
  readonly token = input<string>();

  protected readonly stanje = signal<Stanje>('radi');
  protected readonly poruka = signal('');

  protected readonly naslov = computed(() => {
    switch (this.stanje()) {
      case 'radi':
        return 'Potvrđujemo nalog…';
      case 'uspeh':
        return 'Nalog je potvrđen';
      case 'nedostajuPodaci':
        return 'Neispravan link';
      default:
        return 'Potvrda nije uspela';
    }
  });

  ngOnInit(): void {
    const id = this.korisnikId();
    const t = this.token();

    if (!id || !t) {
      this.stanje.set('nedostajuPodaci');
      this.poruka.set('Link nije potpun. Otvorite ga direktno iz mejla, bez prepisivanja.');
      return;
    }

    this.auth.potvrdaEmaila({ korisnikId: id, token: t }).subscribe({
      next: (o) => {
        this.stanje.set('uspeh');
        this.poruka.set(o.poruka ?? 'Email adresa je uspešno potvrđena.');
      },
      error: (g) => {
        this.stanje.set('greska');
        this.poruka.set(porukaGreske(g, 'Link je istekao ili je već iskorišćen.'));
      },
    });
  }
}
