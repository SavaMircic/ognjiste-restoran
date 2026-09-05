import { DatePipe } from '@angular/common';
import { ChangeDetectionStrategy, Component, computed, inject, signal } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { catchError, of } from 'rxjs';

import { greskePoPolju, porukaGreske } from '../../../core/greske/poruke-gresaka';
import { Smena, Uloga, Zaposleni } from '../../../core/modeli/api.modeli';
import { DatumCev } from '../../../core/pipes/datum.pipe';
import { AdminKorisniciServis } from '../../../core/servisi/admin-korisnici.servis';
import { ObavestenjaServis } from '../../../core/servisi/obavestenja.servis';
import { PotvrdaServis } from '../../../core/servisi/potvrda.servis';
import { RasporedServis } from '../../../core/servisi/raspored.servis';
import { saBrojem } from '../../../core/tekst/mnozina';
import { nazivUloge } from '../../../core/tekst/uloge';

const DANI = ['Ponedeljak', 'Utorak', 'Sreda', 'Četvrtak', 'Petak', 'Subota', 'Nedelja'];
const DANI_KRATKO = ['Pon', 'Uto', 'Sre', 'Čet', 'Pet', 'Sub', 'Ned'];

const TERMINI = [
  { naziv: 'Prepodne', od: '08:00', do: '16:00' },
  { naziv: 'Popodne', od: '16:00', do: '23:00' },
  { naziv: 'Noćna', od: '22:00', do: '02:00' },
];

interface DanZaglavlje {
  datum: string;
  naziv: string;
  kratko: string;
  jeDanas: boolean;
  brojSmena: number;
}

interface CelijaDana {
  dan: DanZaglavlje;
  smene: Smena[];
}

interface RedRasporeda {
  zaposleniId: number;
  ime: string;
  uloga: Uloga | null;
  aktivan: boolean;
  ukupnoSati: number;
  celije: CelijaDana[];
}

@Component({
  selector: 'og-menadzer-raspored',
  imports: [ReactiveFormsModule, DatePipe, DatumCev],
  changeDetection: ChangeDetectionStrategy.OnPush,
  templateUrl: './raspored.html',
  styleUrl: './raspored.scss',
})
export class MenadzerRaspored {
  private readonly fb = inject(FormBuilder);
  private readonly potvrda = inject(PotvrdaServis);
  private readonly servis = inject(RasporedServis);
  private readonly zaposleniServis = inject(AdminKorisniciServis);
  private readonly obavestenja = inject(ObavestenjaServis);

  protected readonly pocetak = signal(this.ponedeljak(new Date()));
  protected readonly smene = signal<Smena[]>([]);
  protected readonly zaposleni = signal<Zaposleni[]>([]);
  protected readonly satiPoZaposlenom = signal<Record<number, number>>({});
  protected readonly imenaSaSmenama = signal<Record<number, string>>({});
  protected readonly ucitava = signal(true);
  protected readonly greska = signal<string | null>(null);
  private readonly zaposleniUcitani = signal(false);
  protected readonly spisakPao = signal(false);

  protected readonly otvorena = signal(false);
  protected readonly uredjujem = signal<Smena | null>(null);
  protected readonly salje = signal(false);
  protected readonly brisem = signal(false);
  protected readonly greskaForme = signal<string | null>(null);

  protected readonly termini = TERMINI;
  protected readonly nazivUloge = nazivUloge;
  protected readonly saBrojem = saBrojem;

  protected readonly forma = this.fb.nonNullable.group({
    zaposleniId: [0, [Validators.required, Validators.min(1)]],
    datum: ['', Validators.required],
    vremePocetka: ['08:00', Validators.required],
    vremeKraja: ['16:00', Validators.required],
  });

  private readonly vrednosti = signal(this.forma.getRawValue());

  protected readonly kraj = computed(() => {
    const d = new Date(this.pocetak());
    d.setDate(d.getDate() + 6);
    return d;
  });

  protected readonly jeTekuca = computed(
    () => this.uIso(this.pocetak()) === this.uIso(this.ponedeljak(new Date())),
  );

  protected readonly dani = computed<DanZaglavlje[]>(() => {
    const danas = this.uIso(new Date());
    const smene = this.smene();

    return Array.from({ length: 7 }, (_, i) => {
      const d = new Date(this.pocetak());
      d.setDate(d.getDate() + i);
      const iso = this.uIso(d);

      return {
        datum: iso,
        naziv: DANI[i],
        kratko: DANI_KRATKO[i],
        jeDanas: iso === danas,
        brojSmena: smene.filter((s) => s.datum === iso).length,
      };
    });
  });

  protected readonly redovi = computed<RedRasporeda[]>(() => {
    const sati = this.satiPoZaposlenom();
    const imena = this.imenaSaSmenama();
    const aktivni = this.zaposleni();
    const dani = this.dani();
    const smene = this.smene();

    const svi = new Map<number, { ime: string; uloga: Uloga | null; aktivan: boolean }>();

    for (const z of aktivni) {
      svi.set(z.id, { ime: `${z.ime} ${z.prezime}`, uloga: z.uloga, aktivan: true });
    }

    for (const [id, ime] of Object.entries(imena)) {
      const broj = Number(id);
      if (!svi.has(broj)) svi.set(broj, { ime, uloga: null, aktivan: false });
    }

    return [...svi.entries()]
      .map(([zaposleniId, o]) => ({
        zaposleniId,
        ime: o.ime,
        uloga: o.uloga,
        aktivan: o.aktivan,
        ukupnoSati: sati[zaposleniId] ?? 0,
        celije: dani.map((dan) => ({
          dan,
          smene: smene.filter((s) => s.zaposleniId === zaposleniId && s.datum === dan.datum),
        })),
      }))
      .sort((a, b) => a.ime.localeCompare(b.ime, 'sr'));
  });

  protected readonly ukupnoSmena = computed(() => this.smene().length);

  protected readonly prazno = computed(
    () =>
      !this.ucitava() &&
      !this.greska() &&
      this.zaposleniUcitani() &&
      this.redovi().length === 0,
  );

  protected readonly ukupnoSati = computed(() =>
    Math.round(this.smene().reduce((zbir, s) => zbir + s.trajanjeSati, 0) * 100) / 100,
  );

  protected readonly trajanje = computed(() => {
    const v = this.vrednosti();
    const od = this.uMinute(v.vremePocetka);
    const do_ = this.uMinute(v.vremeKraja);
    if (od === null || do_ === null) return null;

    const minuta = do_ > od ? do_ - od : do_ + 24 * 60 - od;
    return { sati: Math.round((minuta / 60) * 100) / 100, prelaziPonoc: do_ <= od };
  });

  constructor() {
    this.forma.valueChanges
      .pipe(takeUntilDestroyed())
      .subscribe(() => this.vrednosti.set(this.forma.getRawValue()));

    this.zaposleniServis
      .pretraziZaposlene({ aktivan: true, velicinaStrane: 100 })
      .pipe(catchError(() => of(null)))
      .subscribe((o) => {
        if (o) this.zaposleni.set([...o.podaci].sort((a, b) => a.prezime.localeCompare(b.prezime, 'sr')));
        else this.spisakPao.set(true);
        this.zaposleniUcitani.set(true);
      });

    this.ucitaj();
  }

  protected ucitaj(): void {
    this.ucitava.set(true);
    this.greska.set(null);

    this.servis.nedelja(this.uIso(this.pocetak())).subscribe({
      next: (r) => {
        this.smene.set(r.zaposleni.flatMap((z) => z.smene));
        this.satiPoZaposlenom.set(
          Object.fromEntries(r.zaposleni.map((z) => [z.zaposleniId, z.ukupnoSati])),
        );
        this.imenaSaSmenama.set(
          Object.fromEntries(r.zaposleni.map((z) => [z.zaposleniId, z.imeZaposlenog])),
        );
        this.ucitava.set(false);
      },
      error: (g) => {
        this.greska.set(porukaGreske(g, 'Raspored nije učitan.'));
        this.ucitava.set(false);
      },
    });
  }

  protected prethodna(): void {
    this.pomeri(-7);
  }

  protected sledeca(): void {
    this.pomeri(7);
  }

  protected naTekucu(): void {
    this.pocetak.set(this.ponedeljak(new Date()));
    this.zatvori();
    this.ucitaj();
  }

  protected nova(zaposleniId: number, datum: string): void {
    this.uredjujem.set(null);
    this.greskaForme.set(null);
    this.forma.reset({ zaposleniId, datum, vremePocetka: '08:00', vremeKraja: '16:00' });
    this.forma.controls.zaposleniId.enable();
    this.otvorena.set(true);
  }

  protected izmeni(smena: Smena): void {
    this.uredjujem.set(smena);
    this.greskaForme.set(null);
    this.forma.reset({
      zaposleniId: smena.zaposleniId,
      datum: smena.datum,
      vremePocetka: smena.vremePocetka.slice(0, 5),
      vremeKraja: smena.vremeKraja.slice(0, 5),
    });
    this.forma.controls.zaposleniId.disable();
    this.otvorena.set(true);
  }

  protected zatvori(): void {
    this.otvorena.set(false);
    this.uredjujem.set(null);
    this.greskaForme.set(null);
  }

  protected postaviTermin(od: string, do_: string): void {
    this.forma.patchValue({ vremePocetka: od, vremeKraja: do_ });
  }

  protected sacuvaj(): void {
    this.greskaForme.set(null);

    if (this.forma.invalid) {
      this.forma.markAllAsTouched();
      return;
    }

    const v = this.forma.getRawValue();
    const smena = this.uredjujem();
    this.salje.set(true);

    const zavrsi = (poruka: string) => {
      this.salje.set(false);
      this.zatvori();
      this.obavestenja.uspeh(poruka);
      this.ucitaj();
    };

    const pad = (g: unknown) => {
      this.salje.set(false);
      this.greskaForme.set(porukaGreske(g, 'Smena nije sačuvana.'));

      for (const [polje, poruke] of Object.entries(greskePoPolju(g))) {
        if (!polje || !poruke?.length) continue;
        const naziv = polje[0].toLowerCase() + polje.slice(1);
        const kontrola = this.forma.get(naziv);
        kontrola?.setErrors({ server: poruke[0] });
        kontrola?.markAsTouched();
      }
    };

    const termin = { datum: v.datum, vremePocetka: v.vremePocetka, vremeKraja: v.vremeKraja };

    if (smena) {
      this.servis.izmeni(smena.id, termin).subscribe({
        next: () => zavrsi('Smena je izmenjena.'),
        error: pad,
      });
    } else {
      this.servis.kreiraj({ zaposleniId: Number(v.zaposleniId), ...termin }).subscribe({
        next: () => zavrsi('Smena je dodata u raspored.'),
        error: pad,
      });
    }
  }

  protected async ukloni(): Promise<void> {
    const smena = this.uredjujem();
    if (!smena) return;

    const kada = `${smena.datum.split('-').reverse().join('.')} ${smena.vremePocetka.slice(0, 5)}`;
    const potvrdjeno = await this.potvrda.pitaj({
      naslov: 'Uklanjanje smene',
      tekst: `Smena ${smena.imeZaposlenog}, ${kada}, briše se iz rasporeda.`,
      potvrdi: 'Ukloni smenu',
    });
    if (!potvrdjeno) return;

    this.brisem.set(true);

    this.servis.obrisi(smena.id).subscribe({
      next: () => {
        this.brisem.set(false);
        this.zatvori();
        this.obavestenja.uspeh('Smena je uklonjena iz rasporeda.');
        this.ucitaj();
      },
      error: (g) => {
        this.brisem.set(false);
        this.greskaForme.set(porukaGreske(g, 'Smena nije uklonjena.'));
      },
    });
  }

  protected zapoceta(smena: Smena): boolean {
    return new Date(`${smena.datum}T${smena.vremePocetka}`) <= new Date();
  }

  private pomeri(dana: number): void {
    const d = new Date(this.pocetak());
    d.setDate(d.getDate() + dana);
    this.pocetak.set(d);
    this.zatvori();
    this.ucitaj();
  }

  private ponedeljak(datum: Date): Date {
    const d = new Date(datum);
    d.setHours(0, 0, 0, 0);
    d.setDate(d.getDate() + (d.getDay() === 0 ? -6 : 1 - d.getDay()));
    return d;
  }

  private uIso(datum: Date): string {
    const p = (n: number) => String(n).padStart(2, '0');
    return `${datum.getFullYear()}-${p(datum.getMonth() + 1)}-${p(datum.getDate())}`;
  }

  private uMinute(vreme: string): number | null {
    const [sat, minut] = vreme.split(':').map(Number);
    return Number.isFinite(sat) && Number.isFinite(minut) ? sat * 60 + minut : null;
  }
}
