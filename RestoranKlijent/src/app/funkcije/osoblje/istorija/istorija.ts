import { DecimalPipe } from '@angular/common';
import { ChangeDetectionStrategy, Component, computed, inject, signal } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';

import { porukaGreske } from '../../../core/greske/poruke-gresaka';
import { IstorijaDan } from '../../../core/modeli/api.modeli';
import { DatumCev } from '../../../core/pipes/datum.pipe';
import { OsobljeServis } from '../../../core/servisi/osoblje.servis';
import { saBrojem } from '../../../core/tekst/mnozina';

type Predlog = 7 | 30 | 90;

@Component({
  selector: 'og-istorija-rada',
  imports: [ReactiveFormsModule, DecimalPipe, DatumCev],
  changeDetection: ChangeDetectionStrategy.OnPush,
  templateUrl: './istorija.html',
  styleUrl: './istorija.scss',
})
export class IstorijaRada {
  private readonly fb = inject(FormBuilder);
  private readonly servis = inject(OsobljeServis);

  protected readonly dani = signal<IstorijaDan[]>([]);
  protected readonly ucitava = signal(true);
  protected readonly greska = signal<string | null>(null);

  private readonly otvoreni = signal<ReadonlySet<string>>(new Set());

  protected readonly forma = this.fb.nonNullable.group({
    datumOd: [this.preDana(30), Validators.required],
    datumDo: [this.preDana(0), Validators.required],
  });

  protected readonly prazno = computed(
    () => !this.ucitava() && !this.greska() && this.dani().length === 0,
  );

  protected readonly zbir = computed(() => {
    const d = this.dani();
    return {
      racuna: d.reduce((z, x) => z + x.brojRacuna, 0),
      promet: d.reduce((z, x) => z + x.prometRacuna, 0),
      napojnica: d.reduce((z, x) => z + x.napojnica, 0),
      stavki: d.reduce((z, x) => z + x.brojPripremljenihStavki, 0),
      vrednostPripreme: d.reduce((z, x) => z + x.vrednostPripremljenog, 0),
    };
  });

  protected readonly imaRacuna = computed(() => this.zbir().racuna > 0);
  protected readonly imaPripreme = computed(() => this.zbir().stavki > 0);

  constructor() {
    this.ucitaj();
  }

  protected otvoren(datum: string): boolean {
    return this.otvoreni().has(datum);
  }

  protected prebaci(datum: string): void {
    this.otvoreni.update((stari) => {
      const novi = new Set(stari);
      if (!novi.delete(datum)) novi.add(datum);
      return novi;
    });
  }

  protected racuna(broj: number): string {
    return saBrojem(broj, 'račun', 'računa', 'računa');
  }

  protected stavki(broj: number): string {
    return saBrojem(broj, 'stavka', 'stavke', 'stavki');
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

    this.servis.mojaIstorija(datumOd, datumDo).subscribe({
      next: (d) => {
        this.dani.set(d);
        this.otvoreni.set(d.length ? new Set([d[0].datum]) : new Set());
        this.ucitava.set(false);
      },
      error: (g) => {
        this.dani.set([]);
        this.greska.set(porukaGreske(g, 'Istorija nije učitana.'));
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
