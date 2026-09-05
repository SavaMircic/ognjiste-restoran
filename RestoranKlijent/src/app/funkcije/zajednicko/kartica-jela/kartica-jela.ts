import { DecimalPipe } from '@angular/common';
import { ChangeDetectionStrategy, Component, input } from '@angular/core';
import { RouterLink } from '@angular/router';

import { SlikaCev } from '../../../core/pipes/slika.pipe';
import { RezervnaSlika } from '../../../core/direktive/rezervna-slika.direktiva';
import { StavkaMenija } from '../../../core/modeli/api.modeli';
import { Zvezdice } from '../zvezdice/zvezdice';

@Component({
  selector: 'og-kartica-jela',
  imports: [RouterLink, DecimalPipe, SlikaCev, RezervnaSlika, Zvezdice],
  changeDetection: ChangeDetectionStrategy.OnPush,
  templateUrl: './kartica-jela.html',
  styleUrl: './kartica-jela.scss',
})
export class KarticaJela {
  readonly jelo = input.required<StavkaMenija>();
}
