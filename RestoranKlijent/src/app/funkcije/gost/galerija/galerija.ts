import { ChangeDetectionStrategy, Component, computed, inject, signal } from '@angular/core';
import { RouterLink } from '@angular/router';

import { RezervnaSlika } from '../../../core/direktive/rezervna-slika.direktiva';
import { uOblasti } from '../../../core/galerija/oblasti';
import { porukaGreske } from '../../../core/greske/poruke-gresaka';
import { GalerijaSlika } from '../../../core/modeli/api.modeli';
import { SlikaCev } from '../../../core/pipes/slika.pipe';
import { SadrzajServis } from '../../../core/servisi/sadrzaj.servis';
import { saBrojem } from '../../../core/tekst/mnozina';
import { GalerijaSlajder } from '../../zajednicko/galerija-slajder/galerija-slajder';

@Component({
  selector: 'og-galerija',
  imports: [RouterLink, SlikaCev, RezervnaSlika, GalerijaSlajder],
  changeDetection: ChangeDetectionStrategy.OnPush,
  templateUrl: './galerija.html',
  styleUrl: './galerija.scss',
})
export class Galerija {
  private readonly servis = inject(SadrzajServis);

  protected readonly slike = signal<GalerijaSlika[]>([]);
  protected readonly ucitava = signal(true);
  protected readonly greska = signal<string | null>(null);

  protected readonly otvorenaOblast = signal<string | null>(null);

  protected readonly saBrojem = saBrojem;

  protected readonly oblasti = computed(() => uOblasti(this.slike()));

  protected readonly prazno = computed(
    () => !this.ucitava() && !this.greska() && this.oblasti().length === 0,
  );

  protected readonly oblast = computed(() => {
    const naziv = this.otvorenaOblast();
    return naziv === null ? null : (this.oblasti().find((o) => o.naziv === naziv) ?? null);
  });

  constructor() {
    this.ucitaj();
  }

  protected ucitaj(): void {
    this.ucitava.set(true);
    this.greska.set(null);

    this.servis.galerija().subscribe({
      next: (s) => {
        this.slike.set(s);
        this.ucitava.set(false);
      },
      error: (g) => {
        this.greska.set(porukaGreske(g, 'Galerija trenutno nije dostupna.'));
        this.ucitava.set(false);
      },
    });
  }

  protected otvori(oblast: string): void {
    this.otvorenaOblast.set(oblast);
  }

  protected zatvori(): void {
    this.otvorenaOblast.set(null);
  }
}
