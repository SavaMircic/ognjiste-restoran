import { DecimalPipe } from '@angular/common';
import { ChangeDetectionStrategy, Component, computed, effect, inject, input, signal } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { RouterLink } from '@angular/router';
import { forkJoin } from 'rxjs';

import { porukaGreske } from '../../../../core/greske/poruke-gresaka';
import { Namirnica, ReceptStavka, StavkaMenija } from '../../../../core/modeli/api.modeli';
import { MeniServis } from '../../../../core/servisi/meni.servis';
import { ObavestenjaServis } from '../../../../core/servisi/obavestenja.servis';
import { PotvrdaServis } from '../../../../core/servisi/potvrda.servis';
import { ZaliheServis } from '../../../../core/servisi/zalihe.servis';

@Component({
  selector: 'og-admin-receptura',
  imports: [RouterLink, ReactiveFormsModule, DecimalPipe],
  changeDetection: ChangeDetectionStrategy.OnPush,
  templateUrl: './receptura.html',
  styleUrl: './receptura.scss',
})
export class AdminReceptura {
  private readonly fb = inject(FormBuilder);
  private readonly potvrda = inject(PotvrdaServis);
  private readonly meniServis = inject(MeniServis);
  private readonly servis = inject(ZaliheServis);
  private readonly obavestenja = inject(ObavestenjaServis);

  readonly id = input.required<string>();

  protected readonly jelo = signal<StavkaMenija | null>(null);
  protected readonly receptura = signal<ReceptStavka[]>([]);
  protected readonly namirnice = signal<Namirnica[]>([]);
  protected readonly ucitava = signal(true);
  protected readonly greska = signal<string | null>(null);

  protected readonly salje = signal(false);
  protected readonly menjam = signal<number | null>(null);
  protected readonly novaKolicina = signal(0);
  protected readonly uToku = signal<number | null>(null);

  protected readonly dostupne = computed(() => {
    const vec = new Set(this.receptura().map((r) => r.namirnicaId));
    return this.namirnice().filter((n) => !vec.has(n.id));
  });

  protected readonly forma = this.fb.nonNullable.group({
    namirnicaId: [0, [Validators.required, Validators.min(1)]],
    kolicina: [0, [Validators.required, Validators.min(0.001)]],
  });

  constructor() {
    effect(() => this.ucitaj(Number(this.id())));
  }

  protected ucitaj(id: number): void {
    this.ucitava.set(true);
    this.greska.set(null);

    forkJoin({
      jelo: this.meniServis.stavka(id),
      receptura: this.servis.receptura(id),
      namirnice: this.servis.namirnice(),
    }).subscribe({
      next: ({ jelo, receptura, namirnice }) => {
        this.jelo.set(jelo);
        this.receptura.set(
          [...receptura].sort((a, b) => a.namirnicaNaziv.localeCompare(b.namirnicaNaziv, 'sr')),
        );
        this.namirnice.set([...namirnice].sort((a, b) => a.naziv.localeCompare(b.naziv, 'sr')));
        this.ucitava.set(false);
      },
      error: (g) => {
        this.greska.set(porukaGreske(g, 'Receptura nije učitana.'));
        this.ucitava.set(false);
      },
    });
  }

  protected jedinicaIzabrane(): string {
    const id = Number(this.forma.controls.namirnicaId.value);
    return this.namirnice().find((n) => n.id === id)?.jedinicaMere ?? '';
  }

  protected dodaj(): void {
    if (this.forma.invalid) {
      this.forma.markAllAsTouched();
      return;
    }

    const v = this.forma.getRawValue();
    this.salje.set(true);

    this.servis
      .dodajURecepturu(Number(this.id()), {
        namirnicaId: Number(v.namirnicaId),
        kolicina: Number(v.kolicina),
      })
      .subscribe({
        next: () => {
          this.salje.set(false);
          this.forma.reset({ namirnicaId: 0, kolicina: 0 });
          this.obavestenja.uspeh('Namirnica je dodata u recepturu.');
          this.ucitaj(Number(this.id()));
        },
        error: (g) => {
          this.salje.set(false);
          this.obavestenja.greska(g, 'Namirnica nije dodata.');
        },
      });
  }

  protected zapocniIzmenu(r: ReceptStavka): void {
    this.menjam.set(r.namirnicaId);
    this.novaKolicina.set(r.kolicina);
  }

  protected odustani(): void {
    this.menjam.set(null);
  }

  protected sacuvajKolicinu(r: ReceptStavka): void {
    const kolicina = Number(this.novaKolicina());
    if (!kolicina || kolicina <= 0) {
      this.obavestenja.info('Količina mora biti veća od nule.');
      return;
    }

    this.uToku.set(r.namirnicaId);

    this.servis
      .izmeniKolicinuURecepturi(Number(this.id()), r.namirnicaId, { kolicina })
      .subscribe({
        next: () => {
          this.uToku.set(null);
          this.menjam.set(null);
          this.receptura.update((sve) =>
            sve.map((x) => (x.namirnicaId === r.namirnicaId ? { ...x, kolicina } : x)),
          );
        },
        error: (g) => {
          this.uToku.set(null);
          this.obavestenja.greska(g, 'Količina nije izmenjena.');
        },
      });
  }

  protected async ukloni(r: ReceptStavka): Promise<void> {
    const potvrdjeno = await this.potvrda.pitaj({
      naslov: 'Uklanjanje namirnice',
      tekst: `„${r.namirnicaNaziv}" se uklanja iz recepture ovog jela.`,
      potvrdi: 'Ukloni',
    });
    if (!potvrdjeno) return;

    this.uToku.set(r.namirnicaId);

    this.servis.ukloniIzRecepture(Number(this.id()), r.namirnicaId).subscribe({
      next: () => {
        this.uToku.set(null);
        this.receptura.update((sve) => sve.filter((x) => x.namirnicaId !== r.namirnicaId));
        this.obavestenja.uspeh('Namirnica je uklonjena iz recepture.');
      },
      error: (g) => {
        this.uToku.set(null);
        this.obavestenja.greska(g, 'Namirnica nije uklonjena.');
      },
    });
  }
}
