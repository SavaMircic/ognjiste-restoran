import { DecimalPipe } from '@angular/common';
import { ChangeDetectionStrategy, Component, computed, inject, signal } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';

import { porukaGreske } from '../../../core/greske/poruke-gresaka';
import { UcinakZaposlenog } from '../../../core/modeli/api.modeli';
import { OsobljeServis } from '../../../core/servisi/osoblje.servis';

type Predlog = 7 | 30 | 90;

@Component({
  selector: 'og-moja-statistika',
  imports: [ReactiveFormsModule, DecimalPipe],
  changeDetection: ChangeDetectionStrategy.OnPush,
  templateUrl: './statistika.html',
  styleUrl: './statistika.scss',
})
export class MojaStatistika {
  private readonly fb = inject(FormBuilder);
  private readonly servis = inject(OsobljeServis);

  protected readonly ucinak = signal<UcinakZaposlenog | null>(null);
  protected readonly ucitava = signal(true);
  protected readonly greska = signal<string | null>(null);

  protected readonly forma = this.fb.nonNullable.group({
    datumOd: [this.preDana(30), Validators.required],
    datumDo: [this.preDana(0), Validators.required],
  });

  protected readonly imaKonobarske = computed(() => (this.ucinak()?.brojStolova ?? 0) > 0);

  protected readonly imaPripreme = computed(() => (this.ucinak()?.brojPripremljenihStavki ?? 0) > 0);

  protected readonly praznPeriod = computed(
    () => !this.ucitava() && !this.greska() && !this.imaKonobarske() && !this.imaPripreme(),
  );

  constructor() {
    this.ucitaj();
  }

  protected izaberiPredlog(dana: Predlog): void {
    this.forma.setValue({ datumOd: this.preDana(dana), datumDo: this.preDana(0) });
    this.ucitaj();
  }

  protected ucitaj(): void {
    if (this.forma.invalid) {
      this.forma.markAllAsTouched();
      return;
    }

    this.ucitava.set(true);
    this.greska.set(null);

    const { datumOd, datumDo } = this.forma.getRawValue();

    this.servis.mojaStatistika(datumOd, datumDo).subscribe({
      next: (u) => {
        this.ucinak.set(u);
        this.ucitava.set(false);
      },
      error: (g) => {
        this.ucinak.set(null);
        this.greska.set(porukaGreske(g, 'Statistika nije učitana.'));
        this.ucitava.set(false);
      },
    });
  }

  private preDana(dana: number): string {
    const d = new Date();
    d.setDate(d.getDate() - dana);
    const p = (n: number) => String(n).padStart(2, '0');
    return `${d.getFullYear()}-${p(d.getMonth() + 1)}-${p(d.getDate())}`;
  }
}
