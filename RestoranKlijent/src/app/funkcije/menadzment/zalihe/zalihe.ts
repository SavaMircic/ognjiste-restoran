import { DecimalPipe } from '@angular/common';
import { ChangeDetectionStrategy, Component, computed, inject, signal } from '@angular/core';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { Observable, forkJoin } from 'rxjs';

import { greskePoPolju, porukaGreske } from '../../../core/greske/poruke-gresaka';
import {
  JedinicaMere,
  KorekcijaKolicine,
  NabavkaStavka,
  Namirnica,
} from '../../../core/modeli/api.modeli';
import { ObavestenjaServis } from '../../../core/servisi/obavestenja.servis';
import { ZaliheServis } from '../../../core/servisi/zalihe.servis';

type Panel = 'nova' | 'izmena' | 'korekcija';

@Component({
  selector: 'og-menadzer-zalihe',
  imports: [ReactiveFormsModule, DecimalPipe],
  changeDetection: ChangeDetectionStrategy.OnPush,
  templateUrl: './zalihe.html',
  styleUrl: './zalihe.scss',
})
export class MenadzerZalihe {
  private readonly fb = inject(FormBuilder);
  private readonly servis = inject(ZaliheServis);
  private readonly obavestenja = inject(ObavestenjaServis);

  protected readonly namirnice = signal<Namirnica[]>([]);
  protected readonly nabavka = signal<NabavkaStavka[]>([]);
  protected readonly ucitava = signal(true);
  protected readonly greska = signal<string | null>(null);

  protected readonly pretraga = signal('');
  protected readonly samoIspodPraga = signal(false);

  protected readonly panel = signal<Panel | null>(null);
  protected readonly ciljId = signal<number | null>(null);
  protected readonly ciljNaziv = signal('');
  protected readonly cuva = signal(false);
  protected readonly greskaForme = signal<string | null>(null);

  protected readonly jedinice: JedinicaMere[] = ['Kg', 'L', 'Kom'];

  protected readonly ispodPraga = computed(() => this.namirnice().filter((n) => n.ispodPraga).length);

  protected readonly prikazane = computed(() => {
    const tekst = this.pretraga().trim().toLowerCase();
    return this.namirnice()
      .filter((n) => !this.samoIspodPraga() || n.ispodPraga)
      .filter((n) => !tekst || n.naziv.toLowerCase().includes(tekst));
  });

  protected readonly prazno = computed(
    () => !this.ucitava() && !this.greska() && this.prikazane().length === 0,
  );

  protected readonly formaNove = this.fb.nonNullable.group({
    naziv: ['', [Validators.required, Validators.maxLength(100)]],
    jedinicaMere: ['Kg' as JedinicaMere, Validators.required],
    minimalniPrag: [0, [Validators.required, Validators.min(0)]],
    pocetnaKolicina: [0, [Validators.required, Validators.min(0)]],
  });

  protected readonly formaIzmene = this.fb.nonNullable.group({
    naziv: ['', [Validators.required, Validators.maxLength(100)]],
    minimalniPrag: [0, [Validators.required, Validators.min(0)]],
  });

  protected readonly formaKorekcije = this.fb.nonNullable.group({
    nacin: ['delta' as 'delta' | 'popis', Validators.required],
    vrednost: [0, Validators.required],
    razlog: ['', [Validators.required, Validators.maxLength(500)]],
  });

  constructor() {
    this.ucitaj();
  }

  protected ucitaj(): void {
    this.ucitava.set(true);
    this.greska.set(null);

    forkJoin({ sve: this.servis.namirnice(), nabavka: this.servis.listaZaNabavku() }).subscribe({
      next: ({ sve, nabavka }) => {
        this.namirnice.set([...sve].sort((a, b) => a.naziv.localeCompare(b.naziv, 'sr')));
        this.nabavka.set(nabavka);
        this.ucitava.set(false);
      },
      error: (g) => {
        this.greska.set(porukaGreske(g, 'Zalihe nisu učitane.'));
        this.ucitava.set(false);
      },
    });
  }

  protected nova(): void {
    this.formaNove.reset({ naziv: '', jedinicaMere: 'Kg', minimalniPrag: 0, pocetnaKolicina: 0 });
    this.greskaForme.set(null);
    this.ciljId.set(null);
    this.panel.set('nova');
  }

  protected izmeni(n: Namirnica): void {
    this.formaIzmene.reset({ naziv: n.naziv, minimalniPrag: n.minimalniPrag });
    this.greskaForme.set(null);
    this.ciljId.set(n.id);
    this.ciljNaziv.set(n.naziv);
    this.panel.set('izmena');
  }

  protected korekcija(n: Namirnica): void {
    this.formaKorekcije.reset({ nacin: 'delta', vrednost: 0, razlog: '' });
    this.greskaForme.set(null);
    this.ciljId.set(n.id);
    this.ciljNaziv.set(n.naziv);
    this.panel.set('korekcija');
  }

  protected zatvori(): void {
    this.panel.set(null);
    this.greskaForme.set(null);
  }

  protected posalji(): void {
    const koji = this.panel();
    if (!koji) return;

    this.greskaForme.set(null);

    const forma: FormGroup =
      koji === 'nova'
        ? this.formaNove
        : koji === 'izmena'
          ? this.formaIzmene
          : this.formaKorekcije;

    if (forma.invalid) {
      forma.markAllAsTouched();
      return;
    }

    this.cuva.set(true);
    const id = this.ciljId();
    let poziv: Observable<unknown>;

    if (koji === 'nova') {
      const v = this.formaNove.getRawValue();
      poziv = this.servis.kreirajNamirnicu({
        naziv: v.naziv.trim(),
        jedinicaMere: v.jedinicaMere,
        minimalniPrag: Number(v.minimalniPrag),
        pocetnaKolicina: Number(v.pocetnaKolicina),
      });
    } else if (koji === 'izmena') {
      const v = this.formaIzmene.getRawValue();
      poziv = this.servis.izmeniNamirnicu(id!, {
        naziv: v.naziv.trim(),
        minimalniPrag: Number(v.minimalniPrag),
      });
    } else {
      const v = this.formaKorekcije.getRawValue();
      const podaci: KorekcijaKolicine =
        v.nacin === 'popis'
          ? { novaKolicina: Number(v.vrednost), delta: null, razlog: v.razlog.trim() }
          : { novaKolicina: null, delta: Number(v.vrednost), razlog: v.razlog.trim() };

      poziv = this.servis.korigujKolicinu(id!, podaci);
    }

    poziv.subscribe({
      next: () => {
        this.cuva.set(false);
        this.panel.set(null);
        this.obavestenja.uspeh(
          koji === 'nova'
            ? 'Namirnica je dodata.'
            : koji === 'izmena'
              ? 'Namirnica je izmenjena.'
              : 'Stanje je korigovano.',
        );
        this.ucitaj();
      },
      error: (g) => {
        this.cuva.set(false);
        this.greskaForme.set(porukaGreske(g, 'Izmena nije sačuvana.'));

        for (const [polje, poruke] of Object.entries(greskePoPolju(g))) {
          if (!polje || !poruke?.length) continue;
          const naziv = polje[0].toLowerCase() + polje.slice(1);
          const kontrola = forma.get(naziv);
          kontrola?.setErrors({ server: poruke[0] });
          kontrola?.markAsTouched();
        }
      },
    });
  }

  protected manjak(n: Namirnica): number {
    return Math.max(0, n.minimalniPrag - n.trenutnaKolicina);
  }
}
