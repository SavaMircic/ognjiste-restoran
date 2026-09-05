import { ChangeDetectionStrategy, Component, computed, inject, signal } from '@angular/core';
import { RouterLink } from '@angular/router';
import { catchError, of } from 'rxjs';

import { RezervnaSlika } from '../../../core/direktive/rezervna-slika.direktiva';
import { uOblasti } from '../../../core/galerija/oblasti';
import {
  ClanTima,
  GalerijaSlika,
  Postavke,
  Recenzija,
  StavkaMenija,
} from '../../../core/modeli/api.modeli';
import { SlikaCev } from '../../../core/pipes/slika.pipe';
import { MeniServis } from '../../../core/servisi/meni.servis';
import { RecenzijeServis } from '../../../core/servisi/recenzije.servis';
import { SadrzajServis } from '../../../core/servisi/sadrzaj.servis';
import { nazivUloge } from '../../../core/tekst/uloge';
import { saBrojem } from '../../../core/tekst/mnozina';
import { GalerijaSlajder } from '../../zajednicko/galerija-slajder/galerija-slajder';
import { KarticaJela } from '../../zajednicko/kartica-jela/kartica-jela';
import { KarticaRecenzije } from '../../zajednicko/kartica-recenzije/kartica-recenzije';

const IZDVOJENIH_IZ_TIMA = 3;

@Component({
  selector: 'og-pocetna',
  imports: [RouterLink, SlikaCev, RezervnaSlika, KarticaJela, KarticaRecenzije, GalerijaSlajder],
  changeDetection: ChangeDetectionStrategy.OnPush,
  templateUrl: './pocetna.html',
  styleUrl: './pocetna.scss',
})
export class Pocetna {
  private readonly meniServis = inject(MeniServis);
  private readonly recenzijeServis = inject(RecenzijeServis);
  private readonly sadrzajServis = inject(SadrzajServis);

  protected readonly izdvojena = signal<StavkaMenija[]>([]);
  protected readonly recenzije = signal<Recenzija[]>([]);
  protected readonly galerija = signal<GalerijaSlika[]>([]);
  protected readonly tim = signal<ClanTima[]>([]);
  protected readonly postavke = signal<Postavke | null>(null);
  protected readonly ucitavaJela = signal(true);

  protected readonly otvorenaOblast = signal<string | null>(null);

  protected readonly nazivUloge = nazivUloge;
  protected readonly saBrojem = saBrojem;

  protected readonly oblasti = computed(() => uOblasti(this.galerija()).slice(0, 6));

  protected readonly oblast = computed(() => {
    const naziv = this.otvorenaOblast();
    return naziv === null ? null : (this.oblasti().find((o) => o.naziv === naziv) ?? null);
  });

  protected readonly udvojene = computed(() => [...this.recenzije(), ...this.recenzije()]);

  constructor() {
    this.meniServis
      .stavke({ sortiranje: 'ocena', strana: 1, velicinaStrane: 6 })
      .pipe(catchError(() => of(null)))
      .subscribe((odgovor) => {
        if (odgovor) this.izdvojena.set(odgovor.podaci);
        this.ucitavaJela.set(false);
      });

    this.recenzijeServis
      .pretrazi({ sortiranje: 'najbolje-ocenjeno', strana: 1, velicinaStrane: 6 })
      .pipe(catchError(() => of(null)))
      .subscribe((odgovor) => odgovor && this.recenzije.set(odgovor.podaci));

    this.sadrzajServis
      .galerija()
      .pipe(catchError(() => of([] as GalerijaSlika[])))
      .subscribe((slike) => this.galerija.set(slike));

    this.sadrzajServis
      .nasTim()
      .pipe(catchError(() => of([] as ClanTima[])))
      .subscribe((svi) => this.tim.set(this.nasumicnih(svi, IZDVOJENIH_IZ_TIMA)));

    this.sadrzajServis
      .postavke()
      .pipe(catchError(() => of(null)))
      .subscribe((p) => p && this.postavke.set(p));
  }

  protected otvoriOblast(naziv: string): void {
    this.otvorenaOblast.set(naziv);
  }

  protected zatvoriOblast(): void {
    this.otvorenaOblast.set(null);
  }

  protected inicijali(c: ClanTima): string {
    return `${c.ime[0] ?? ''}${c.prezime[0] ?? ''}`.toUpperCase();
  }

  private nasumicnih<T>(sve: T[], koliko: number): T[] {
    const kopija = [...sve];
    for (let i = kopija.length - 1; i > 0; i--) {
      const j = Math.floor(Math.random() * (i + 1));
      [kopija[i], kopija[j]] = [kopija[j], kopija[i]];
    }
    return kopija.slice(0, koliko);
  }
}
