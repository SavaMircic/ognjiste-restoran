import { ChangeDetectionStrategy, Component, computed, input } from '@angular/core';
import { RouterLink } from '@angular/router';

import { DatumCev } from '../../../core/pipes/datum.pipe';
import { Recenzija } from '../../../core/modeli/api.modeli';
import { RezervnaSlika } from '../../../core/direktive/rezervna-slika.direktiva';
import { SlikaCev } from '../../../core/pipes/slika.pipe';
import { Zvezdice } from '../zvezdice/zvezdice';

@Component({
  selector: 'og-kartica-recenzije',
  imports: [RouterLink, DatumCev, RezervnaSlika, SlikaCev, Zvezdice],
  changeDetection: ChangeDetectionStrategy.OnPush,
  templateUrl: './kartica-recenzije.html',
  styleUrl: './kartica-recenzije.scss',
})
export class KarticaRecenzije {
  readonly recenzija = input.required<Recenzija>();
  readonly prikaziJelo = input(true);

  protected readonly inicijali = computed(() =>
    this.recenzija()
      .autorIme.split(' ')
      .filter(Boolean)
      .slice(0, 2)
      .map((d) => d[0]?.toUpperCase() ?? '')
      .join(''),
  );
}
