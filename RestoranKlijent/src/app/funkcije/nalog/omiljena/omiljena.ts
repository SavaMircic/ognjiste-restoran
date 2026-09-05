import { ChangeDetectionStrategy, Component, computed, inject, signal } from '@angular/core';
import { RouterLink } from '@angular/router';

import { porukaGreske } from '../../../core/greske/poruke-gresaka';
import { StavkaMenija } from '../../../core/modeli/api.modeli';
import { LajkoviServis } from '../../../core/servisi/lajkovi.servis';
import { ObavestenjaServis } from '../../../core/servisi/obavestenja.servis';
import { ProfilServis } from '../../../core/servisi/profil.servis';
import { KarticaJela } from '../../zajednicko/kartica-jela/kartica-jela';

@Component({
  selector: 'og-omiljena',
  imports: [RouterLink, KarticaJela],
  changeDetection: ChangeDetectionStrategy.OnPush,
  templateUrl: './omiljena.html',
  styleUrl: './omiljena.scss',
})
export class Omiljena {
  private readonly servis = inject(ProfilServis);
  private readonly lajkovi = inject(LajkoviServis);
  private readonly obavestenja = inject(ObavestenjaServis);

  protected readonly jela = signal<StavkaMenija[]>([]);
  protected readonly ucitava = signal(true);
  protected readonly greska = signal<string | null>(null);
  protected readonly uklanjaId = signal<number | null>(null);

  protected readonly prazno = computed(() => !this.ucitava() && !this.greska() && this.jela().length === 0);

  constructor() {
    this.ucitaj();
  }

  protected ucitaj(): void {
    this.ucitava.set(true);
    this.greska.set(null);

    this.servis.omiljenaJela().subscribe({
      next: (j) => {
        this.jela.set(j);
        this.ucitava.set(false);
      },
      error: (g) => {
        this.greska.set(porukaGreske(g, 'Omiljena jela nisu učitana.'));
        this.ucitava.set(false);
      },
    });
  }

  protected ukloni(jelo: StavkaMenija): void {
    this.uklanjaId.set(jelo.id);

    this.lajkovi.ukloni(jelo.id).subscribe({
      next: () => {
        this.uklanjaId.set(null);
        this.jela.update((sve) => sve.filter((j) => j.id !== jelo.id));
        this.obavestenja.uspeh(`„${jelo.naziv}" više nije među omiljenima.`);
      },
      error: (g) => {
        this.uklanjaId.set(null);
        this.obavestenja.greska(g, 'Uklanjanje nije uspelo.');
      },
    });
  }
}
