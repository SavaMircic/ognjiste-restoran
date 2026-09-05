import { ChangeDetectionStrategy, Component, computed, input } from '@angular/core';

@Component({
  selector: 'og-zvezdice',
  changeDetection: ChangeDetectionStrategy.OnPush,
  templateUrl: './zvezdice.html',
  styleUrl: './zvezdice.scss',
})
export class Zvezdice {
  readonly ocena = input.required<number | null>();

  protected readonly zvezde = computed<boolean[]>(() => {
    const o = this.ocena();
    if (o === null) return [];
    return [1, 2, 3, 4, 5].map((i) => i <= Math.round(o));
  });

  protected readonly opis = computed(() => `Ocena ${this.ocena()} od 5`);
}
