import { ChangeDetectionStrategy, Component, computed, inject, signal } from '@angular/core';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';

import { greskePoPolju, porukaGreske } from '../../../core/greske/poruke-gresaka';
import { Recenzija, TipRecenzije } from '../../../core/modeli/api.modeli';
import { DatumCev } from '../../../core/pipes/datum.pipe';
import { AdminKorisniciServis } from '../../../core/servisi/admin-korisnici.servis';
import { ObavestenjaServis } from '../../../core/servisi/obavestenja.servis';
import { RecenzijeServis } from '../../../core/servisi/recenzije.servis';
import { Paginacija } from '../../zajednicko/paginacija/paginacija';

const PO_STRANI = 20;

@Component({
  selector: 'og-admin-recenzije',
  imports: [ReactiveFormsModule, DatumCev, Paginacija],
  changeDetection: ChangeDetectionStrategy.OnPush,
  templateUrl: './recenzije.html',
  styleUrl: './recenzije.scss',
})
export class AdminRecenzije {
  private readonly fb = inject(FormBuilder);
  private readonly citanje = inject(RecenzijeServis);
  private readonly servis = inject(AdminKorisniciServis);
  private readonly obavestenja = inject(ObavestenjaServis);

  protected readonly tip = signal<TipRecenzije | null>(null);
  protected readonly samoBezOdgovora = signal(false);
  protected readonly strana = signal(1);

  private readonly sve = signal<Recenzija[]>([]);
  protected readonly ukupnoStrana = signal(1);
  protected readonly ukupnoZapisa = signal(0);
  protected readonly ucitava = signal(true);
  protected readonly greska = signal<string | null>(null);

  protected readonly odgovaram = signal<number | null>(null);
  protected readonly salje = signal(false);
  protected readonly greskaForme = signal<string | null>(null);
  protected readonly uklanjam = signal<number | null>(null);

  protected readonly tipovi: TipRecenzije[] = ['Jelo', 'Usluga', 'Restoran'];

  protected readonly recenzije = computed(() =>
    this.samoBezOdgovora() ? this.sve().filter((r) => !r.odgovorRestorana) : this.sve(),
  );

  protected readonly bezOdgovoraNaStrani = computed(
    () => this.sve().filter((r) => !r.odgovorRestorana).length,
  );

  protected readonly prazno = computed(
    () => !this.ucitava() && !this.greska() && this.recenzije().length === 0,
  );

  protected readonly forma = this.fb.nonNullable.group({
    odgovorRestorana: ['', [Validators.required, Validators.maxLength(1000)]],
  });

  protected readonly formaUklanjanja = this.fb.nonNullable.group({
    razlog: ['', [Validators.required, Validators.maxLength(500)]],
  });

  constructor() {
    this.ucitaj();
  }

  protected ucitaj(): void {
    this.ucitava.set(true);
    this.greska.set(null);

    this.citanje
      .pretrazi({
        tipRecenzije: this.tip(),
        strana: this.strana(),
        velicinaStrane: PO_STRANI,
      })
      .subscribe({
        next: (odgovor) => {
          this.sve.set(odgovor.podaci);
          this.ukupnoStrana.set(Math.max(1, odgovor.ukupnoStrana));
          this.ukupnoZapisa.set(odgovor.ukupnoZapisa);
          this.ucitava.set(false);
        },
        error: (g) => {
          this.greska.set(porukaGreske(g, 'Recenzije nisu učitane.'));
          this.ucitava.set(false);
        },
      });
  }

  protected filtrirajTip(vrednost: string): void {
    this.tip.set((vrednost as TipRecenzije) || null);
    this.strana.set(1);
    this.ucitaj();
  }

  protected naStranu(strana: number): void {
    this.strana.set(strana);
    this.ucitaj();
  }

  protected otvoriOdgovor(r: Recenzija): void {
    this.forma.reset({ odgovorRestorana: r.odgovorRestorana ?? '' });
    this.greskaForme.set(null);
    this.odgovaram.set(r.id);
  }

  protected zatvoriOdgovor(): void {
    this.odgovaram.set(null);
    this.greskaForme.set(null);
  }

  protected posaljiOdgovor(): void {
    const id = this.odgovaram();
    if (!id) return;

    this.greskaForme.set(null);

    if (this.forma.invalid) {
      this.forma.markAllAsTouched();
      return;
    }

    this.salje.set(true);
    const tekst = this.forma.getRawValue().odgovorRestorana.trim();

    this.servis.odgovoriNaRecenziju(id, { odgovorRestorana: tekst }).subscribe({
      next: () => {
        this.salje.set(false);
        this.odgovaram.set(null);
        this.obavestenja.uspeh('Odgovor je objavljen.');
        this.ucitaj();
      },
      error: (g) => {
        this.salje.set(false);
        this.greskaForme.set(porukaGreske(g, 'Odgovor nije objavljen.'));
        this.prikaziGreskePolja(g, 'odgovorRestorana');
      },
    });
  }

  protected otvoriUklanjanje(r: Recenzija): void {
    this.formaUklanjanja.reset({ razlog: '' });
    this.greskaForme.set(null);
    this.uklanjam.set(r.id);
  }

  protected zatvoriUklanjanje(): void {
    this.uklanjam.set(null);
    this.greskaForme.set(null);
  }

  protected potvrdiUklanjanje(): void {
    const id = this.uklanjam();
    if (!id) return;

    this.greskaForme.set(null);

    if (this.formaUklanjanja.invalid) {
      this.formaUklanjanja.markAllAsTouched();
      return;
    }

    this.salje.set(true);
    const razlog = this.formaUklanjanja.getRawValue().razlog.trim();

    this.servis.obrisiRecenziju(id, razlog).subscribe({
      next: () => {
        this.salje.set(false);
        this.uklanjam.set(null);
        this.obavestenja.uspeh('Recenzija je uklonjena.');
        this.ucitaj();
      },
      error: (g) => {
        this.salje.set(false);
        this.greskaForme.set(porukaGreske(g, 'Recenzija nije uklonjena.'));
        this.prikaziGreskePolja(g, 'razlog');
      },
    });
  }

  protected zvezdice(ocena: number): string {
    return '★'.repeat(ocena) + '☆'.repeat(Math.max(0, 5 - ocena));
  }

  private prikaziGreskePolja(g: unknown, rezervnoPolje: string): void {
    const forma: FormGroup = rezervnoPolje === 'razlog' ? this.formaUklanjanja : this.forma;

    for (const [polje, poruke] of Object.entries(greskePoPolju(g))) {
      if (!poruke?.length) continue;
      const naziv = polje ? polje[0].toLowerCase() + polje.slice(1) : rezervnoPolje;
      const kontrola = forma.get(naziv);
      kontrola?.setErrors({ server: poruke[0] });
      kontrola?.markAsTouched();
    }
  }
}
