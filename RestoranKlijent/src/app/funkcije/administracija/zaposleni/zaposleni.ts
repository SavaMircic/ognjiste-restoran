import { ChangeDetectionStrategy, Component, computed, inject, signal } from '@angular/core';
import { takeUntilDestroyed, toObservable } from '@angular/core/rxjs-interop';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { Observable, debounceTime, distinctUntilChanged, skip } from 'rxjs';

import { greskePoPolju, porukaGreske } from '../../../core/greske/poruke-gresaka';
import { ULOGE_OSOBLJA, Uloga, Zaposleni } from '../../../core/modeli/api.modeli';
import { DatumCev } from '../../../core/pipes/datum.pipe';
import { AdminKorisniciServis } from '../../../core/servisi/admin-korisnici.servis';
import { ObavestenjaServis } from '../../../core/servisi/obavestenja.servis';
import { PotvrdaServis } from '../../../core/servisi/potvrda.servis';
import { nazivUloge } from '../../../core/tekst/uloge';
import { Paginacija } from '../../zajednicko/paginacija/paginacija';

const PO_STRANI = 20;

@Component({
  selector: 'og-admin-zaposleni',
  imports: [ReactiveFormsModule, DatumCev, Paginacija],
  changeDetection: ChangeDetectionStrategy.OnPush,
  templateUrl: './zaposleni.html',
  styleUrl: './zaposleni.scss',
})
export class AdminZaposleni {
  private readonly fb = inject(FormBuilder);
  private readonly potvrda = inject(PotvrdaServis);
  private readonly servis = inject(AdminKorisniciServis);
  private readonly obavestenja = inject(ObavestenjaServis);

  protected readonly pretraga = signal('');
  protected readonly uloga = signal<string | null>(null);
  protected readonly aktivan = signal<boolean | null>(null);
  protected readonly strana = signal(1);

  protected readonly zaposleni = signal<Zaposleni[]>([]);
  protected readonly ukupnoStrana = signal(1);
  protected readonly ukupnoZapisa = signal(0);
  protected readonly ucitava = signal(true);
  protected readonly greska = signal<string | null>(null);

  protected readonly menjam = signal<number | null>(null);
  protected readonly cuva = signal(false);
  protected readonly greskaForme = signal<string | null>(null);
  protected readonly uToku = signal<number | null>(null);

  protected readonly uloge = ULOGE_OSOBLJA;
  protected readonly nazivUloge = nazivUloge;

  protected readonly prazno = computed(
    () => !this.ucitava() && !this.greska() && this.zaposleni().length === 0,
  );

  protected readonly naslovForme = computed(() =>
    this.menjam() ? 'Izmena zaposlenog' : 'Nov nalog zaposlenog',
  );

  protected readonly forma = this.fb.nonNullable.group({
    ime: ['', [Validators.required, Validators.maxLength(100)]],
    prezime: ['', [Validators.required, Validators.maxLength(100)]],
    email: ['', [Validators.required, Validators.email]],
    privremenaLozinka: ['', [Validators.required, Validators.minLength(8)]],
    uloga: ['Konobar' as Uloga, Validators.required],
    datumZaposlenja: [this.danas(), Validators.required],
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
      .pretraziZaposlene({
        pretraga: this.pretraga().trim() || null,
        uloga: this.uloga(),
        aktivan: this.aktivan(),
        strana: this.strana(),
        velicinaStrane: PO_STRANI,
      })
      .subscribe({
        next: (odgovor) => {
          this.zaposleni.set(odgovor.podaci);
          this.ukupnoStrana.set(Math.max(1, odgovor.ukupnoStrana));
          this.ukupnoZapisa.set(odgovor.ukupnoZapisa);
          this.ucitava.set(false);
        },
        error: (g) => {
          this.greska.set(porukaGreske(g, 'Zaposleni nisu učitani.'));
          this.ucitava.set(false);
        },
      });
  }

  protected filtrirajUlogu(vrednost: string): void {
    this.uloga.set(vrednost || null);
    this.strana.set(1);
    this.ucitaj();
  }

  protected filtrirajStatus(vrednost: string): void {
    this.aktivan.set(vrednost === '' ? null : vrednost === 'true');
    this.strana.set(1);
    this.ucitaj();
  }

  protected naStranu(strana: number): void {
    this.strana.set(strana);
    this.ucitaj();
  }

  protected novi(): void {
    this.forma.reset({
      ime: '',
      prezime: '',
      email: '',
      privremenaLozinka: '',
      uloga: 'Konobar',
      datumZaposlenja: this.danas(),
    });
    this.forma.controls.email.enable();
    this.forma.controls.privremenaLozinka.enable();
    this.greskaForme.set(null);
    this.menjam.set(0);
  }

  protected izmeni(z: Zaposleni): void {
    this.forma.reset({
      ime: z.ime,
      prezime: z.prezime,
      email: z.email,
      privremenaLozinka: '',
      uloga: z.uloga,
      datumZaposlenja: z.datumZaposlenja.slice(0, 10),
    });

    this.forma.controls.email.disable();
    this.forma.controls.privremenaLozinka.disable();

    this.greskaForme.set(null);
    this.menjam.set(z.id);
  }

  protected odustani(): void {
    this.menjam.set(null);
    this.greskaForme.set(null);
  }

  protected sacuvaj(): void {
    this.greskaForme.set(null);

    if (this.forma.invalid) {
      this.forma.markAllAsTouched();
      return;
    }

    const v = this.forma.getRawValue();
    const id = this.menjam();
    this.cuva.set(true);

    const poziv: Observable<unknown> = id
      ? this.servis.izmeniZaposlenog(id, {
          ime: v.ime.trim(),
          prezime: v.prezime.trim(),
          uloga: v.uloga,
        })
      : this.servis.kreirajZaposlenog({
          ime: v.ime.trim(),
          prezime: v.prezime.trim(),
          email: v.email.trim(),
          privremenaLozinka: v.privremenaLozinka,
          uloga: v.uloga,
          datumZaposlenja: v.datumZaposlenja,
        });

    poziv.subscribe({
      next: () => {
        this.cuva.set(false);
        this.menjam.set(null);
        this.obavestenja.uspeh(id ? 'Zaposleni je izmenjen.' : 'Nalog zaposlenog je otvoren.');
        this.ucitaj();
      },
      error: (g) => {
        this.cuva.set(false);
        this.greskaForme.set(porukaGreske(g, 'Nalog nije sačuvan.'));

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

  protected async deaktiviraj(z: Zaposleni): Promise<void> {
    const potvrdjeno = await this.potvrda.pitaj({
      naslov: 'Deaktivacija naloga',
      tekst: `${z.ime} ${z.prezime} više neće moći da se prijavi.`,
      potvrdi: 'Deaktiviraj',
    });
    if (!potvrdjeno) return;

    this.uToku.set(z.id);

    this.servis.deaktivirajZaposlenog(z.id).subscribe({
      next: () => {
        this.uToku.set(null);
        this.obavestenja.uspeh('Nalog je deaktiviran.');
        this.ucitaj();
      },
      error: (g) => {
        this.uToku.set(null);
        this.obavestenja.greska(g, 'Nalog nije deaktiviran.');
      },
    });
  }

  private danas(): string {
    const d = new Date();
    const p = (n: number) => String(n).padStart(2, '0');
    return `${d.getFullYear()}-${p(d.getMonth() + 1)}-${p(d.getDate())}`;
  }
}
