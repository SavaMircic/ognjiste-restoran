import { DecimalPipe } from '@angular/common';
import { ChangeDetectionStrategy, Component, computed, inject, signal } from '@angular/core';

import { porukaGreske } from '../../../core/greske/poruke-gresaka';
import { Bonus } from '../../../core/modeli/api.modeli';
import { DatumCev } from '../../../core/pipes/datum.pipe';
import { OsobljeServis } from '../../../core/servisi/osoblje.servis';

@Component({
  selector: 'og-moji-bonusi',
  imports: [DecimalPipe, DatumCev],
  changeDetection: ChangeDetectionStrategy.OnPush,
  templateUrl: './bonusi.html',
  styleUrl: './bonusi.scss',
})
export class MojiBonusi {
  private readonly servis = inject(OsobljeServis);

  protected readonly bonusi = signal<Bonus[]>([]);
  protected readonly ucitava = signal(true);
  protected readonly greska = signal<string | null>(null);

  protected readonly aktivni = computed(() => this.bonusi().filter((b) => b.vazi));
  protected readonly istekli = computed(() => this.bonusi().filter((b) => !b.vazi));

  protected readonly prazno = computed(
    () => !this.ucitava() && !this.greska() && this.bonusi().length === 0,
  );

  protected readonly zbirAktivnih = computed(() =>
    this.aktivni().reduce((zbir, b) => zbir + b.iznos, 0),
  );

  constructor() {
    this.ucitaj();
  }

  protected ucitaj(): void {
    this.ucitava.set(true);
    this.greska.set(null);

    this.servis.mojiBonusi().subscribe({
      next: (b) => {
        this.bonusi.set(b);
        this.ucitava.set(false);
      },
      error: (g) => {
        this.greska.set(porukaGreske(g, 'Bonusi nisu učitani.'));
        this.ucitava.set(false);
      },
    });
  }
}
