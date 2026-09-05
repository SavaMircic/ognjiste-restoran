import { ChangeDetectionStrategy, Component, computed, effect, input, output, signal } from '@angular/core';

import { RezervnaSlika } from '../../../core/direktive/rezervna-slika.direktiva';
import { GalerijaOblast } from '../../../core/modeli/api.modeli';
import { SlikaCev } from '../../../core/pipes/slika.pipe';

@Component({
  selector: 'og-galerija-slajder',
  imports: [SlikaCev, RezervnaSlika],
  changeDetection: ChangeDetectionStrategy.OnPush,
  host: {
    '(document:keydown.escape)': 'zatvoreno.emit()',
    '(document:keydown.arrowLeft)': 'pomeri(-1)',
    '(document:keydown.arrowRight)': 'pomeri(1)',
  },
  templateUrl: './galerija-slajder.html',
  styleUrl: './galerija-slajder.scss',
})
export class GalerijaSlajder {
  readonly oblast = input.required<GalerijaOblast>();
  readonly pocetni = input(0);

  readonly zatvoreno = output<void>();

  protected readonly indeks = signal(0);

  protected readonly slika = computed(() => this.oblast().slike[this.indeks()] ?? null);

  constructor() {
    effect(() => {
      this.oblast();
      this.indeks.set(this.pocetni());
    });
  }

  protected pomeri(korak: number): void {
    const ukupno = this.oblast().slike.length;
    if (ukupno === 0) return;
    this.indeks.update((i) => (i + korak + ukupno) % ukupno);
  }

  protected naSlicicu(i: number): void {
    this.indeks.set(i);
  }
}
