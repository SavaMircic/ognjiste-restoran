import { ChangeDetectionStrategy, Component, computed, inject, signal } from '@angular/core';
import { takeUntilDestroyed, toObservable } from '@angular/core/rxjs-interop';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { debounceTime, distinctUntilChanged, skip } from 'rxjs';

import { greskePoPolju, porukaGreske } from '../../../core/greske/poruke-gresaka';
import { KategorijaPoruke, Poruka, StatusPoruke } from '../../../core/modeli/api.modeli';
import { DatumCev } from '../../../core/pipes/datum.pipe';
import { AdminSadrzajServis } from '../../../core/servisi/admin-sadrzaj.servis';
import { ObavestenjaServis } from '../../../core/servisi/obavestenja.servis';
import { Paginacija } from '../../zajednicko/paginacija/paginacija';

const PO_STRANI = 20;

@Component({
  selector: 'og-admin-poruke',
  imports: [ReactiveFormsModule, DatumCev, Paginacija],
  changeDetection: ChangeDetectionStrategy.OnPush,
  templateUrl: './poruke.html',
  styleUrl: './poruke.scss',
})
export class AdminPoruke {
  private readonly fb = inject(FormBuilder);
  private readonly servis = inject(AdminSadrzajServis);
  private readonly obavestenja = inject(ObavestenjaServis);

  protected readonly status = signal<StatusPoruke | null>(null);
  protected readonly kategorija = signal<KategorijaPoruke | null>(null);
  protected readonly pretraga = signal('');
  protected readonly strana = signal(1);

  protected readonly poruke = signal<Poruka[]>([]);
  protected readonly ukupnoStrana = signal(1);
  protected readonly ukupnoZapisa = signal(0);
  protected readonly ucitava = signal(true);
  protected readonly greska = signal<string | null>(null);

  protected readonly odgovaram = signal<number | null>(null);
  protected readonly salje = signal(false);
  protected readonly greskaForme = signal<string | null>(null);
  protected readonly uToku = signal<number | null>(null);

  protected readonly statusi: StatusPoruke[] = ['Novo', 'Procitano', 'Odgovoreno'];
  protected readonly kategorije: KategorijaPoruke[] = [
    'Pitanje',
    'Sugestija',
    'Rezervacija',
    'Zalba',
    'Pohvala',
  ];

  protected readonly nazivStatusa: Record<StatusPoruke, string> = {
    Novo: 'Novo',
    Procitano: 'Pročitano',
    Odgovoreno: 'Odgovoreno',
  };

  protected readonly klasaStatusa: Record<StatusPoruke, string> = {
    Novo: 'nova',
    Procitano: 'u-obradi',
    Odgovoreno: 'resena',
  };

  protected readonly novih = computed(() => this.poruke().filter((p) => p.status === 'Novo').length);

  protected readonly prazno = computed(
    () => !this.ucitava() && !this.greska() && this.poruke().length === 0,
  );

  protected readonly forma = this.fb.nonNullable.group({
    odgovor: ['', [Validators.required, Validators.maxLength(2000)]],
  });

  constructor() {
    toObservable(this.pretraga)
      .pipe(skip(1), debounceTime(300), distinctUntilChanged(), takeUntilDestroyed())
      .subscribe(() => {
        this.strana.set(1);
        this.ucitaj();
      });

    this.ucitaj();
  }

  protected ucitaj(): void {
    this.ucitava.set(true);
    this.greska.set(null);

    this.servis
      .pretraziPoruke({
        status: this.status(),
        kategorija: this.kategorija(),
        pretraga: this.pretraga().trim() || null,
        strana: this.strana(),
        velicinaStrane: PO_STRANI,
      })
      .subscribe({
        next: (odgovor) => {
          this.poruke.set(odgovor.podaci);
          this.ukupnoStrana.set(Math.max(1, odgovor.ukupnoStrana));
          this.ukupnoZapisa.set(odgovor.ukupnoZapisa);
          this.ucitava.set(false);
        },
        error: (g) => {
          this.greska.set(porukaGreske(g, 'Poruke nisu učitane.'));
          this.ucitava.set(false);
        },
      });
  }

  protected filtrirajStatus(vrednost: string): void {
    this.status.set((vrednost as StatusPoruke) || null);
    this.strana.set(1);
    this.ucitaj();
  }

  protected filtrirajKategoriju(vrednost: string): void {
    this.kategorija.set((vrednost as KategorijaPoruke) || null);
    this.strana.set(1);
    this.ucitaj();
  }

  protected naStranu(strana: number): void {
    this.strana.set(strana);
    this.ucitaj();
  }

  protected otvoriOdgovor(p: Poruka): void {
    this.forma.reset({ odgovor: p.odgovor ?? '' });
    this.greskaForme.set(null);
    this.odgovaram.set(p.id);
  }

  protected zatvoriOdgovor(): void {
    this.odgovaram.set(null);
    this.greskaForme.set(null);
  }

  protected posalji(): void {
    const id = this.odgovaram();
    if (!id) return;

    this.greskaForme.set(null);

    if (this.forma.invalid) {
      this.forma.markAllAsTouched();
      return;
    }

    this.salje.set(true);

    this.servis.odgovoriNaPoruku(id, { odgovor: this.forma.getRawValue().odgovor.trim() }).subscribe({
      next: () => {
        this.salje.set(false);
        this.odgovaram.set(null);
        this.obavestenja.uspeh('Odgovor je poslat na email pošiljaoca.');
        this.ucitaj();
      },
      error: (g) => {
        this.salje.set(false);
        this.greskaForme.set(porukaGreske(g, 'Odgovor nije poslat.'));

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

  protected oznaciProcitanom(p: Poruka): void {
    this.uToku.set(p.id);

    this.servis.oznaciProcitanom(p.id).subscribe({
      next: () => {
        this.uToku.set(null);
        this.poruke.update((sve) =>
          sve.map((x) => (x.id === p.id ? { ...x, status: 'Procitano' as StatusPoruke } : x)),
        );
      },
      error: (g) => {
        this.uToku.set(null);
        this.obavestenja.greska(g, 'Poruka nije označena pročitanom.');
      },
    });
  }
}
