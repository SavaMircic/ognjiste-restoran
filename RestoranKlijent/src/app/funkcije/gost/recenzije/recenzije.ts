import { ChangeDetectionStrategy, Component, computed, inject, signal } from '@angular/core';
import { RouterLink } from '@angular/router';

import { porukaGreske } from '../../../core/greske/poruke-gresaka';
import { Recenzija, TipRecenzije } from '../../../core/modeli/api.modeli';
import { RecenzijeServis } from '../../../core/servisi/recenzije.servis';
import { saBrojem } from '../../../core/tekst/mnozina';
import { KarticaRecenzije } from '../../zajednicko/kartica-recenzije/kartica-recenzije';
import { Paginacija } from '../../zajednicko/paginacija/paginacija';

type Sortiranje = '' | 'najbolje-ocenjeno';

const VELICINA_STRANE = 9;

@Component({
  selector: 'og-recenzije',
  imports: [RouterLink, KarticaRecenzije, Paginacija],
  changeDetection: ChangeDetectionStrategy.OnPush,
  templateUrl: './recenzije.html',
  styleUrl: './recenzije.scss',
})
export class Recenzije {
  private readonly servis = inject(RecenzijeServis);

  protected readonly tip = signal<TipRecenzije | null>(null);
  protected readonly sortiranje = signal<Sortiranje>('');
  protected readonly strana = signal(1);

  protected readonly recenzije = signal<Recenzija[]>([]);
  protected readonly ukupnoZapisa = signal(0);
  protected readonly ukupnoStrana = signal(0);
  protected readonly ucitava = signal(true);
  protected readonly greska = signal<string | null>(null);

  protected readonly prazno = computed(() => !this.ucitava() && !this.greska() && this.recenzije().length === 0);

  protected readonly oznakaUkupno = computed(() =>
    saBrojem(this.ukupnoZapisa(), 'recenzija', 'recenzije', 'recenzija'),
  );

  protected readonly tipovi: { vrednost: TipRecenzije | null; naziv: string }[] = [
    { vrednost: null, naziv: 'Sve' },
    { vrednost: 'Jelo', naziv: 'O jelima' },
    { vrednost: 'Usluga', naziv: 'O usluzi' },
    { vrednost: 'Restoran', naziv: 'O restoranu' },
  ];

  constructor() {
    this.ucitaj();
  }

  protected izaberiTip(vrednost: TipRecenzije | null): void {
    this.tip.set(vrednost);
    this.strana.set(1);
    this.ucitaj();
  }

  protected promeniSortiranje(dogadjaj: Event): void {
    this.sortiranje.set((dogadjaj.target as HTMLSelectElement).value as Sortiranje);
    this.strana.set(1);
    this.ucitaj();
  }

  protected promeniStranu(strana: number): void {
    this.strana.set(strana);
    this.ucitaj();
    window.scrollTo({ top: 0, behavior: 'smooth' });
  }

  protected ucitaj(): void {
    this.ucitava.set(true);
    this.greska.set(null);

    this.servis
      .pretrazi({
        tipRecenzije: this.tip(),
        sortiranje: this.sortiranje() || null,
        strana: this.strana(),
        velicinaStrane: VELICINA_STRANE,
      })
      .subscribe({
        next: (odgovor) => {
          this.recenzije.set(odgovor.podaci);
          this.ukupnoZapisa.set(odgovor.ukupnoZapisa);
          this.ukupnoStrana.set(odgovor.ukupnoStrana);
          this.ucitava.set(false);
        },
        error: (g) => {
          this.recenzije.set([]);
          this.greska.set(porukaGreske(g, 'Recenzije trenutno nisu dostupne.'));
          this.ucitava.set(false);
        },
      });
  }
}
