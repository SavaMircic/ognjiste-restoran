import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';

import { environment } from '../../../environments/environment';
import {
  Bonus,
  BonusiPretraga,
  KreirajBonus,
  PaginiranaLista,
  PrihodIzvestaj,
  PrihodUpit,
  TopJelaUpit,
  TopJelo,
  UcinakUpit,
  UcinakZaposlenog,
} from '../modeli/api.modeli';
import { uParametre } from './upitni-parametri';

@Injectable({ providedIn: 'root' })
export class IzvestajiServis {
  private readonly http = inject(HttpClient);
  private readonly osnova = `${environment.apiUrl}/menadzment`;

  prihod(upit: PrihodUpit): Observable<PrihodIzvestaj> {
    return this.http.get<PrihodIzvestaj>(`${this.osnova}/izvestaji/prihod`, {
      params: uParametre(upit),
    });
  }

  ucinakZaposlenih(upit: UcinakUpit): Observable<UcinakZaposlenog[]> {
    return this.http.get<UcinakZaposlenog[]>(`${this.osnova}/izvestaji/ucinak-zaposlenih`, {
      params: uParametre(upit),
    });
  }

  topJela(upit: TopJelaUpit): Observable<TopJelo[]> {
    return this.http.get<TopJelo[]>(`${this.osnova}/izvestaji/top-jela`, {
      params: uParametre(upit),
    });
  }

  pretraziBonuse(filter: BonusiPretraga): Observable<PaginiranaLista<Bonus>> {
    return this.http.get<PaginiranaLista<Bonus>>(`${this.osnova}/bonusi`, {
      params: uParametre(filter),
    });
  }

  dodeliBonus(podaci: KreirajBonus): Observable<Bonus[]> {
    return this.http.post<Bonus[]>(`${this.osnova}/bonusi`, podaci);
  }
}
