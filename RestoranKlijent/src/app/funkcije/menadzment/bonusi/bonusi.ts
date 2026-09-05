import { DecimalPipe } from '@angular/common';
import { ChangeDetectionStrategy, Component, computed, inject, signal } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { catchError, of } from 'rxjs';

import { greskePoPolju, porukaGreske } from '../../../core/greske/poruke-gresaka';
import { Bonus, Zaposleni } from '../../../core/modeli/api.modeli';
import { DatumCev } from '../../../core/pipes/datum.pipe';
import { AdminKorisniciServis } from '../../../core/servisi/admin-korisnici.servis';
import { IzvestajiServis } from '../../../core/servisi/izvestaji.servis';
import { ObavestenjaServis } from '../../../core/servisi/obavestenja.servis';
import { nazivUloge } from '../../../core/tekst/uloge';
import { Paginacija } from '../../zajednicko/paginacija/paginacija';

const PO_STRANI = 20;

const MAKS_PRIMALACA = 20;

@Component({
  selector: 'og-menadzer-bonusi',
  imports: [ReactiveFormsModule, DecimalPipe, DatumCev, Paginacija],
  changeDetection: ChangeDetectionStrategy.OnPush,
  templateUrl: './bonusi.html',
  styleUrl: './bonusi.scss',
})
export class MenadzerBonusi {
  private readonly fb = inject(FormBuilder);
  private readonly servis = inject(IzvestajiServis);
  private readonly zaposleniServis = inject(AdminKorisniciServis);
  private readonly obavestenja = inject(ObavestenjaServis);

  protected readonly bonusi = signal<Bonus[]>([]);
  protected readonly zaposleni = signal<Zaposleni[]>([]);
  protected readonly ukupnoStrana = signal(1);
  protected readonly ukupnoZapisa = signal(0);
  protected readonly ucitava = signal(true);
  protected readonly greska = signal<string | null>(null);

  protected readonly filterZaposleni = signal<number | null>(null);
  protected readonly strana = signal(1);

  protected readonly dodeljujem = signal(false);
  protected readonly izabrani = signal<number[]>([]);
  protected readonly salje = signal(false);
  protected readonly greskaForme = signal<string | null>(null);

  protected readonly nazivUloge = nazivUloge;
  protected readonly maksPrimalaca = MAKS_PRIMALACA;

  protected readonly aktivnih = computed(() => this.bonusi().filter((b) => b.vazi).length);

  protected readonly prazno = computed(
    () => !this.ucitava() && !this.greska() && this.bonusi().length === 0,
  );

  protected readonly forma = this.fb.nonNullable.group({
    iznos: [0, [Validators.required, Validators.min(1)]],
    datumPocetka: [this.danas(), Validators.required],
    datumKraja: [this.zaDana(30), Validators.required],
    razlog: ['', [Validators.required, Validators.maxLength(500)]],
  });

  private readonly vrednosti = signal(this.forma.getRawValue());

  protected readonly ukupanTrosak = computed(
    () => (Number(this.vrednosti().iznos) || 0) * this.izabrani().length,
  );

  constructor() {
    this.forma.valueChanges
      .pipe(takeUntilDestroyed())
      .subscribe(() => this.vrednosti.set(this.forma.getRawValue()));

    this.zaposleniServis
      .pretraziZaposlene({ aktivan: true, velicinaStrane: 100 })
      .pipe(catchError(() => of(null)))
      .subscribe((o) => {
        if (!o) return;
        this.zaposleni.set(
          [...o.podaci].sort((a, b) => a.prezime.localeCompare(b.prezime, 'sr')),
        );
      });

    this.ucitaj();
  }

  protected ucitaj(): void {
    this.ucitava.set(true);
    this.greska.set(null);

    this.servis
      .pretraziBonuse({
        zaposleniId: this.filterZaposleni(),
        strana: this.strana(),
        velicinaStrane: PO_STRANI,
      })
      .subscribe({
        next: (o) => {
          this.bonusi.set(o.podaci);
          this.ukupnoStrana.set(Math.max(1, o.ukupnoStrana));
          this.ukupnoZapisa.set(o.ukupnoZapisa);
          this.ucitava.set(false);
        },
        error: (g) => {
          this.greska.set(porukaGreske(g, 'Bonusi nisu učitani.'));
          this.ucitava.set(false);
        },
      });
  }

  protected filtriraj(vrednost: string): void {
    this.filterZaposleni.set(vrednost ? Number(vrednost) : null);
    this.strana.set(1);
    this.ucitaj();
  }

  protected naStranu(strana: number): void {
    this.strana.set(strana);
    this.ucitaj();
  }

  protected zapocni(): void {
    this.forma.reset({
      iznos: 0,
      datumPocetka: this.danas(),
      datumKraja: this.zaDana(30),
      razlog: '',
    });
    this.izabrani.set([]);
    this.greskaForme.set(null);
    this.dodeljujem.set(true);
  }

  protected odustani(): void {
    this.dodeljujem.set(false);
    this.greskaForme.set(null);
  }

  protected prebaciPrimaoca(id: number): void {
    this.izabrani.update((sve) =>
      sve.includes(id) ? sve.filter((x) => x !== id) : [...sve, id],
    );
  }

  protected jeIzabran(id: number): boolean {
    return this.izabrani().includes(id);
  }

  protected izaberiSve(): void {
    this.izabrani.set(this.zaposleni().slice(0, MAKS_PRIMALACA).map((z) => z.id));
  }

  protected ponistiIzbor(): void {
    this.izabrani.set([]);
  }

  protected dodeli(): void {
    this.greskaForme.set(null);

    if (!this.izabrani().length) {
      this.greskaForme.set('Izaberite bar jednog zaposlenog.');
      return;
    }

    if (this.forma.invalid) {
      this.forma.markAllAsTouched();
      return;
    }

    const v = this.forma.getRawValue();
    this.salje.set(true);

    this.servis
      .dodeliBonus({
        zaposleniIds: this.izabrani(),
        iznos: Number(v.iznos),
        datumPocetka: v.datumPocetka,
        datumKraja: v.datumKraja,
        razlog: v.razlog.trim(),
      })
      .subscribe({
        next: (dodeljeni) => {
          this.salje.set(false);
          this.dodeljujem.set(false);
          this.obavestenja.uspeh(
            dodeljeni.length === 1
              ? 'Bonus je dodeljen.'
              : `Bonus je dodeljen — ${dodeljeni.length} zaposlenih.`,
          );
          this.strana.set(1);
          this.ucitaj();
        },
        error: (g) => {
          this.salje.set(false);
          this.greskaForme.set(porukaGreske(g, 'Bonus nije dodeljen.'));

          for (const [polje, poruke] of Object.entries(greskePoPolju(g))) {
            if (!polje || !poruke?.length) continue;
            const naziv = polje[0].toLowerCase() + polje.slice(1);
            const kontrola = this.forma.get(naziv);
            kontrola?.setErrors({ server: poruke[0] });
            kontrola?.markAsTouched();
          }
        },
      });
  }

  private uIso(datum: Date): string {
    const p = (n: number) => String(n).padStart(2, '0');
    return `${datum.getFullYear()}-${p(datum.getMonth() + 1)}-${p(datum.getDate())}`;
  }

  private danas(): string {
    return this.uIso(new Date());
  }

  private zaDana(dana: number): string {
    const d = new Date();
    d.setDate(d.getDate() + dana);
    return this.uIso(d);
  }
}
