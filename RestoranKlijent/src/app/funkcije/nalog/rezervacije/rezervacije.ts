import { ChangeDetectionStrategy, Component, computed, inject, signal } from '@angular/core';
import { RouterLink } from '@angular/router';

import { DatumCev } from '../../../core/pipes/datum.pipe';
import { porukaGreske } from '../../../core/greske/poruke-gresaka';
import { Rezervacija, StatusRezervacije } from '../../../core/modeli/api.modeli';
import { ObavestenjaServis } from '../../../core/servisi/obavestenja.servis';
import { PotvrdaServis } from '../../../core/servisi/potvrda.servis';
import { RezervacijeServis } from '../../../core/servisi/rezervacije.servis';
import { izServera } from '../../../core/tekst/vreme';

const ROK_OTKAZIVANJA_SATI = 2;

@Component({
  selector: 'og-moje-rezervacije',
  imports: [RouterLink, DatumCev],
  changeDetection: ChangeDetectionStrategy.OnPush,
  templateUrl: './rezervacije.html',
  styleUrl: './rezervacije.scss',
})
export class MojeRezervacije {
  private readonly servis = inject(RezervacijeServis);
  private readonly potvrda = inject(PotvrdaServis);
  private readonly obavestenja = inject(ObavestenjaServis);

  protected readonly sve = signal<Rezervacija[]>([]);
  protected readonly ucitava = signal(true);
  protected readonly greska = signal<string | null>(null);
  protected readonly otkazujeId = signal<number | null>(null);

  protected readonly aktivne = computed(() =>
    this.sve()
      .filter((r) => r.status === 'Aktivna')
      .sort((a, b) => (izServera(a.datumVreme)?.getTime() ?? 0) - (izServera(b.datumVreme)?.getTime() ?? 0)),
  );

  protected readonly istorija = computed(() =>
    this.sve()
      .filter((r) => r.status !== 'Aktivna')
      .sort((a, b) => (izServera(b.datumVreme)?.getTime() ?? 0) - (izServera(a.datumVreme)?.getTime() ?? 0)),
  );

  protected readonly prazno = computed(() => !this.ucitava() && !this.greska() && this.sve().length === 0);

  protected readonly realizovanih = computed(() => this.sve().filter((r) => r.status === 'Realizovana').length);
  protected readonly isteklih = computed(() => this.sve().filter((r) => r.status === 'Istekla').length);

  constructor() {
    this.ucitaj();
  }

  protected ucitaj(): void {
    this.ucitava.set(true);
    this.greska.set(null);

    this.servis.moje().subscribe({
      next: (r) => {
        this.sve.set(r);
        this.ucitava.set(false);
      },
      error: (g) => {
        this.greska.set(porukaGreske(g, 'Rezervacije nisu učitane.'));
        this.ucitava.set(false);
      },
    });
  }

  protected smeDaOtkaze(r: Rezervacija): boolean {
    const termin = izServera(r.datumVreme);
    if (!termin) return false;
    return termin.getTime() - Date.now() > ROK_OTKAZIVANJA_SATI * 3600 * 1000;
  }

  protected async otkazi(r: Rezervacija): Promise<void> {
    const potvrdjeno = await this.potvrda.pitaj({
      naslov: 'Otkazivanje rezervacije',
      tekst: `Rezervacija ${r.kodRezervacije} za sto br. ${r.brojStola} se otkazuje. Za isti termin ćete morati da rezervišete ponovo.`,
      potvrdi: 'Otkaži rezervaciju',
      odustani: 'Zadrži',
    });
    if (!potvrdjeno) return;

    this.otkazujeId.set(r.id);

    this.servis.otkazi(r.id).subscribe({
      next: () => {
        this.otkazujeId.set(null);
        this.obavestenja.uspeh('Rezervacija je otkazana.');
        this.ucitaj();
      },
      error: (g) => {
        this.otkazujeId.set(null);
        this.obavestenja.greska(g, 'Otkazivanje nije uspelo.');
      },
    });
  }

  protected oznaka(status: StatusRezervacije): string {
    const nazivi: Record<StatusRezervacije, string> = {
      Aktivna: 'Aktivna',
      Realizovana: 'Realizovana',
      Istekla: 'Istekla',
      Otkazana: 'Otkazana',
    };
    return nazivi[status];
  }
}
