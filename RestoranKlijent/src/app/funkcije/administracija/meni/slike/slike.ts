import { ChangeDetectionStrategy, Component, computed, effect, inject, input, signal } from '@angular/core';
import { RouterLink } from '@angular/router';

import { RezervnaSlika } from '../../../../core/direktive/rezervna-slika.direktiva';
import { porukaGreske } from '../../../../core/greske/poruke-gresaka';
import { SlikaStavkeMenija, StavkaMenija } from '../../../../core/modeli/api.modeli';
import { SlikaCev } from '../../../../core/pipes/slika.pipe';
import { AdminMeniServis } from '../../../../core/servisi/admin-meni.servis';
import { MeniServis } from '../../../../core/servisi/meni.servis';
import { ObavestenjaServis } from '../../../../core/servisi/obavestenja.servis';
import { PotvrdaServis } from '../../../../core/servisi/potvrda.servis';

const MAKS_BAJTOVA = 2 * 1024 * 1024;
const EKSTENZIJE = ['.jpg', '.jpeg', '.png', '.webp'];

const MAKS_DODATNIH = 6;

@Component({
  selector: 'og-admin-slike-jela',
  imports: [RouterLink, SlikaCev, RezervnaSlika],
  changeDetection: ChangeDetectionStrategy.OnPush,
  templateUrl: './slike.html',
  styleUrl: './slike.scss',
})
export class AdminSlikeJela {
  private readonly citanje = inject(MeniServis);
  private readonly potvrda = inject(PotvrdaServis);
  private readonly servis = inject(AdminMeniServis);
  private readonly obavestenja = inject(ObavestenjaServis);

  readonly id = input.required<string>();

  protected readonly jelo = signal<StavkaMenija | null>(null);
  protected readonly dodatne = signal<SlikaStavkeMenija[]>([]);
  protected readonly ucitava = signal(true);
  protected readonly greska = signal<string | null>(null);

  protected readonly saljeGlavnu = signal(false);
  protected readonly saljeDodatnu = signal(false);
  protected readonly opisNove = signal('');
  protected readonly brisem = signal<number | null>(null);
  protected readonly menjaRedosled = signal(false);

  protected readonly popunjeno = computed(() => this.dodatne().length >= MAKS_DODATNIH);
  protected readonly maksDodatnih = MAKS_DODATNIH;

  constructor() {
    effect(() => this.ucitaj(Number(this.id())));
  }

  protected ucitaj(id: number): void {
    this.ucitava.set(true);
    this.greska.set(null);

    this.citanje.stavka(id).subscribe({
      next: (j) => {
        this.jelo.set(j);
        this.ucitava.set(false);
      },
      error: (g) => {
        this.greska.set(porukaGreske(g, 'Jelo nije učitano.'));
        this.ucitava.set(false);
      },
    });

    this.ucitajDodatne(id);
  }

  private ucitajDodatne(id: number): void {
    this.servis.listirajDodatneSlike(id).subscribe({
      next: (s) => this.dodatne.set([...s].sort((a, b) => a.redosled - b.redosled)),
      error: (g) => this.obavestenja.greska(g, 'Dodatne slike nisu učitane.'),
    });
  }

  protected postaviGlavnu(dogadjaj: Event): void {
    const ulaz = dogadjaj.target as HTMLInputElement;
    const datoteka = this.uzmiDatoteku(ulaz);
    if (!datoteka) return;

    this.saljeGlavnu.set(true);

    this.servis.postaviGlavnuSliku(Number(this.id()), datoteka).subscribe({
      next: ({ slikaUrl }) => {
        this.saljeGlavnu.set(false);
        this.jelo.update((j) => (j ? { ...j, slikaUrl } : j));
        this.obavestenja.uspeh('Glavna slika je postavljena.');
      },
      error: (g) => {
        this.saljeGlavnu.set(false);
        this.obavestenja.greska(g, 'Slika nije postavljena.');
      },
    });
  }

  protected dodajDodatnu(dogadjaj: Event): void {
    const ulaz = dogadjaj.target as HTMLInputElement;
    const datoteka = this.uzmiDatoteku(ulaz);
    if (!datoteka) return;

    this.saljeDodatnu.set(true);

    this.servis.dodajDodatnuSliku(Number(this.id()), datoteka, this.opisNove().trim()).subscribe({
      next: (slika) => {
        this.saljeDodatnu.set(false);
        this.opisNove.set('');
        this.dodatne.update((sve) => [...sve, slika]);
        this.obavestenja.uspeh('Slika je dodata u galeriju jela.');
      },
      error: (g) => {
        this.saljeDodatnu.set(false);
        this.obavestenja.greska(g, 'Slika nije dodata.');
      },
    });
  }

  protected async obrisiDodatnu(slika: SlikaStavkeMenija): Promise<void> {
    const potvrdjeno = await this.potvrda.pitaj({
      naslov: 'Brisanje slike',
      tekst: 'Slika se trajno uklanja iz galerije ovog jela.',
      potvrdi: 'Obriši',
    });
    if (!potvrdjeno) return;

    this.brisem.set(slika.id);

    this.servis.obrisiDodatnuSliku(Number(this.id()), slika.id).subscribe({
      next: () => {
        this.brisem.set(null);
        this.dodatne.update((sve) => sve.filter((s) => s.id !== slika.id));
        this.obavestenja.uspeh('Slika je obrisana.');
      },
      error: (g) => {
        this.brisem.set(null);
        this.obavestenja.greska(g, 'Slika nije obrisana.');
      },
    });
  }

  protected pomeri(indeks: number, za: number): void {
    const novi = [...this.dodatne()];
    const cilj = indeks + za;
    if (cilj < 0 || cilj >= novi.length || this.menjaRedosled()) return;

    [novi[indeks], novi[cilj]] = [novi[cilj], novi[indeks]];

    const prethodno = this.dodatne();
    this.dodatne.set(novi);
    this.menjaRedosled.set(true);

    this.servis.poredjajSlike(Number(this.id()), novi.map((s) => s.id)).subscribe({
      next: () => this.menjaRedosled.set(false),
      error: (g) => {
        this.menjaRedosled.set(false);
        this.dodatne.set(prethodno);
        this.obavestenja.greska(g, 'Redosled nije sačuvan.');
      },
    });
  }

  private uzmiDatoteku(ulaz: HTMLInputElement): File | null {
    const datoteka = ulaz.files?.[0] ?? null;
    ulaz.value = '';

    if (!datoteka) return null;

    const tacka = datoteka.name.lastIndexOf('.');
    const ekstenzija = tacka === -1 ? '' : datoteka.name.slice(tacka).toLowerCase();

    if (!EKSTENZIJE.includes(ekstenzija)) {
      this.obavestenja.greska(null, `Dozvoljeni formati: ${EKSTENZIJE.join(', ')}.`);
      return null;
    }

    if (datoteka.size > MAKS_BAJTOVA) {
      this.obavestenja.greska(null, 'Slika ne može biti veća od 2 MB.');
      return null;
    }

    return datoteka;
  }
}
