import { ChangeDetectionStrategy, Component, computed, inject, signal } from '@angular/core';
import { takeUntilDestroyed, toObservable } from '@angular/core/rxjs-interop';
import { debounceTime, distinctUntilChanged, skip } from 'rxjs';

import { porukaGreske } from '../../../core/greske/poruke-gresaka';
import { AdminAkcija, AuditLog } from '../../../core/modeli/api.modeli';
import { DatumCev } from '../../../core/pipes/datum.pipe';
import { AdminSadrzajServis } from '../../../core/servisi/admin-sadrzaj.servis';
import { uUtcIso } from '../../../core/tekst/vreme';
import { Paginacija } from '../../zajednicko/paginacija/paginacija';

const PO_STRANI = 25;

type Prikaz = 'trag' | 'mere';

@Component({
  selector: 'og-admin-dnevnik',
  imports: [DatumCev, Paginacija],
  changeDetection: ChangeDetectionStrategy.OnPush,
  templateUrl: './dnevnik.html',
  styleUrl: './dnevnik.scss',
})
export class AdminDnevnik {
  private readonly servis = inject(AdminSadrzajServis);

  protected readonly prikaz = signal<Prikaz>('trag');

  protected readonly pretraga = signal('');
  protected readonly slucaj = signal('');
  protected readonly tipAkcije = signal('');
  protected readonly datumOd = signal('');
  protected readonly datumDo = signal('');
  protected readonly strana = signal(1);

  protected readonly trag = signal<AuditLog[]>([]);
  protected readonly mere = signal<AdminAkcija[]>([]);
  protected readonly ukupnoStrana = signal(1);
  protected readonly ukupnoZapisa = signal(0);
  protected readonly ucitava = signal(true);
  protected readonly greska = signal<string | null>(null);

  protected readonly tipoviAkcija = [
    { vrednost: 'Blokiranje', naziv: 'Blokiranje' },
    { vrednost: 'Odblokiranje', naziv: 'Odblokiranje' },
    { vrednost: 'ZabranaKomentarisanja', naziv: 'Zabrana komentarisanja' },
    { vrednost: 'UkidanjeZabraneKomentarisanja', naziv: 'Ukidanje zabrane' },
    { vrednost: 'BrisanjeRecenzije', naziv: 'Uklanjanje recenzije' },
  ];

  protected readonly prazno = computed(() => {
    if (this.ucitava() || this.greska()) return false;
    return this.prikaz() === 'trag' ? this.trag().length === 0 : this.mere().length === 0;
  });

  protected readonly neuspesnih = computed(() => this.trag().filter((t) => !t.uspesnoIzvrseno).length);

  constructor() {
    toObservable(this.pretraga)
      .pipe(skip(1), debounceTime(300), distinctUntilChanged(), takeUntilDestroyed())
      .subscribe(() => {
        this.strana.set(1);
        this.ucitaj();
      });

    toObservable(this.slucaj)
      .pipe(skip(1), debounceTime(300), distinctUntilChanged(), takeUntilDestroyed())
      .subscribe(() => {
        this.strana.set(1);
        this.ucitaj();
      });

    this.ucitaj();
  }

  protected ucitaj(): void {
    this.ucitava.set(true);
    this.greska.set(null);

    const zajednicko = {
      pretraga: this.pretraga().trim() || null,
      datumOd: this.datumOd() ? uUtcIso(`${this.datumOd()}T00:00`) : null,
      datumDo: this.datumDo() ? uUtcIso(`${this.datumDo()}T23:59`) : null,
      strana: this.strana(),
      velicinaStrane: PO_STRANI,
    };

    if (this.prikaz() === 'trag') {
      this.servis
        .auditLog({ ...zajednicko, nazivSlucajaKoriscenja: this.slucaj().trim() || null })
        .subscribe({
          next: (o) => {
            this.trag.set(o.podaci);
            this.ukupnoStrana.set(Math.max(1, o.ukupnoStrana));
            this.ukupnoZapisa.set(o.ukupnoZapisa);
            this.ucitava.set(false);
          },
          error: (g) => {
            this.greska.set(porukaGreske(g, 'Spisak aktivnosti nije učitan.'));
            this.ucitava.set(false);
          },
        });
      return;
    }

    this.servis.adminAkcije({ ...zajednicko, tipAkcije: this.tipAkcije() || null }).subscribe({
      next: (o) => {
        this.mere.set(o.podaci);
        this.ukupnoStrana.set(Math.max(1, o.ukupnoStrana));
        this.ukupnoZapisa.set(o.ukupnoZapisa);
        this.ucitava.set(false);
      },
      error: (g) => {
        this.greska.set(porukaGreske(g, 'Spisak mera nije učitan.'));
        this.ucitava.set(false);
      },
    });
  }

  protected prebaci(prikaz: Prikaz): void {
    if (this.prikaz() === prikaz) return;

    this.prikaz.set(prikaz);
    this.slucaj.set('');
    this.tipAkcije.set('');
    this.strana.set(1);
    this.ucitaj();
  }

  protected postaviTip(vrednost: string): void {
    this.tipAkcije.set(vrednost);
    this.strana.set(1);
    this.ucitaj();
  }

  protected postaviDatum(koji: 'od' | 'do', vrednost: string): void {
    if (koji === 'od') this.datumOd.set(vrednost);
    else this.datumDo.set(vrednost);

    this.strana.set(1);
    this.ucitaj();
  }

  protected naStranu(strana: number): void {
    this.strana.set(strana);
    this.ucitaj();
  }

  protected nazivTipa(tip: string): string {
    return this.tipoviAkcija.find((t) => t.vrednost === tip)?.naziv ?? tip;
  }
}
