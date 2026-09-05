import { ChangeDetectionStrategy, Component, computed, inject, signal } from '@angular/core';
import { RouterLink } from '@angular/router';

import { SlikaCev } from '../../../core/pipes/slika.pipe';
import { RezervnaSlika } from '../../../core/direktive/rezervna-slika.direktiva';
import { porukaGreske } from '../../../core/greske/poruke-gresaka';
import { ClanTima } from '../../../core/modeli/api.modeli';
import { SadrzajServis } from '../../../core/servisi/sadrzaj.servis';
import { nazivUloge } from '../../../core/tekst/uloge';

@Component({
  selector: 'og-nas-tim',
  imports: [RouterLink, SlikaCev, RezervnaSlika],
  changeDetection: ChangeDetectionStrategy.OnPush,
  templateUrl: './nas-tim.html',
  styleUrl: './nas-tim.scss',
})
export class NasTim {
  private readonly servis = inject(SadrzajServis);

  protected readonly clanovi = signal<ClanTima[]>([]);
  protected readonly ucitava = signal(true);
  protected readonly greska = signal<string | null>(null);

  protected readonly prazno = computed(() => !this.ucitava() && !this.greska() && this.clanovi().length === 0);

  protected readonly naziv = nazivUloge;

  constructor() {
    this.ucitaj();
  }

  protected ucitaj(): void {
    this.ucitava.set(true);
    this.greska.set(null);

    this.servis.nasTim().subscribe({
      next: (c) => {
        this.clanovi.set(c);
        this.ucitava.set(false);
      },
      error: (g) => {
        this.greska.set(porukaGreske(g, 'Podaci o timu trenutno nisu dostupni.'));
        this.ucitava.set(false);
      },
    });
  }

  protected inicijali(clan: ClanTima): string {
    return `${clan.ime[0] ?? ''}${clan.prezime[0] ?? ''}`.toUpperCase();
  }
}
