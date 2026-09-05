import { ChangeDetectionStrategy, Component, computed, inject, signal } from '@angular/core';
import { RouterLink } from '@angular/router';

import { DatumCev } from '../../../core/pipes/datum.pipe';
import { porukaGreske } from '../../../core/greske/poruke-gresaka';
import { KategorijaPoruke, Poruka, StatusPoruke } from '../../../core/modeli/api.modeli';
import { ProfilServis } from '../../../core/servisi/profil.servis';
import { izServera } from '../../../core/tekst/vreme';

@Component({
  selector: 'og-moje-poruke',
  imports: [RouterLink, DatumCev],
  changeDetection: ChangeDetectionStrategy.OnPush,
  templateUrl: './poruke.html',
  styleUrl: './poruke.scss',
})
export class MojePoruke {
  private readonly servis = inject(ProfilServis);

  protected readonly poruke = signal<Poruka[]>([]);
  protected readonly ucitava = signal(true);
  protected readonly greska = signal<string | null>(null);

  protected readonly prazno = computed(() => !this.ucitava() && !this.greska() && this.poruke().length === 0);

  protected readonly sortirane = computed(() =>
    [...this.poruke()].sort(
      (a, b) => (izServera(b.datumSlanja)?.getTime() ?? 0) - (izServera(a.datumSlanja)?.getTime() ?? 0),
    ),
  );

  protected readonly nazivKategorije: Record<KategorijaPoruke, string> = {
    Pitanje: 'Pitanje',
    Sugestija: 'Sugestija',
    Rezervacija: 'Zahtev za rezervaciju',
    Zalba: 'Žalba',
    Pohvala: 'Pohvala',
  };

  protected readonly nazivStatusa: Record<StatusPoruke, string> = {
    Novo: 'Novo',
    Procitano: 'Pročitano',
    Odgovoreno: 'Odgovoreno',
  };

  constructor() {
    this.ucitaj();
  }

  protected ucitaj(): void {
    this.ucitava.set(true);
    this.greska.set(null);

    this.servis.mojePoruke().subscribe({
      next: (p) => {
        this.poruke.set(p);
        this.ucitava.set(false);
      },
      error: (g) => {
        this.greska.set(porukaGreske(g, 'Poruke nisu učitane.'));
        this.ucitava.set(false);
      },
    });
  }
}
