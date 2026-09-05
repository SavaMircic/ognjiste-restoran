import { DecimalPipe } from '@angular/common';
import { ChangeDetectionStrategy, Component, computed, effect, inject, input, signal } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { RouterLink } from '@angular/router';
import { catchError, of } from 'rxjs';

import { SlikaCev } from '../../../core/pipes/slika.pipe';
import { RezervnaSlika } from '../../../core/direktive/rezervna-slika.direktiva';
import { porukaGreske } from '../../../core/greske/poruke-gresaka';
import { Recenzija, StavkaMenija } from '../../../core/modeli/api.modeli';
import { AuthServis } from '../../../core/servisi/auth.servis';
import { LajkoviServis } from '../../../core/servisi/lajkovi.servis';
import { MeniServis } from '../../../core/servisi/meni.servis';
import { ObavestenjaServis } from '../../../core/servisi/obavestenja.servis';
import { ProfilServis } from '../../../core/servisi/profil.servis';
import { RecenzijeServis } from '../../../core/servisi/recenzije.servis';
import { KarticaRecenzije } from '../../zajednicko/kartica-recenzije/kartica-recenzije';
import { Zvezdice } from '../../zajednicko/zvezdice/zvezdice';

@Component({
  selector: 'og-detalj-jela',
  imports: [
    RouterLink,
    ReactiveFormsModule,
    DecimalPipe,
    SlikaCev,
    RezervnaSlika,
    Zvezdice,
    KarticaRecenzije,
  ],
  changeDetection: ChangeDetectionStrategy.OnPush,
  templateUrl: './detalj-jela.html',
  styleUrl: './detalj-jela.scss',
})
export class DetaljJela {
  private readonly meniServis = inject(MeniServis);
  private readonly recenzijeServis = inject(RecenzijeServis);
  private readonly lajkoviServis = inject(LajkoviServis);
  private readonly profilServis = inject(ProfilServis);
  private readonly obavestenja = inject(ObavestenjaServis);
  private readonly fb = inject(FormBuilder);
  protected readonly auth = inject(AuthServis);

  readonly id = input.required<string>();

  protected readonly jelo = signal<StavkaMenija | null>(null);
  protected readonly recenzije = signal<Recenzija[]>([]);
  protected readonly ucitava = signal(true);
  protected readonly greska = signal<string | null>(null);

  protected readonly izabranaSlika = signal(-1);

  protected readonly sveSlike = computed(() => {
    const j = this.jelo();
    if (!j) return [];
    return [
      { url: j.slikaUrl, opis: j.naziv },
      ...j.dodatneSlike.map((s) => ({ url: s.slikaUrl, opis: s.opis ?? j.naziv })),
    ];
  });

  protected readonly velikaSlika = computed(() => {
    const slike = this.sveSlike();
    if (!slike.length) return null;
    const i = this.izabranaSlika();
    return i === -1 ? slike[0] : (slike[i + 1] ?? slike[0]);
  });

  protected readonly imaPopust = computed(() => !!this.jelo()?.popust);

  protected readonly jeKorisnik = computed(() => this.auth.imaUlogu(['Korisnik']));
  protected readonly lajkovano = signal(false);
  protected readonly menjaLajk = signal(false);

  private ucitajLajk(stavkaId: number): void {
    if (!this.jeKorisnik()) return;

    this.profilServis
      .omiljenaJela()
      .pipe(catchError(() => of([] as StavkaMenija[])))
      .subscribe((jela) => this.lajkovano.set(jela.some((j) => j.id === stavkaId)));
  }

  protected prebaciLajk(): void {
    const j = this.jelo();
    if (!j || this.menjaLajk()) return;

    const bilo = this.lajkovano();
    this.menjaLajk.set(true);

    const zahtev = bilo ? this.lajkoviServis.ukloni(j.id) : this.lajkoviServis.lajkuj(j.id);

    zahtev.subscribe({
      next: () => {
        this.menjaLajk.set(false);
        this.lajkovano.set(!bilo);
        this.jelo.update((s) => (s ? { ...s, brojLajkova: s.brojLajkova + (bilo ? -1 : 1) } : s));
      },
      error: (g) => {
        this.menjaLajk.set(false);
        this.obavestenja.greska(g, 'Radnja nije uspela.');
      },
    });
  }

  protected readonly mojaRecenzija = computed(() => {
    const mojId = this.auth.sesija()?.korisnikId;
    return mojId ? (this.recenzije().find((r) => r.korisnikId === mojId) ?? null) : null;
  });

  protected readonly pisemRecenziju = signal(false);
  protected readonly cuvaRecenziju = signal(false);
  protected readonly greskaRecenzije = signal<string | null>(null);

  protected readonly formaRecenzije = this.fb.nonNullable.group({
    ocena: [5, [Validators.required, Validators.min(1), Validators.max(5)]],
    naslov: [''],
    tekst: ['', [Validators.required, Validators.maxLength(2000)]],
  });

  protected zapocniRecenziju(): void {
    const postojeca = this.mojaRecenzija();
    this.formaRecenzije.reset({
      ocena: postojeca?.ocena ?? 5,
      naslov: postojeca?.naslov ?? '',
      tekst: postojeca?.tekst ?? '',
    });
    this.greskaRecenzije.set(null);
    this.pisemRecenziju.set(true);
  }

  protected odustaniOdRecenzije(): void {
    this.pisemRecenziju.set(false);
    this.greskaRecenzije.set(null);
  }

  protected postaviOcenu(ocena: number): void {
    this.formaRecenzije.controls.ocena.setValue(ocena);
  }

  protected sacuvajRecenziju(): void {
    const j = this.jelo();
    if (!j) return;

    this.greskaRecenzije.set(null);

    if (this.formaRecenzije.invalid) {
      this.formaRecenzije.markAllAsTouched();
      return;
    }

    const v = this.formaRecenzije.getRawValue();
    const postojeca = this.mojaRecenzija();
    this.cuvaRecenziju.set(true);

    const zavrsi = (poruka: string) => {
      this.cuvaRecenziju.set(false);
      this.pisemRecenziju.set(false);
      this.obavestenja.uspeh(poruka);
      this.ucitaj(j.id);
    };

    const pukni = (g: unknown) => {
      this.cuvaRecenziju.set(false);
      this.greskaRecenzije.set(porukaGreske(g, 'Recenzija nije sačuvana.'));
    };

    if (postojeca) {
      this.recenzijeServis
        .izmeni(postojeca.id, { ocena: v.ocena, naslov: v.naslov.trim() || null, tekst: v.tekst.trim() })
        .subscribe({ next: () => zavrsi('Recenzija je izmenjena.'), error: pukni });
    } else {
      this.recenzijeServis
        .kreiraj({
          tipRecenzije: 'Jelo',
          stavkaMenijaId: j.id,
          ocena: v.ocena,
          naslov: v.naslov.trim() || null,
          tekst: v.tekst.trim(),
        })
        .subscribe({ next: () => zavrsi('Hvala na recenziji.'), error: pukni });
    }
  }

  constructor() {
    effect(() => {
      const id = Number(this.id());
      this.ucitaj(id);
    });
  }

  protected izaberi(indeks: number): void {
    this.izabranaSlika.set(indeks);
  }

  protected ucitaj(id: number): void {
    this.ucitava.set(true);
    this.greska.set(null);
    this.izabranaSlika.set(-1);

    if (!Number.isInteger(id) || id < 1) {
      this.greska.set('Neispravna adresa jela.');
      this.ucitava.set(false);
      return;
    }

    this.meniServis.stavka(id).subscribe({
      next: (j) => {
        this.jelo.set(j);
        this.ucitava.set(false);
        this.ucitajLajk(j.id);
      },
      error: (g) => {
        this.jelo.set(null);
        this.greska.set(porukaGreske(g, 'Ovo jelo nije pronađeno.'));
        this.ucitava.set(false);
      },
    });

    this.recenzijeServis
      .pretrazi({ stavkaMenijaId: id, strana: 1, velicinaStrane: 20 })
      .pipe(catchError(() => of({ podaci: [] as Recenzija[], ukupnoZapisa: 0, trenutnaStrana: 1, ukupnoStrana: 0 })))
      .subscribe((odgovor) => this.recenzije.set(odgovor.podaci));
  }
}
