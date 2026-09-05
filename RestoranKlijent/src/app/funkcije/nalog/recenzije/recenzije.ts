import { ChangeDetectionStrategy, Component, computed, inject, signal } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';

import { DatumCev } from '../../../core/pipes/datum.pipe';
import { porukaGreske } from '../../../core/greske/poruke-gresaka';
import { Recenzija, TipRecenzije } from '../../../core/modeli/api.modeli';
import { AuthServis } from '../../../core/servisi/auth.servis';
import { ObavestenjaServis } from '../../../core/servisi/obavestenja.servis';
import { PotvrdaServis } from '../../../core/servisi/potvrda.servis';
import { RecenzijeServis } from '../../../core/servisi/recenzije.servis';
import { Zvezdice } from '../../zajednicko/zvezdice/zvezdice';

@Component({
  selector: 'og-moje-recenzije',
  imports: [ReactiveFormsModule, DatumCev, Zvezdice],
  changeDetection: ChangeDetectionStrategy.OnPush,
  templateUrl: './recenzije.html',
  styleUrl: './recenzije.scss',
})
export class MojeRecenzije {
  private readonly fb = inject(FormBuilder);
  private readonly potvrda = inject(PotvrdaServis);
  private readonly servis = inject(RecenzijeServis);
  private readonly auth = inject(AuthServis);
  private readonly obavestenja = inject(ObavestenjaServis);

  protected readonly moje = signal<Recenzija[]>([]);
  protected readonly ucitava = signal(true);
  protected readonly greska = signal<string | null>(null);

  protected readonly menjaId = signal<number | null>(null);
  protected readonly cuva = signal(false);
  protected readonly brisId = signal<number | null>(null);
  protected readonly greskaForme = signal<string | null>(null);

  protected readonly pisemNovu = signal(false);

  protected readonly prazno = computed(() => !this.ucitava() && !this.greska() && this.moje().length === 0);

  protected readonly slobodniTipovi = computed<TipRecenzije[]>(() => {
    const zauzeti = new Set(this.moje().map((r) => r.tipRecenzije));
    return (['Usluga', 'Restoran'] as TipRecenzije[]).filter((t) => !zauzeti.has(t));
  });

  protected readonly nazivTipa: Record<TipRecenzije, string> = {
    Jelo: 'O jelu',
    Usluga: 'O usluzi',
    Restoran: 'O restoranu',
  };

  protected readonly forma = this.fb.nonNullable.group({
    tipRecenzije: ['Usluga' as TipRecenzije, Validators.required],
    ocena: [5, [Validators.required, Validators.min(1), Validators.max(5)]],
    naslov: [''],
    tekst: ['', [Validators.required, Validators.maxLength(2000)]],
  });

  constructor() {
    this.ucitaj();
  }

  protected ucitaj(): void {
    this.ucitava.set(true);
    this.greska.set(null);

    const id = this.auth.sesija()?.korisnikId;
    if (!id) {
      this.greska.set('Sesija nije prepoznata. Prijavite se ponovo.');
      this.ucitava.set(false);
      return;
    }

    this.servis.moje(id).subscribe({
      next: (r) => {
        this.moje.set(r);
        this.ucitava.set(false);
      },
      error: (g) => {
        this.greska.set(porukaGreske(g, 'Recenzije nisu učitane.'));
        this.ucitava.set(false);
      },
    });
  }

  protected zapocniNovu(): void {
    const prvi = this.slobodniTipovi()[0];
    if (!prvi) return;

    this.menjaId.set(null);
    this.greskaForme.set(null);
    this.forma.reset({ tipRecenzije: prvi, ocena: 5, naslov: '', tekst: '' });
    this.pisemNovu.set(true);
  }

  protected zapocniIzmenu(r: Recenzija): void {
    this.pisemNovu.set(false);
    this.greskaForme.set(null);
    this.forma.reset({
      tipRecenzije: r.tipRecenzije,
      ocena: r.ocena,
      naslov: r.naslov ?? '',
      tekst: r.tekst,
    });
    this.menjaId.set(r.id);
  }

  protected odustani(): void {
    this.menjaId.set(null);
    this.pisemNovu.set(false);
    this.greskaForme.set(null);
  }

  protected postaviOcenu(ocena: number): void {
    this.forma.controls.ocena.setValue(ocena);
  }

  protected sacuvaj(): void {
    this.greskaForme.set(null);

    if (this.forma.invalid) {
      this.forma.markAllAsTouched();
      return;
    }

    const v = this.forma.getRawValue();
    const id = this.menjaId();
    this.cuva.set(true);

    const zavrsi = (poruka: string) => {
      this.cuva.set(false);
      this.odustani();
      this.obavestenja.uspeh(poruka);
      this.ucitaj();
    };

    const pukni = (g: unknown) => {
      this.cuva.set(false);
      this.greskaForme.set(porukaGreske(g, 'Recenzija nije sačuvana.'));
    };

    if (id !== null) {
      this.servis
        .izmeni(id, { ocena: v.ocena, naslov: v.naslov.trim() || null, tekst: v.tekst.trim() })
        .subscribe({ next: () => zavrsi('Recenzija je izmenjena.'), error: pukni });
    } else {
      this.servis
        .kreiraj({
          tipRecenzije: v.tipRecenzije,
          stavkaMenijaId: null,
          ocena: v.ocena,
          naslov: v.naslov.trim() || null,
          tekst: v.tekst.trim(),
        })
        .subscribe({ next: () => zavrsi('Recenzija je objavljena.'), error: pukni });
    }
  }

  protected async obrisi(r: Recenzija): Promise<void> {
    const potvrdjeno = await this.potvrda.pitaj({
      naslov: 'Brisanje recenzije',
      tekst: 'Recenzija se trajno uklanja i više neće biti vidljiva drugim gostima.',
      potvrdi: 'Obriši',
    });
    if (!potvrdjeno) return;

    this.brisId.set(r.id);

    this.servis.obrisi(r.id).subscribe({
      next: () => {
        this.brisId.set(null);
        this.obavestenja.uspeh('Recenzija je obrisana.');
        this.ucitaj();
      },
      error: (g) => {
        this.brisId.set(null);
        this.obavestenja.greska(g, 'Brisanje nije uspelo.');
      },
    });
  }
}
