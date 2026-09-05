import { ChangeDetectionStrategy, Component, computed, effect, inject, input, signal } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { RouterLink } from '@angular/router';
import { interval } from 'rxjs';

import { porukaGreske } from '../../core/greske/poruke-gresaka';
import { Odrediste, RedCekanjaStavka } from '../../core/modeli/api.modeli';
import { AuthServis } from '../../core/servisi/auth.servis';
import { KuhinjaVezaServis } from '../../core/servisi/kuhinja-veza.servis';
import { ObavestenjaServis } from '../../core/servisi/obavestenja.servis';
import { PripremaServis } from '../../core/servisi/priprema.servis';
import { izServera } from '../../core/tekst/vreme';

interface RedStavka {
  s: RedCekanjaStavka;
  minuta: number;
  hitnost: 'na-vreme' | 'pozno' | 'kasni';
}

const PRAG_POZNO = 10;
const PRAG_KASNI = 20;

const OTKUCAJ_MS = 30_000;

const NASLOVI: Record<Odrediste, { naslov: string; uloga: string }> = {
  Kuhinja: { naslov: 'Kuhinja', uloga: 'Kuvar' },
  Sank: { naslov: 'Šank', uloga: 'Šanker' },
};

@Component({
  selector: 'og-red-cekanja',
  imports: [RouterLink],
  changeDetection: ChangeDetectionStrategy.OnPush,
  templateUrl: './red-cekanja.html',
  styleUrl: './red-cekanja.scss',
})
export class RedCekanja {
  private readonly servis = inject(PripremaServis);
  private readonly obavestenja = inject(ObavestenjaServis);
  protected readonly veza = inject(KuhinjaVezaServis);
  protected readonly auth = inject(AuthServis);

  readonly odrediste = input.required<Odrediste>();

  private readonly stavke = signal<RedCekanjaStavka[]>([]);
  private readonly sada = signal(Date.now());

  protected readonly ucitava = signal(true);
  protected readonly greska = signal<string | null>(null);
  protected readonly uToku = signal<number | null>(null);

  protected readonly naslov = computed(() => NASLOVI[this.odrediste()].naslov);
  protected readonly uloga = computed(() => NASLOVI[this.odrediste()].uloga);

  private readonly redovi = computed<RedStavka[]>(() => {
    const sada = this.sada();

    return this.stavke().map((s) => {
      const poslato = izServera(s.vremeSlanja);
      const minuta = poslato ? Math.max(0, Math.floor((sada - poslato.getTime()) / 60_000)) : 0;

      return {
        s,
        minuta,
        hitnost: minuta >= PRAG_KASNI ? 'kasni' : minuta >= PRAG_POZNO ? 'pozno' : 'na-vreme',
      };
    });
  });

  protected readonly cekaju = computed(() => this.redovi().filter((x) => x.s.status === 'Poslato'));
  protected readonly uPripremi = computed(() =>
    this.redovi().filter((x) => x.s.status === 'UPripremi'),
  );

  protected readonly mojih = computed(() => this.uPripremi().filter((x) => x.s.mojaStavka).length);
  protected readonly kasne = computed(() => this.redovi().filter((x) => x.hitnost === 'kasni').length);

  protected readonly prazno = computed(
    () => !this.ucitava() && !this.greska() && this.stavke().length === 0,
  );

  constructor() {
    void this.veza.povezi();

    effect(() => this.ucitaj(this.odrediste()));

    this.veza.novaStavka.pipe(takeUntilDestroyed()).subscribe((n) => {
      this.obavestenja.info(`Sto ${n.brojStola}: ${n.kolicina}× ${n.nazivStavke}`);
      this.ucitaj(this.odrediste(), true);
    });

    interval(OTKUCAJ_MS)
      .pipe(takeUntilDestroyed())
      .subscribe(() => this.sada.set(Date.now()));
  }

  protected ucitaj(odrediste: Odrediste, tiho = false): void {
    if (!tiho) {
      this.ucitava.set(true);
      this.greska.set(null);
    }

    this.servis.redCekanja(odrediste).subscribe({
      next: (s) => {
        this.stavke.set(s);
        this.sada.set(Date.now());
        this.ucitava.set(false);
      },
      error: (g) => {
        if (!tiho) this.greska.set(porukaGreske(g, 'Red čekanja nije učitan.'));
        this.ucitava.set(false);
      },
    });
  }

  protected osvezi(): void {
    this.ucitaj(this.odrediste());
  }

  protected preuzmi(x: RedStavka): void {
    if (this.uToku()) return;
    this.uToku.set(x.s.id);

    this.servis.preuzmi(x.s.id).subscribe({
      next: () => {
        this.uToku.set(null);
        this.ucitaj(this.odrediste(), true);
      },
      error: (g) => {
        this.uToku.set(null);
        this.obavestenja.greska(g, 'Stavka nije preuzeta.');
        this.ucitaj(this.odrediste(), true);
      },
    });
  }

  protected zavrsi(x: RedStavka): void {
    if (this.uToku()) return;
    this.uToku.set(x.s.id);

    this.servis.zavrsi(x.s.id).subscribe({
      next: () => {
        this.uToku.set(null);
        this.obavestenja.uspeh(`Sto ${x.s.brojStola}: ${x.s.nazivStavke} — javljeno konobaru.`);
        this.ucitaj(this.odrediste(), true);
      },
      error: (g) => {
        this.uToku.set(null);
        this.obavestenja.greska(g, 'Stavka nije označena gotovom.');
        this.ucitaj(this.odrediste(), true);
      },
    });
  }

  protected cekanje(minuta: number): string {
    if (minuta < 1) return 'upravo stiglo';
    return `čeka ${minuta} min`;
  }
}
