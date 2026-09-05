import { ChangeDetectionStrategy, Component, computed, inject, signal } from '@angular/core';
import { RouterLink } from '@angular/router';
import { catchError, of } from 'rxjs';

import { porukaGreske } from '../../../core/greske/poruke-gresaka';
import { KategorijaMenija, StavkaMenija } from '../../../core/modeli/api.modeli';
import { MeniServis } from '../../../core/servisi/meni.servis';
import { saBrojem } from '../../../core/tekst/mnozina';
import { KarticaJela } from '../../zajednicko/kartica-jela/kartica-jela';
import { Paginacija } from '../../zajednicko/paginacija/paginacija';

type Sortiranje = '' | 'cena' | 'popularnost' | 'ocena';

const PO_STRANI = 9;

@Component({
  selector: 'og-meni',
  imports: [RouterLink, KarticaJela, Paginacija],
  changeDetection: ChangeDetectionStrategy.OnPush,
  templateUrl: './meni.html',
  styleUrl: './meni.scss',
})
export class Meni {
  private readonly servis = inject(MeniServis);

  protected readonly kategorijaId = signal<number | null>(null);
  protected readonly sortiranje = signal<Sortiranje>('');
  protected readonly strana = signal(1);

  protected readonly kategorije = signal<KategorijaMenija[]>([]);
  protected readonly stavke = signal<StavkaMenija[]>([]);
  protected readonly ukupnoStrana = signal(1);
  protected readonly ukupnoZapisa = signal(0);
  protected readonly ucitava = signal(true);
  protected readonly greska = signal<string | null>(null);

  protected readonly saBrojem = saBrojem;

  protected readonly prazno = computed(() => !this.ucitava() && this.stavke().length === 0);

  constructor() {
    this.servis
      .kategorije()
      .pipe(catchError(() => of([] as KategorijaMenija[])))
      .subscribe((k) => this.kategorije.set(k));

    this.ucitaj();
  }

  protected izaberiKategoriju(id: number | null): void {
    this.kategorijaId.set(id);
    this.strana.set(1);
    this.ucitaj();
  }

  protected promeniSortiranje(dogadjaj: Event): void {
    this.sortiranje.set((dogadjaj.target as HTMLSelectElement).value as Sortiranje);
    this.strana.set(1);
    this.ucitaj();
  }

  protected naStranu(strana: number): void {
    this.strana.set(strana);
    this.ucitaj();
    window.scrollTo({ top: 0, behavior: 'smooth' });
  }

  protected ucitaj(): void {
    this.ucitava.set(true);
    this.greska.set(null);

    this.servis
      .stavke({
        kategorijaId: this.kategorijaId(),
        sortiranje: this.sortiranje() || null,
        strana: this.strana(),
        velicinaStrane: PO_STRANI,
      })
      .subscribe({
        next: (odgovor) => {
          this.stavke.set(odgovor.podaci);
          this.ukupnoStrana.set(Math.max(1, odgovor.ukupnoStrana));
          this.ukupnoZapisa.set(odgovor.ukupnoZapisa);
          this.ucitava.set(false);
        },
        error: (g) => {
          this.greska.set(porukaGreske(g, 'Meni trenutno nije dostupan.'));
          this.ucitava.set(false);
        },
      });
  }
}
