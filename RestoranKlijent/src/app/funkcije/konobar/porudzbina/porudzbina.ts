import { DecimalPipe } from '@angular/common';
import { ChangeDetectionStrategy, Component, computed, effect, inject, input, signal } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { RouterLink } from '@angular/router';
import { forkJoin } from 'rxjs';

import { porukaGreske } from '../../../core/greske/poruke-gresaka';
import {
  KategorijaMenija,
  NacinPlacanja,
  PorudzbinaDetalj,
  StavkaMenija,
  StavkaPorudzbine,
  StavkaZaDodavanje,
} from '../../../core/modeli/api.modeli';
import { DatumCev } from '../../../core/pipes/datum.pipe';
import { KuhinjaVezaServis } from '../../../core/servisi/kuhinja-veza.servis';
import { MeniServis } from '../../../core/servisi/meni.servis';
import { ObavestenjaServis } from '../../../core/servisi/obavestenja.servis';
import { PorudzbineServis } from '../../../core/servisi/porudzbine.servis';

interface RedKorpe extends StavkaZaDodavanje {
  naziv: string;
  cena: number;
}

const VELICINA_STRANE = 100;

@Component({
  selector: 'og-konobar-porudzbina',
  imports: [RouterLink, DecimalPipe, DatumCev],
  changeDetection: ChangeDetectionStrategy.OnPush,
  templateUrl: './porudzbina.html',
  styleUrl: './porudzbina.scss',
})
export class KonobarPorudzbina {
  private readonly servis = inject(PorudzbineServis);
  private readonly meniServis = inject(MeniServis);
  private readonly obavestenja = inject(ObavestenjaServis);
  private readonly veza = inject(KuhinjaVezaServis);

  readonly id = input.required<string>();

  protected readonly porudzbina = signal<PorudzbinaDetalj | null>(null);
  protected readonly ucitava = signal(true);
  protected readonly greska = signal<string | null>(null);

  protected readonly kategorije = signal<KategorijaMenija[]>([]);
  protected readonly jela = signal<StavkaMenija[]>([]);
  protected readonly meniUcitava = signal(false);
  protected readonly izabranaKategorija = signal<number | null>(null);
  protected readonly pretraga = signal('');
  protected readonly korpa = signal<RedKorpe[]>([]);
  protected readonly salje = signal(false);

  protected readonly zatvara = signal(false);
  protected readonly nacinPlacanja = signal<NacinPlacanja>('Gotovina');
  protected readonly napojnica = signal<number | null>(null);

  protected readonly uToku = signal<number | null>(null);

  protected readonly zatvorena = computed(() => this.porudzbina()?.status === 'Zatvorena');

  protected readonly poslate = computed(() => this.poStatusu('Poslato'));
  protected readonly uPripremi = computed(() => this.poStatusu('UPripremi'));

  protected readonly zaOdnosenje = computed(() =>
    this.poStatusu('Spremno').filter((s) => !this.jeIsporucena(s.id)),
  );
  protected readonly odnete = computed(() =>
    this.poStatusu('Spremno').filter((s) => this.jeIsporucena(s.id)),
  );

  protected readonly ukupno = computed(() =>
    (this.porudzbina()?.stavke ?? []).reduce(
      (zbir, s) => zbir + s.cenaUTrenutkuNarudzbine * s.kolicina,
      0,
    ),
  );

  protected readonly ukupnoKorpa = computed(() =>
    this.korpa().reduce((zbir, r) => zbir + r.cena * r.kolicina, 0),
  );

  protected readonly moguceZatvoriti = computed(
    () => !this.poslate().length && !this.uPripremi().length,
  );

  protected readonly filtriranaJela = computed(() => {
    const tekst = this.pretraga().trim().toLowerCase();
    if (tekst) return this.jela().filter((j) => j.naziv.toLowerCase().includes(tekst));

    const kategorija = this.izabranaKategorija();
    return kategorija === null
      ? this.jela()
      : this.jela().filter((j) => j.kategorijaId === kategorija);
  });

  protected readonly naciniPlacanja: NacinPlacanja[] = ['Gotovina', 'Kartica'];

  constructor() {
    effect(() => this.ucitaj(Number(this.id())));

    this.ucitajMeni();

    this.veza.stavkaSpremna.pipe(takeUntilDestroyed()).subscribe((s) => {
      if (s.brojStola === this.porudzbina()?.brojStola) this.ucitaj(Number(this.id()), true);
    });
  }

  protected ucitaj(id: number, tiho = false): void {
    if (!tiho) {
      this.ucitava.set(true);
      this.greska.set(null);
    }

    this.servis.detalj(id).subscribe({
      next: (p) => {
        this.porudzbina.set(p);
        this.ucitava.set(false);
      },
      error: (g) => {
        if (!tiho) this.greska.set(porukaGreske(g, 'Porudžbina nije učitana.'));
        this.ucitava.set(false);
      },
    });
  }

  protected dodajUKorpu(jelo: StavkaMenija): void {
    this.korpa.update((redovi) => {
      const postojeci = redovi.find((r) => r.stavkaMenijaId === jelo.id && !r.napomena);
      if (postojeci) {
        return redovi.map((r) => (r === postojeci ? { ...r, kolicina: r.kolicina + 1 } : r));
      }

      return [
        ...redovi,
        {
          stavkaMenijaId: jelo.id,
          kolicina: 1,
          napomena: null,
          naziv: jelo.naziv,
          cena: jelo.cenaSaPopustom,
        },
      ];
    });
  }

  protected promeniKolicinu(indeks: number, za: number): void {
    this.korpa.update((redovi) =>
      redovi
        .map((r, i) => (i === indeks ? { ...r, kolicina: r.kolicina + za } : r))
        .filter((r) => r.kolicina > 0),
    );
  }

  protected postaviNapomenu(indeks: number, tekst: string): void {
    this.korpa.update((redovi) =>
      redovi.map((r, i) => (i === indeks ? { ...r, napomena: tekst.trim() || null } : r)),
    );
  }

  protected izbaci(indeks: number): void {
    this.korpa.update((redovi) => redovi.filter((_, i) => i !== indeks));
  }

  protected posalji(): void {
    const redovi = this.korpa();
    if (!redovi.length || this.salje()) return;

    this.salje.set(true);

    const stavke: StavkaZaDodavanje[] = redovi.map((r) => ({
      stavkaMenijaId: r.stavkaMenijaId,
      kolicina: r.kolicina,
      napomena: r.napomena,
    }));

    this.servis.dodajStavke(Number(this.id()), { stavke }).subscribe({
      next: () => {
        this.salje.set(false);
        this.korpa.set([]);
        this.obavestenja.uspeh('Poslato u kuhinju i na šank.');
        this.ucitaj(Number(this.id()), true);
      },
      error: (g) => {
        this.salje.set(false);
        this.obavestenja.greska(g, 'Stavke nisu poslate.');
        this.ucitaj(Number(this.id()), true);
      },
    });
  }

  protected otkazi(stavka: StavkaPorudzbine): void {
    if (this.uToku()) return;
    this.uToku.set(stavka.id);

    this.servis.otkaziStavku(Number(this.id()), stavka.id).subscribe({
      next: () => {
        this.uToku.set(null);
        this.obavestenja.uspeh(`„${stavka.nazivStavke}" je otkazano.`);
        this.ucitaj(Number(this.id()), true);
      },
      error: (g) => {
        this.uToku.set(null);
        this.obavestenja.greska(g, 'Stavka nije otkazana.');
        this.ucitaj(Number(this.id()), true);
      },
    });
  }

  protected isporuci(stavka: StavkaPorudzbine): void {
    if (this.uToku()) return;
    this.uToku.set(stavka.id);

    this.servis.isporuciStavku(Number(this.id()), stavka.id).subscribe({
      next: () => this.uToku.set(null),
      error: (g) => {
        this.uToku.set(null);
        this.obavestenja.greska(g, 'Isporuka nije zabeležena.');
      },
    });
  }

  protected jeIsporucena(stavkaId: number): boolean {
    return this.servis.jeIsporucena(Number(this.id()), stavkaId);
  }

  protected zatvori(): void {
    if (this.zatvara()) return;
    this.zatvara.set(true);

    const iznos = this.napojnica();

    this.servis
      .zatvori(Number(this.id()), {
        nacinPlacanja: this.nacinPlacanja(),
        iznosNapojnice: iznos && iznos > 0 ? iznos : null,
      })
      .subscribe({
        next: () => {
          this.zatvara.set(false);
          this.obavestenja.uspeh('Sto je zatvoren.');
          this.ucitaj(Number(this.id()), true);
        },
        error: (g) => {
          this.zatvara.set(false);
          this.obavestenja.greska(g, 'Sto nije zatvoren.');
        },
      });
  }

  protected preuzmiRacun(): void {
    const id = Number(this.id());

    this.servis.racun(id).subscribe({
      next: (blob) => {
        const url = URL.createObjectURL(blob);
        const veza = document.createElement('a');
        veza.href = url;
        veza.download = `racun-${id}.txt`;
        veza.click();
        URL.revokeObjectURL(url);
      },
      error: (g) => this.obavestenja.greska(g, 'Račun nije preuzet.'),
    });
  }

  private ucitajMeni(): void {
    this.meniUcitava.set(true);

    forkJoin({
      kategorije: this.meniServis.kategorije(),
      stavke: this.meniServis.stavke({ strana: 1, velicinaStrane: VELICINA_STRANE }),
    }).subscribe({
      next: ({ kategorije, stavke }) => {
        this.kategorije.set(kategorije);
        this.jela.set(stavke.podaci.filter((j) => j.dostupno));
        this.meniUcitava.set(false);
      },
      error: () => {
        this.meniUcitava.set(false);
        this.obavestenja.greska(null, 'Meni nije učitan — osvežite stranicu.');
      },
    });
  }

  protected izaberiKategoriju(id: number | null): void {
    this.izabranaKategorija.set(id);
  }

  private poStatusu(status: StavkaPorudzbine['status']): StavkaPorudzbine[] {
    return (this.porudzbina()?.stavke ?? []).filter((s) => s.status === status);
  }
}
