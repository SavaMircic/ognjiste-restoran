import { DecimalPipe } from '@angular/common';
import { ChangeDetectionStrategy, Component, computed, inject, signal } from '@angular/core';

import { porukaGreske } from '../../../core/greske/poruke-gresaka';
import {
  GrupisanjePrihoda,
  PrihodIzvestaj,
  RedosledTopJela,
  TopJelo,
  UcinakZaposlenog,
} from '../../../core/modeli/api.modeli';
import { IzvestajiServis } from '../../../core/servisi/izvestaji.servis';

type Pogled = 'promet' | 'jela' | 'ucinak';

const IZBOR_LIMITA = [10, 20, 0];

const PODRAZUMEVANO = {
  danaUnazad: 29,
  grupisanje: 'Dan' as GrupisanjePrihoda,
  redosled: 'Najprodavanije' as RedosledTopJela,
  limit: 10,
};

@Component({
  selector: 'og-menadzer-izvestaji',
  imports: [DecimalPipe],
  changeDetection: ChangeDetectionStrategy.OnPush,
  templateUrl: './izvestaji.html',
  styleUrl: './izvestaji.scss',
})
export class MenadzerIzvestaji {
  private readonly servis = inject(IzvestajiServis);

  protected readonly pogled = signal<Pogled>('promet');

  protected readonly datumOd = signal(this.preDana(PODRAZUMEVANO.danaUnazad));
  protected readonly datumDo = signal(this.danas());

  protected readonly grupisanje = signal<GrupisanjePrihoda>(PODRAZUMEVANO.grupisanje);
  protected readonly redosled = signal<RedosledTopJela>(PODRAZUMEVANO.redosled);
  protected readonly limit = signal(PODRAZUMEVANO.limit);

  protected readonly prihod = signal<PrihodIzvestaj | null>(null);
  protected readonly jela = signal<TopJelo[]>([]);
  protected readonly ucinak = signal<UcinakZaposlenog[]>([]);
  protected readonly ucitava = signal(true);
  protected readonly greska = signal<string | null>(null);

  protected readonly grupisanja: GrupisanjePrihoda[] = [
    'Dan',
    'Nedelja',
    'Mesec',
    'Kategorija',
    'Artikal',
  ];

  protected readonly nazivGrupisanja: Record<GrupisanjePrihoda, string> = {
    Dan: 'Po danu',
    Nedelja: 'Po nedelji',
    Mesec: 'Po mesecu',
    Kategorija: 'Po kategoriji',
    Artikal: 'Po artiklu',
  };

  protected readonly limiti = IZBOR_LIMITA;

  protected readonly najveciUGrupi = computed(() =>
    Math.max(1, ...(this.prihod()?.stavke ?? []).map((s) => s.prihod)),
  );

  protected readonly prikazanaJela = computed(() =>
    this.limit() ? this.jela().slice(0, this.limit()) : this.jela(),
  );

  protected readonly konobari = computed(() => this.ucinak().filter((u) => u.brojStolova > 0));
  protected readonly priprema = computed(() =>
    this.ucinak().filter((u) => u.brojPripremljenihStavki > 0),
  );

  protected readonly promenjeno = computed(
    () =>
      this.datumOd() !== this.preDana(PODRAZUMEVANO.danaUnazad) ||
      this.datumDo() !== this.danas() ||
      this.grupisanje() !== PODRAZUMEVANO.grupisanje ||
      this.redosled() !== PODRAZUMEVANO.redosled ||
      this.limit() !== PODRAZUMEVANO.limit,
  );

  protected readonly prazno = computed(() => {
    if (this.ucitava() || this.greska()) return false;
    if (this.pogled() === 'promet') return !this.prihod()?.stavke.length;
    if (this.pogled() === 'jela') return this.jela().length === 0;
    return this.ucinak().length === 0;
  });

  constructor() {
    this.ucitaj();
  }

  protected ucitaj(): void {
    this.ucitava.set(true);
    this.greska.set(null);

    const period = { datumOd: this.datumOd(), datumDo: this.datumDo() };
    const neuspeh = (g: unknown) => {
      this.greska.set(porukaGreske(g, 'Izveštaj nije učitan.'));
      this.ucitava.set(false);
    };

    if (this.pogled() === 'promet') {
      this.servis.prihod({ ...period, grupisanjePo: this.grupisanje() }).subscribe({
        next: (r) => {
          this.prihod.set(r);
          this.ucitava.set(false);
        },
        error: neuspeh,
      });
      return;
    }

    if (this.pogled() === 'jela') {
      this.servis.topJela({ ...period, redosled: this.redosled() }).subscribe({
        next: (j) => {
          this.jela.set(j);
          this.ucitava.set(false);
        },
        error: neuspeh,
      });
      return;
    }

    this.servis.ucinakZaposlenih(period).subscribe({
      next: (u) => {
        this.ucinak.set(u);
        this.ucitava.set(false);
      },
      error: neuspeh,
    });
  }

  protected prebaci(pogled: Pogled): void {
    if (this.pogled() === pogled) return;
    this.pogled.set(pogled);
    this.ucitaj();
  }

  protected postaviDatum(koji: 'od' | 'do', vrednost: string): void {
    if (!vrednost) return;
    if (koji === 'od') this.datumOd.set(vrednost);
    else this.datumDo.set(vrednost);
    this.ucitaj();
  }

  protected raspon(dana: number): void {
    this.datumOd.set(this.preDana(dana - 1));
    this.datumDo.set(this.danas());
    this.ucitaj();
  }

  protected ponisti(): void {
    this.datumOd.set(this.preDana(PODRAZUMEVANO.danaUnazad));
    this.datumDo.set(this.danas());
    this.grupisanje.set(PODRAZUMEVANO.grupisanje);
    this.redosled.set(PODRAZUMEVANO.redosled);
    this.limit.set(PODRAZUMEVANO.limit);
    this.ucitaj();
  }

  protected postaviGrupisanje(vrednost: string): void {
    this.grupisanje.set(vrednost as GrupisanjePrihoda);
    this.ucitaj();
  }

  protected postaviRedosled(vrednost: string): void {
    this.redosled.set(vrednost as RedosledTopJela);
    this.ucitaj();
  }

  protected sirina(prihod: number): number {
    return Math.round((prihod / this.najveciUGrupi()) * 100);
  }

  private uIso(datum: Date): string {
    const p = (n: number) => String(n).padStart(2, '0');
    return `${datum.getFullYear()}-${p(datum.getMonth() + 1)}-${p(datum.getDate())}`;
  }

  private danas(): string {
    return this.uIso(new Date());
  }

  private preDana(dana: number): string {
    const d = new Date();
    d.setDate(d.getDate() - dana);
    return this.uIso(d);
  }
}
