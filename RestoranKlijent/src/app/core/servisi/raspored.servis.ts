import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';

import { environment } from '../../../environments/environment';
import { IzmenaSmene, KreirajSmenu, RasporedNedelje, Smena } from '../modeli/api.modeli';
import { uParametre } from './upitni-parametri';

@Injectable({ providedIn: 'root' })
export class RasporedServis {
  private readonly http = inject(HttpClient);
  private readonly osnova = `${environment.apiUrl}/menadzment/raspored`;

  nedelja(nedelja?: string | null): Observable<RasporedNedelje> {
    return this.http.get<RasporedNedelje>(this.osnova, { params: uParametre({ nedelja }) });
  }

  kreiraj(podaci: KreirajSmenu): Observable<Smena> {
    return this.http.post<Smena>(this.osnova, podaci);
  }

  izmeni(id: number, podaci: IzmenaSmene): Observable<void> {
    return this.http.put<void>(`${this.osnova}/${id}`, podaci);
  }

  obrisi(id: number): Observable<void> {
    return this.http.delete<void>(`${this.osnova}/${id}`);
  }
}
