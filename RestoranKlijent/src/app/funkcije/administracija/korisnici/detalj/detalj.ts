import { ChangeDetectionStrategy, Component, computed, effect, inject, signal } from '@angular/core';
import { input } from '@angular/core';
import { RouterLink } from '@angular/router';

import { porukaGreske } from '../../../../core/greske/poruke-gresaka';
import { KorisnikDetalj, StatusRezervacije } from '../../../../core/modeli/api.modeli';
import { DatumCev } from '../../../../core/pipes/datum.pipe';
import { AdminKorisniciServis } from '../../../../core/servisi/admin-korisnici.servis';

@Component({
  selector: 'og-admin-korisnik-detalj',
  imports: [RouterLink, DatumCev],
  changeDetection: ChangeDetectionStrategy.OnPush,
  templateUrl: './detalj.html',
  styleUrl: './detalj.scss',
})
export class AdminKorisnikDetalj {
  private readonly servis = inject(AdminKorisniciServis);

  readonly id = input.required<string>();

  protected readonly detalj = signal<KorisnikDetalj | null>(null);
  protected readonly ucitava = signal(true);
  protected readonly greska = signal<string | null>(null);

  protected readonly uklonjenih = computed(
    () => this.detalj()?.recenzije.filter((r) => !r.aktivan).length ?? 0,
  );

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

  constructor() {
    effect(() => this.ucitaj(this.id()));
  }

  protected ucitaj(id: string): void {
    this.ucitava.set(true);
    this.greska.set(null);

    this.servis.detalj(id).subscribe({
      next: (d) => {
        this.detalj.set(d);
        this.ucitava.set(false);
      },
      error: (g) => {
        this.greska.set(porukaGreske(g, 'Korisnik nije učitan.'));
        this.ucitava.set(false);
      },
    });
  }

  protected zvezdice(ocena: number): string {
    return '★'.repeat(ocena) + '☆'.repeat(Math.max(0, 5 - ocena));
  }
}
