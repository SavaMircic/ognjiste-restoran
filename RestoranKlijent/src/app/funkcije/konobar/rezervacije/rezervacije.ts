import { ChangeDetectionStrategy, Component, computed, inject, signal } from '@angular/core';
import { Router } from '@angular/router';
import { catchError, forkJoin, of } from 'rxjs';

import { porukaGreske } from '../../../core/greske/poruke-gresaka';
import { PorudzbinaAktivna, Rezervacija, StatusRezervacije } from '../../../core/modeli/api.modeli';
import { DatumCev } from '../../../core/pipes/datum.pipe';
import { ObavestenjaServis } from '../../../core/servisi/obavestenja.servis';
import { PorudzbineServis } from '../../../core/servisi/porudzbine.servis';
import { RezervacijeServis } from '../../../core/servisi/rezervacije.servis';
import { saBrojem } from '../../../core/tekst/mnozina';
import { izServera } from '../../../core/tekst/vreme';

interface RedRezervacije {
  r: Rezervacija;
  kasni: boolean;
  stoOtvoren: boolean;
  tudjRacun: boolean;
  porudzbinaId: number | null;
}

interface OtvorenRacun {
  porudzbinaId: number;
  rezervacijaId: number | null;
}

@Component({
  selector: 'og-konobar-rezervacije',
  imports: [DatumCev],
  changeDetection: ChangeDetectionStrategy.OnPush,
  templateUrl: './rezervacije.html',
  styleUrl: './rezervacije.scss',
})
export class RezervacijeDanas {
  private readonly servis = inject(RezervacijeServis);
  private readonly porudzbine = inject(PorudzbineServis);
  private readonly obavestenja = inject(ObavestenjaServis);
  private readonly router = inject(Router);

  private readonly rezervacije = signal<Rezervacija[]>([]);

  private readonly otvoreniStolovi = signal<ReadonlyMap<number, OtvorenRacun>>(new Map());

  private readonly sada = signal(Date.now());

  protected readonly ucitava = signal(true);
  protected readonly greska = signal<string | null>(null);
  protected readonly uToku = signal<number | null>(null);

  protected readonly redovi = computed<RedRezervacije[]>(() => {
    const sada = this.sada();

    const otvoreni = this.otvoreniStolovi();

    return this.rezervacije().map((r) => {
      const tolerancija = izServera(r.tolerancijaDo);
      const racun = otvoreni.get(r.brojStola) ?? null;

      return {
        r,
        kasni: r.status === 'Aktivna' && !!tolerancija && tolerancija.getTime() < sada,
        stoOtvoren: racun !== null,
        tudjRacun: racun !== null && racun.rezervacijaId !== null && racun.rezervacijaId !== r.id,
        porudzbinaId: racun?.porudzbinaId ?? null,
      };
    });
  });

  protected readonly ocekivani = computed(() => this.redovi().filter((x) => x.r.status === 'Aktivna'));
  protected readonly obradjeni = computed(() => this.redovi().filter((x) => x.r.status !== 'Aktivna'));

  protected readonly prazno = computed(
    () => !this.ucitava() && !this.greska() && this.rezervacije().length === 0,
  );

  protected readonly nazivStatusa: Record<StatusRezervacije, string> = {
    Aktivna: 'Očekuje se',
    Realizovana: 'Gost je stigao',
    Istekla: 'Nije došao',
    Otkazana: 'Otkazana',
  };

  protected readonly klasaStatusa: Record<StatusRezervacije, string> = {
    Aktivna: 'nova',
    Realizovana: 'resena',
    Istekla: 'odbijena',
    Otkazana: '',
  };

  constructor() {
    this.ucitaj();
  }

  protected gosti(broj: number): string {
    return saBrojem(broj, 'gost', 'gosta', 'gostiju');
  }

  protected ucitaj(): void {
    this.ucitava.set(true);
    this.greska.set(null);

    forkJoin({
      rezervacije: this.servis.zaDanas(),
      aktivne: this.porudzbine.aktivne().pipe(catchError(() => of([] as PorudzbinaAktivna[]))),
    }).subscribe({
      next: ({ rezervacije, aktivne }) => {
        this.rezervacije.set(rezervacije);
        this.otvoreniStolovi.set(
          new Map(
            aktivne.map((p) => [p.brojStola, { porudzbinaId: p.id, rezervacijaId: p.rezervacijaId }]),
          ),
        );
        this.sada.set(Date.now());
        this.ucitava.set(false);
      },
      error: (g) => {
        this.greska.set(porukaGreske(g, 'Rezervacije nisu učitane.'));
        this.ucitava.set(false);
      },
    });
  }

  protected prijaviDolazak(r: Rezervacija): void {
    if (this.uToku()) return;
    this.uToku.set(r.id);

    this.servis.prijaviDolazak(r.id).subscribe({
      next: () => {
        this.uToku.set(null);
        this.obavestenja.uspeh(
          this.otvoreniStolovi().has(r.brojStola)
            ? `${r.imeZaPrikaz} je upisan na već otvoren sto ${r.brojStola}.`
            : `Sto ${r.brojStola} je otvoren za ${r.imeZaPrikaz}.`,
        );
        this.otvoriNjenuPorudzbinu(r.brojStola);
      },
      error: (g) => {
        this.uToku.set(null);
        this.obavestenja.greska(g, 'Dolazak nije prijavljen.');
        this.ucitaj();
      },
    });
  }

  protected otvoriRacun(porudzbinaId: number): void {
    void this.router.navigate(['/konobar/porudzbina', porudzbinaId]);
  }

  protected oznaciIsteklom(r: Rezervacija): void {
    if (this.uToku()) return;
    this.uToku.set(r.id);

    this.servis.oznaciIsteklom(r.id).subscribe({
      next: () => {
        this.uToku.set(null);
        this.obavestenja.info(`Sto ${r.brojStola} je oslobođen.`);
        this.ucitaj();
      },
      error: (g) => {
        this.uToku.set(null);
        this.obavestenja.greska(g, 'Rezervacija nije označena isteklom.');
        this.ucitaj();
      },
    });
  }

  private otvoriNjenuPorudzbinu(brojStola: number): void {
    this.porudzbine.aktivne().subscribe({
      next: (aktivne) => {
        const nova = aktivne.find((p) => p.brojStola === brojStola);
        if (nova) void this.router.navigate(['/konobar/porudzbina', nova.id]);
        else this.ucitaj();
      },
      error: () => this.ucitaj(),
    });
  }
}
