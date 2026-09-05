import { ChangeDetectionStrategy, Component, computed, inject, signal } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';

import { greskePoPolju, porukaGreske } from '../../../core/greske/poruke-gresaka';
import {
  KategorijaProblema,
  PrijavaProblema,
  PrioritetProblema,
  StatusPrijaveProblema,
} from '../../../core/modeli/api.modeli';
import { DatumCev } from '../../../core/pipes/datum.pipe';
import { ObavestenjaServis } from '../../../core/servisi/obavestenja.servis';
import { OsobljeServis } from '../../../core/servisi/osoblje.servis';

@Component({
  selector: 'og-prijave-problema',
  imports: [ReactiveFormsModule, DatumCev],
  changeDetection: ChangeDetectionStrategy.OnPush,
  templateUrl: './prijave.html',
  styleUrl: './prijave.scss',
})
export class Prijave {
  private readonly fb = inject(FormBuilder);
  private readonly servis = inject(OsobljeServis);
  private readonly obavestenja = inject(ObavestenjaServis);

  protected readonly prijave = signal<PrijavaProblema[]>([]);
  protected readonly ucitava = signal(true);
  protected readonly greska = signal<string | null>(null);

  protected readonly pisem = signal(false);
  protected readonly salje = signal(false);
  protected readonly greskaForme = signal<string | null>(null);

  protected readonly prazno = computed(
    () => !this.ucitava() && !this.greska() && this.prijave().length === 0,
  );

  protected readonly kategorije: KategorijaProblema[] = [
    'Oprema',
    'Namirnice',
    'Higijena',
    'Softver',
    'Ostalo',
  ];

  protected readonly prioriteti: PrioritetProblema[] = ['Nizak', 'Srednji', 'Visok'];

  protected readonly nazivStatusa: Record<StatusPrijaveProblema, string> = {
    Nova: 'Nova',
    UObradi: 'U obradi',
    Resena: 'Rešena',
    Odbijena: 'Odbijena',
  };

  protected readonly klasaStatusa: Record<StatusPrijaveProblema, string> = {
    Nova: 'nova',
    UObradi: 'u-obradi',
    Resena: 'resena',
    Odbijena: 'odbijena',
  };

  protected readonly forma = this.fb.nonNullable.group({
    naslov: ['', [Validators.required, Validators.maxLength(150)]],
    opis: ['', [Validators.required, Validators.maxLength(2000)]],
    kategorija: ['Oprema' as KategorijaProblema, Validators.required],
    prioritet: ['Srednji' as PrioritetProblema, Validators.required],
  });

  constructor() {
    this.ucitaj();
  }

  protected ucitaj(): void {
    this.ucitava.set(true);
    this.greska.set(null);

    this.servis.mojePrijave().subscribe({
      next: (p) => {
        this.prijave.set(p);
        this.ucitava.set(false);
      },
      error: (g) => {
        this.greska.set(porukaGreske(g, 'Prijave nisu učitane.'));
        this.ucitava.set(false);
      },
    });
  }

  protected zapocni(): void {
    this.forma.reset({ naslov: '', opis: '', kategorija: 'Oprema', prioritet: 'Srednji' });
    this.greskaForme.set(null);
    this.pisem.set(true);
  }

  protected odustani(): void {
    this.pisem.set(false);
    this.greskaForme.set(null);
  }

  protected posalji(): void {
    this.greskaForme.set(null);

    if (this.forma.invalid) {
      this.forma.markAllAsTouched();
      return;
    }

    const v = this.forma.getRawValue();
    this.salje.set(true);

    this.servis
      .prijaviProblem({
        naslov: v.naslov.trim(),
        opis: v.opis.trim(),
        kategorija: v.kategorija,
        prioritet: v.prioritet,
      })
      .subscribe({
        next: () => {
          this.salje.set(false);
          this.pisem.set(false);
          this.obavestenja.uspeh('Prijava je poslata menadžeru.');
          this.ucitaj();
        },
        error: (g) => {
          this.salje.set(false);
          this.greskaForme.set(porukaGreske(g, 'Prijava nije poslata.'));

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
}
