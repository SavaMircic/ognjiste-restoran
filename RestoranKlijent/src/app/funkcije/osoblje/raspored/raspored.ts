import { DatePipe } from '@angular/common';
import { ChangeDetectionStrategy, Component, computed, inject, signal } from '@angular/core';

import { porukaGreske } from '../../../core/greske/poruke-gresaka';
import { Smena } from '../../../core/modeli/api.modeli';
import { DatumCev } from '../../../core/pipes/datum.pipe';
import { OsobljeServis } from '../../../core/servisi/osoblje.servis';

const DANI = ['Nedelja', 'Ponedeljak', 'Utorak', 'Sreda', 'Četvrtak', 'Petak', 'Subota'];

interface DanRasporeda {
  datum: string;
  naziv: string;
  jeDanas: boolean;
  smene: Smena[];
}

@Component({
  selector: 'og-moj-raspored',
  imports: [DatePipe, DatumCev],
  changeDetection: ChangeDetectionStrategy.OnPush,
  templateUrl: './raspored.html',
  styleUrl: './raspored.scss',
})
export class MojRaspored {
  private readonly servis = inject(OsobljeServis);

  protected readonly pocetak = signal(this.ponedeljak(new Date()));
  protected readonly smene = signal<Smena[]>([]);
  protected readonly ucitava = signal(true);
  protected readonly greska = signal<string | null>(null);

  protected readonly kraj = computed(() => {
    const d = new Date(this.pocetak());
    d.setDate(d.getDate() + 6);
    return d;
  });

  protected readonly jeTekuca = computed(
    () => this.uIso(this.pocetak()) === this.uIso(this.ponedeljak(new Date())),
  );

  protected readonly ukupnoSati = computed(() =>
    this.smene().reduce((zbir, s) => zbir + s.trajanjeSati, 0),
  );

  protected readonly dani = computed<DanRasporeda[]>(() => {
    const danas = this.uIso(new Date());

    return Array.from({ length: 7 }, (_, i) => {
      const d = new Date(this.pocetak());
      d.setDate(d.getDate() + i);
      const iso = this.uIso(d);

      return {
        datum: iso,
        naziv: DANI[d.getDay()],
        jeDanas: iso === danas,
        smene: this.smene().filter((s) => s.datum === iso),
      };
    });
  });

  constructor() {
    this.ucitaj();
  }

  protected prethodna(): void {
    this.pomeri(-7);
  }

  protected sledeca(): void {
    this.pomeri(7);
  }

  protected naTekucu(): void {
    this.pocetak.set(this.ponedeljak(new Date()));
    this.ucitaj();
  }

  protected ucitaj(): void {
    this.ucitava.set(true);
    this.greska.set(null);

    this.servis.mojRaspored(this.uIso(this.pocetak())).subscribe({
      next: (s) => {
        this.smene.set(s);
        this.ucitava.set(false);
      },
      error: (g) => {
        this.greska.set(porukaGreske(g, 'Raspored nije učitan.'));
        this.ucitava.set(false);
      },
    });
  }

  private pomeri(dana: number): void {
    const d = new Date(this.pocetak());
    d.setDate(d.getDate() + dana);
    this.pocetak.set(d);
    this.ucitaj();
  }

  private ponedeljak(datum: Date): Date {
    const d = new Date(datum);
    d.setHours(0, 0, 0, 0);
    const pomak = d.getDay() === 0 ? -6 : 1 - d.getDay();
    d.setDate(d.getDate() + pomak);
    return d;
  }

  private uIso(datum: Date): string {
    const p = (n: number) => String(n).padStart(2, '0');
    return `${datum.getFullYear()}-${p(datum.getMonth() + 1)}-${p(datum.getDate())}`;
  }
}
