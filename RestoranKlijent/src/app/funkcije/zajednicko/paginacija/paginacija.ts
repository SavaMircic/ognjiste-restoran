import { ChangeDetectionStrategy, Component, computed, input, output } from '@angular/core';

@Component({
  selector: 'og-paginacija',
  changeDetection: ChangeDetectionStrategy.OnPush,
  templateUrl: './paginacija.html',
  styleUrl: './paginacija.scss',
})
export class Paginacija {
  readonly strana = input.required<number>();
  readonly ukupnoStrana = input.required<number>();

  readonly promena = output<number>();

  protected readonly brojevi = computed<(number | null)[]>(() => {
    const ukupno = this.ukupnoStrana();
    const tekuca = this.strana();

    if (ukupno <= 7) return Array.from({ length: ukupno }, (_, i) => i + 1);

    const rezultat: (number | null)[] = [1];
    const od = Math.max(2, tekuca - 1);
    const doo = Math.min(ukupno - 1, tekuca + 1);

    if (od > 2) rezultat.push(null);
    for (let i = od; i <= doo; i++) rezultat.push(i);
    if (doo < ukupno - 1) rezultat.push(null);

    rezultat.push(ukupno);
    return rezultat;
  });

  protected idi(strana: number): void {
    if (strana < 1 || strana > this.ukupnoStrana() || strana === this.strana()) return;
    this.promena.emit(strana);
  }
}
