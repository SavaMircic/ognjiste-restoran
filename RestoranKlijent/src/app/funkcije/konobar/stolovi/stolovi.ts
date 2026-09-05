import { ChangeDetectionStrategy, Component, computed, inject, signal } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { Router } from '@angular/router';
import { forkJoin } from 'rxjs';

import { porukaGreske } from '../../../core/greske/poruke-gresaka';
import { PorudzbinaAktivna, StatusStola, Sto } from '../../../core/modeli/api.modeli';
import { KuhinjaVezaServis } from '../../../core/servisi/kuhinja-veza.servis';
import { ObavestenjaServis } from '../../../core/servisi/obavestenja.servis';
import { PorudzbineServis } from '../../../core/servisi/porudzbine.servis';

interface KartaStola {
  sto: Sto;
  porudzbina: PorudzbinaAktivna | null;
  spremno: number;
  uPripremi: number;
  poslato: number;
}

@Component({
  selector: 'og-konobar-stolovi',
  changeDetection: ChangeDetectionStrategy.OnPush,
  templateUrl: './stolovi.html',
  styleUrl: './stolovi.scss',
})
export class KonobarStolovi {
  private readonly servis = inject(PorudzbineServis);
  private readonly obavestenja = inject(ObavestenjaServis);
  private readonly veza = inject(KuhinjaVezaServis);
  private readonly router = inject(Router);

  private readonly stolovi = signal<Sto[]>([]);
  private readonly aktivne = signal<PorudzbinaAktivna[]>([]);

  protected readonly ucitava = signal(true);
  protected readonly greska = signal<string | null>(null);

  protected readonly otvaraSe = signal<number | null>(null);

  protected readonly karte = computed<KartaStola[]>(() => {
    const poBroju = new Map(this.aktivne().map((p) => [p.brojStola, p]));

    return this.stolovi().map((sto) => {
      const porudzbina = poBroju.get(sto.brojStola) ?? null;
      return {
        sto,
        porudzbina,
        spremno: porudzbina?.brojStavkiSpremno ?? 0,
        uPripremi: porudzbina?.brojStavkiUPripremi ?? 0,
        poslato: porudzbina?.brojStavkiPoslato ?? 0,
      };
    });
  });

  protected readonly brojSlobodnih = computed(
    () => this.karte().filter((k) => !k.porudzbina).length,
  );

  protected readonly ukupnoSpremno = computed(() =>
    this.karte().reduce((zbir, k) => zbir + k.spremno, 0),
  );

  protected readonly nazivStatusa: Record<StatusStola, string> = {
    Slobodan: 'Slobodan',
    Zauzet: 'Zauzet',
    Rezervisan: 'Rezervisan',
  };

  constructor() {
    this.ucitaj();

    this.veza.stavkaSpremna.pipe(takeUntilDestroyed()).subscribe(() => this.osveziAktivne());
  }

  protected ucitaj(): void {
    this.ucitava.set(true);
    this.greska.set(null);

    forkJoin({ stolovi: this.servis.stolovi(), aktivne: this.servis.aktivne() }).subscribe({
      next: ({ stolovi, aktivne }) => {
        this.stolovi.set([...stolovi].sort((a, b) => a.brojStola - b.brojStola));
        this.aktivne.set(aktivne);
        this.ucitava.set(false);
      },
      error: (g) => {
        this.greska.set(porukaGreske(g, 'Stolovi nisu učitani.'));
        this.ucitava.set(false);
      },
    });
  }

  protected otvori(karta: KartaStola): void {
    if (karta.porudzbina) {
      void this.router.navigate(['/konobar/porudzbina', karta.porudzbina.id]);
      return;
    }

    this.otvaraSe.set(karta.sto.id);

    this.servis.otvori({ stoId: karta.sto.id }).subscribe({
      next: ({ porudzbinaId }) => {
        this.otvaraSe.set(null);
        void this.router.navigate(['/konobar/porudzbina', porudzbinaId]);
      },
      error: (g) => {
        this.otvaraSe.set(null);
        this.obavestenja.greska(g, 'Sto nije otvoren.');
        this.osveziAktivne();
      },
    });
  }

  private osveziAktivne(): void {
    this.servis.aktivne().subscribe({
      next: (a) => this.aktivne.set(a),
      error: () => undefined,
    });
  }
}
