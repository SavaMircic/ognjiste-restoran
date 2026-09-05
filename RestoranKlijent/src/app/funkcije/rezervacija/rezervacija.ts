import { DatePipe } from '@angular/common';
import { ChangeDetectionStrategy, Component, computed, inject, signal } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { RouterLink } from '@angular/router';

import { DatumCev } from '../../core/pipes/datum.pipe';
import { porukaGreske } from '../../core/greske/poruke-gresaka';
import { PredlogSpajanja, Rezervacija as RezervacijaModel, Sto } from '../../core/modeli/api.modeli';
import { ObavestenjaServis } from '../../core/servisi/obavestenja.servis';
import { RezervacijeServis } from '../../core/servisi/rezervacije.servis';
import { saBrojem } from '../../core/tekst/mnozina';
import { uUtcIso, zaSatVremena } from '../../core/tekst/vreme';

type Korak = 'unos' | 'izborStola' | 'gotovo';

@Component({
  selector: 'og-rezervacija',
  imports: [ReactiveFormsModule, RouterLink, DatePipe, DatumCev],
  changeDetection: ChangeDetectionStrategy.OnPush,
  templateUrl: './rezervacija.html',
  styleUrl: './rezervacija.scss',
})
export class Rezervacija {
  private readonly fb = inject(FormBuilder);
  private readonly servis = inject(RezervacijeServis);
  private readonly obavestenja = inject(ObavestenjaServis);

  protected readonly korak = signal<Korak>('unos');
  protected readonly radi = signal(false);
  protected readonly greska = signal<string | null>(null);

  protected readonly stolovi = signal<Sto[]>([]);
  protected readonly predlozi = signal<PredlogSpajanja[]>([]);
  protected readonly maksGostiju = signal(20);
  protected readonly telefon = signal<string | null>(null);

  protected readonly izabraniStolovi = signal<number[]>([]);
  protected readonly potvrdjena = signal<RezervacijaModel | null>(null);

  protected readonly najranije = signal(zaSatVremena());

  protected readonly forma = this.fb.nonNullable.group({
    datumVreme: ['', Validators.required],
    brojGostiju: [2, [Validators.required, Validators.min(1), Validators.max(20)]],
  });

  protected readonly nemaSlobodnih = computed(
    () => this.korak() === 'izborStola' && this.stolovi().length === 0 && this.predlozi().length === 0,
  );

  protected readonly prekoGranice = computed(
    () => this.vrednosti().brojGostiju > this.maksGostiju(),
  );

  private readonly vrednosti = signal(this.forma.getRawValue());

  protected readonly trazeniTermin = computed(() => this.vrednosti().datumVreme);

  protected readonly brojGostijuTekst = computed(() =>
    saBrojem(this.vrednosti().brojGostiju, 'gosta', 'gosta', 'gostiju'),
  );

  constructor() {
    this.forma.valueChanges
      .pipe(takeUntilDestroyed())
      .subscribe(() => this.vrednosti.set(this.forma.getRawValue()));
  }

  protected mesta(kapacitet: number): string {
    return saBrojem(kapacitet, 'mesto', 'mesta', 'mesta');
  }

  protected proveri(): void {
    this.greska.set(null);

    if (this.forma.invalid) {
      this.forma.markAllAsTouched();
      return;
    }

    const v = this.forma.getRawValue();
    this.radi.set(true);
    this.izabraniStolovi.set([]);

    this.servis.dostupnost(uUtcIso(v.datumVreme), v.brojGostiju).subscribe({
      next: (d) => {
        this.stolovi.set(d.stolovi);
        this.predlozi.set(d.predlozi);
        this.maksGostiju.set(d.maksGostijuOnline);
        this.telefon.set(d.telefon);
        this.korak.set('izborStola');
        this.radi.set(false);
      },
      error: (g) => {
        this.radi.set(false);
        this.greska.set(porukaGreske(g, 'Provera dostupnosti nije uspela.'));
      },
    });
  }

  protected izaberi(stoId: number): void {
    this.izabraniStolovi.set([stoId]);
  }

  protected izaberiPredlog(predlog: PredlogSpajanja): void {
    this.izabraniStolovi.set(predlog.stolovi.map((s) => s.id));
  }

  protected idovi(p: PredlogSpajanja): number[] {
    return p.stolovi.map((s) => s.id);
  }

  protected brojeviStolova(p: PredlogSpajanja): string {
    return p.stolovi.map((s) => `Sto ${s.brojStola}`).join(' + ');
  }

  protected jeIzabran(idovi: number[]): boolean {
    const izabrani = this.izabraniStolovi();
    return izabrani.length === idovi.length && idovi.every((id) => izabrani.includes(id));
  }

  protected oznakaStolova(brojevi: number[]): string {
    return brojevi.length === 1 ? `sto ${brojevi[0]}` : `stolovi ${brojevi.join(' i ')}`;
  }

  protected nazad(): void {
    this.korak.set('unos');
    this.greska.set(null);
    this.najranije.set(zaSatVremena());
  }

  protected rezervisi(): void {
    const stoIds = this.izabraniStolovi();
    if (stoIds.length === 0) return;

    this.greska.set(null);
    this.radi.set(true);

    const v = this.forma.getRawValue();

    this.servis
      .kreiraj({ stoIds, datumVreme: uUtcIso(v.datumVreme), brojGostiju: v.brojGostiju })
      .subscribe({
        next: (r) => {
          this.radi.set(false);
          this.potvrdjena.set(r);
          this.korak.set('gotovo');
          this.obavestenja.uspeh(`Rezervacija potvrđena, kod ${r.kodRezervacije}.`);
        },
        error: (g) => {
          this.radi.set(false);
          this.greska.set(porukaGreske(g, 'Rezervacija nije napravljena.'));
          this.proveri();
        },
      });
  }

  protected novaRezervacija(): void {
    this.forma.reset({ brojGostiju: 2, datumVreme: '' });
    this.stolovi.set([]);
    this.predlozi.set([]);
    this.izabraniStolovi.set([]);
    this.potvrdjena.set(null);
    this.greska.set(null);
    this.najranije.set(zaSatVremena());
    this.korak.set('unos');
  }
}
