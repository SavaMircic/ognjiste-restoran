import { ChangeDetectionStrategy, Component, computed, inject, signal } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { DomSanitizer, SafeResourceUrl } from '@angular/platform-browser';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { catchError, of } from 'rxjs';

import { greskePoPolju, porukaGreske } from '../../../core/greske/poruke-gresaka';
import { KategorijaPoruke, Postavke } from '../../../core/modeli/api.modeli';
import { AuthServis } from '../../../core/servisi/auth.servis';
import { ObavestenjaServis } from '../../../core/servisi/obavestenja.servis';
import { PorukeServis } from '../../../core/servisi/poruke.servis';
import { SadrzajServis } from '../../../core/servisi/sadrzaj.servis';
import { IME_OBRAZAC, TELEFON_OBRAZAC } from '../../../core/validacija/obrasci';

@Component({
  selector: 'og-kontakt',
  imports: [ReactiveFormsModule],
  changeDetection: ChangeDetectionStrategy.OnPush,
  templateUrl: './kontakt.html',
  styleUrl: './kontakt.scss',
})
export class Kontakt {
  private readonly fb = inject(FormBuilder);
  private readonly porukeServis = inject(PorukeServis);
  private readonly sadrzajServis = inject(SadrzajServis);
  private readonly obavestenja = inject(ObavestenjaServis);
  protected readonly auth = inject(AuthServis);
  private readonly sanitizer = inject(DomSanitizer);

  protected readonly postavke = signal<Postavke | null>(null);
  protected readonly salje = signal(false);
  protected readonly poslato = signal(false);
  protected readonly greska = signal<string | null>(null);

  protected readonly kategorije = computed<KategorijaPoruke[]>(() =>
    this.auth.prijavljen()
      ? ['Pitanje', 'Sugestija', 'Zalba', 'Pohvala']
      : ['Pitanje', 'Sugestija', 'Rezervacija', 'Zalba', 'Pohvala'],
  );

  protected readonly nazivKategorije: Record<KategorijaPoruke, string> = {
    Pitanje: 'Pitanje',
    Sugestija: 'Sugestija',
    Rezervacija: 'Zahtev za rezervaciju',
    Zalba: 'Žalba',
    Pohvala: 'Pohvala',
  };

  protected readonly forma = this.fb.nonNullable.group({
    ime: [''],
    email: [''],
    telefon: [''],
    kategorija: ['Pitanje' as KategorijaPoruke, Validators.required],
    zeljeniDatumVreme: [''],
    zeljeniBrojGostiju: [2],
    tekst: ['', [Validators.required, Validators.maxLength(2000)]],
  });

  protected readonly najranijiTermin = signal(this.zaSatVremena());

  protected readonly jeRezervacija = signal(false);

  protected readonly mapaUrl = computed<SafeResourceUrl | null>(() => {
    const p = this.postavke();
    if (p?.geoSirina == null || p?.geoDuzina == null) return null;

    const lat = p.geoSirina;
    const lon = p.geoDuzina;
    const bbox = [lon - 0.006, lat - 0.004, lon + 0.006, lat + 0.004].join(',');

    return this.sanitizer.bypassSecurityTrustResourceUrl(
      `https://www.openstreetmap.org/export/embed.html?bbox=${bbox}&layer=mapnik&marker=${lat},${lon}`,
    );
  });

  protected readonly linkZaMapu = computed(() => {
    const p = this.postavke();
    if (p?.geoSirina == null || p?.geoDuzina == null) return null;
    return `https://www.openstreetmap.org/?mlat=${p.geoSirina}&mlon=${p.geoDuzina}#map=17/${p.geoSirina}/${p.geoDuzina}`;
  });

  constructor() {
    this.sadrzajServis
      .postavke()
      .pipe(catchError(() => of(null)))
      .subscribe((p) => p && this.postavke.set(p));

    this.postaviPraviloGosta();

    this.forma.controls.kategorija.valueChanges
      .pipe(takeUntilDestroyed())
      .subscribe((k) => this.postaviPraviloRezervacije(k));
  }

  protected posalji(): void {
    this.greska.set(null);

    if (this.forma.invalid) {
      this.forma.markAllAsTouched();
      return;
    }

    const v = this.forma.getRawValue();
    const jeRez = v.kategorija === 'Rezervacija';

    this.salje.set(true);

    this.porukeServis
      .posalji({
        kategorija: v.kategorija,
        tekst: v.tekst.trim(),
        ime: this.auth.prijavljen() ? null : v.ime.trim(),
        email: this.auth.prijavljen() ? null : v.email.trim(),
        telefon: this.auth.prijavljen() ? null : v.telefon.trim(),
        zeljeniDatumVreme: jeRez ? v.zeljeniDatumVreme : null,
        zeljeniBrojGostiju: jeRez ? v.zeljeniBrojGostiju : null,
      })
      .subscribe({
        next: () => {
          this.salje.set(false);
          this.poslato.set(true);
          this.obavestenja.uspeh('Poruka je poslata. Javljamo se uskoro.');
          this.forma.reset({ kategorija: 'Pitanje', zeljeniBrojGostiju: 2, tekst: '' });
          this.postaviPraviloRezervacije('Pitanje');
        },
        error: (g) => {
          this.salje.set(false);
          this.greska.set(porukaGreske(g, 'Poruka nije poslata.'));
          this.upisiGreskeServera(g);
        },
      });
  }

  protected novaPoruka(): void {
    this.poslato.set(false);
  }

  private postaviPraviloGosta(): void {
    if (this.auth.prijavljen()) return;

    this.forma.controls.ime.addValidators([
      Validators.required,
      Validators.minLength(2),
      Validators.maxLength(100),
      Validators.pattern(IME_OBRAZAC),
    ]);
    this.forma.controls.email.addValidators([Validators.required, Validators.email]);
    this.forma.controls.telefon.addValidators([Validators.required, Validators.pattern(TELEFON_OBRAZAC)]);

    this.forma.controls.ime.updateValueAndValidity();
    this.forma.controls.email.updateValueAndValidity();
    this.forma.controls.telefon.updateValueAndValidity();
  }

  private postaviPraviloRezervacije(kategorija: KategorijaPoruke): void {
    const jeRez = kategorija === 'Rezervacija';
    this.jeRezervacija.set(jeRez);
    this.najranijiTermin.set(this.zaSatVremena());

    const termin = this.forma.controls.zeljeniDatumVreme;
    const gosti = this.forma.controls.zeljeniBrojGostiju;

    termin.clearValidators();
    gosti.clearValidators();

    if (jeRez) {
      termin.addValidators(Validators.required);
      gosti.addValidators([Validators.required, Validators.min(1), Validators.max(50)]);
    }

    termin.updateValueAndValidity();
    gosti.updateValueAndValidity();
  }

  private upisiGreskeServera(g: unknown): void {
    for (const [polje, poruke] of Object.entries(greskePoPolju(g))) {
      if (!polje || !poruke?.length) continue;

      const naziv = polje[0].toLowerCase() + polje.slice(1);
      const kontrola = this.forma.get(naziv);
      kontrola?.setErrors({ server: poruke[0] });
      kontrola?.markAsTouched();
    }
  }

  private zaSatVremena(): string {
    const d = new Date(Date.now() + 60 * 60 * 1000);
    const pad = (n: number) => String(n).padStart(2, '0');
    return `${d.getFullYear()}-${pad(d.getMonth() + 1)}-${pad(d.getDate())}T${pad(d.getHours())}:${pad(d.getMinutes())}`;
  }
}
