import { ChangeDetectionStrategy, Component, computed, inject, signal } from '@angular/core';
import { takeUntilDestroyed, toObservable } from '@angular/core/rxjs-interop';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { debounceTime, distinctUntilChanged, skip } from 'rxjs';

import { greskePoPolju, porukaGreske } from '../../../core/greske/poruke-gresaka';
import { IME_OBRAZAC, TELEFON_OBRAZAC } from '../../../core/validacija/obrasci';
import { Rezervacija, StatusRezervacije, Sto } from '../../../core/modeli/api.modeli';
import { DatumCev } from '../../../core/pipes/datum.pipe';
import { AdminSadrzajServis } from '../../../core/servisi/admin-sadrzaj.servis';
import { ObavestenjaServis } from '../../../core/servisi/obavestenja.servis';
import { RezervacijeServis } from '../../../core/servisi/rezervacije.servis';
import { saBrojem } from '../../../core/tekst/mnozina';
import { uUtcIso, zaSatVremena } from '../../../core/tekst/vreme';
import { Paginacija } from '../../zajednicko/paginacija/paginacija';

const PO_STRANI = 20;

@Component({
  selector: 'og-admin-rezervacije',
  imports: [ReactiveFormsModule, DatumCev, Paginacija],
  changeDetection: ChangeDetectionStrategy.OnPush,
  templateUrl: './rezervacije.html',
  styleUrl: './rezervacije.scss',
})
export class AdminRezervacije {
  private readonly fb = inject(FormBuilder);
  private readonly servis = inject(AdminSadrzajServis);
  private readonly rezervacijeServis = inject(RezervacijeServis);
  private readonly obavestenja = inject(ObavestenjaServis);

  protected readonly datum = signal('');
  protected readonly status = signal<StatusRezervacije | null>(null);
  protected readonly pretraga = signal('');
  protected readonly strana = signal(1);

  protected readonly rezervacije = signal<Rezervacija[]>([]);
  protected readonly ukupnoStrana = signal(1);
  protected readonly ukupnoZapisa = signal(0);
  protected readonly ucitava = signal(true);
  protected readonly greska = signal<string | null>(null);

  protected readonly pisem = signal(false);
  protected readonly cuva = signal(false);
  protected readonly greskaForme = signal<string | null>(null);
  protected readonly slobodni = signal<Sto[]>([]);
  protected readonly traziStolove = signal(false);
  protected readonly proveravano = signal(false);

  protected readonly statusi: StatusRezervacije[] = [
    'Aktivna',
    'Realizovana',
    'Istekla',
    'Otkazana',
  ];

  protected readonly nazivStatusa: Record<StatusRezervacije, string> = {
    Aktivna: 'Aktivna',
    Realizovana: 'Realizovana',
    Istekla: 'Nije došao',
    Otkazana: 'Otkazana',
  };

  protected readonly klasaStatusa: Record<StatusRezervacije, string> = {
    Aktivna: 'nova',
    Realizovana: 'resena',
    Istekla: 'odbijena',
    Otkazana: '',
  };

  protected readonly prazno = computed(
    () => !this.ucitava() && !this.greska() && this.rezervacije().length === 0,
  );

  protected readonly forma = this.fb.nonNullable.group({
    datumVreme: [zaSatVremena(), Validators.required],
    brojGostiju: [2, [Validators.required, Validators.min(1)]],
    stoId: [0, [Validators.required, Validators.min(1)]],
    nacinKreiranja: ['TelefonAdmin' as 'TelefonAdmin' | 'KontaktForma', Validators.required],
    gostIme: ['', [Validators.required, Validators.maxLength(100), Validators.pattern(IME_OBRAZAC)]],
    gostEmail: ['', [Validators.required, Validators.email]],
    gostTelefon: ['', [Validators.required, Validators.pattern(TELEFON_OBRAZAC)]],
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
      .pretraziRezervacije({
        datum: this.datum() || null,
        status: this.status(),
        pretraga: this.pretraga().trim() || null,
        strana: this.strana(),
        velicinaStrane: PO_STRANI,
      })
      .subscribe({
        next: (odgovor) => {
          this.rezervacije.set(odgovor.podaci);
          this.ukupnoStrana.set(Math.max(1, odgovor.ukupnoStrana));
          this.ukupnoZapisa.set(odgovor.ukupnoZapisa);
          this.ucitava.set(false);
        },
        error: (g) => {
          this.greska.set(porukaGreske(g, 'Rezervacije nisu učitane.'));
          this.ucitava.set(false);
        },
      });
  }

  protected postaviDatum(vrednost: string): void {
    this.datum.set(vrednost);
    this.strana.set(1);
    this.ucitaj();
  }

  protected filtrirajStatus(vrednost: string): void {
    this.status.set((vrednost as StatusRezervacije) || null);
    this.strana.set(1);
    this.ucitaj();
  }

  protected naStranu(strana: number): void {
    this.strana.set(strana);
    this.ucitaj();
  }

  protected zapocni(): void {
    this.forma.reset({
      datumVreme: zaSatVremena(),
      brojGostiju: 2,
      stoId: 0,
      nacinKreiranja: 'TelefonAdmin',
      gostIme: '',
      gostEmail: '',
      gostTelefon: '',
    });
    this.slobodni.set([]);
    this.proveravano.set(false);
    this.greskaForme.set(null);
    this.pisem.set(true);
  }

  protected odustani(): void {
    this.pisem.set(false);
    this.greskaForme.set(null);
  }

  protected proveriDostupnost(): void {
    const v = this.forma.getRawValue();
    if (!v.datumVreme || !v.brojGostiju) return;

    this.traziStolove.set(true);
    this.forma.controls.stoId.setValue(0);

    this.rezervacijeServis.dostupnost(uUtcIso(v.datumVreme), Number(v.brojGostiju)).subscribe({
      next: (d) => {
        this.slobodni.set(d.stolovi);
        this.proveravano.set(true);
        this.traziStolove.set(false);
        if (d.stolovi.length === 1) this.forma.controls.stoId.setValue(d.stolovi[0].id);
      },
      error: (g) => {
        this.traziStolove.set(false);
        this.proveravano.set(true);
        this.slobodni.set([]);
        this.obavestenja.greska(g, 'Provera dostupnosti nije uspela.');
      },
    });
  }

  protected sacuvaj(): void {
    this.greskaForme.set(null);

    if (this.forma.invalid) {
      this.forma.markAllAsTouched();
      return;
    }

    const v = this.forma.getRawValue();
    this.cuva.set(true);

    this.servis
      .kreirajRezervaciju({
        stoId: Number(v.stoId),
        datumVreme: uUtcIso(v.datumVreme),
        brojGostiju: Number(v.brojGostiju),
        nacinKreiranja: v.nacinKreiranja,
        gostIme: v.gostIme.trim(),
        gostEmail: v.gostEmail.trim(),
        gostTelefon: v.gostTelefon.trim(),
      })
      .subscribe({
        next: (r) => {
          this.cuva.set(false);
          this.pisem.set(false);
          this.obavestenja.uspeh(`Rezervacija je kreirana — kod ${r.kodRezervacije}.`);
          this.ucitaj();
        },
        error: (g) => {
          this.cuva.set(false);
          this.greskaForme.set(porukaGreske(g, 'Rezervacija nije kreirana.'));

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

  protected gosti(broj: number): string {
    return saBrojem(broj, 'gost', 'gosta', 'gostiju');
  }
}
