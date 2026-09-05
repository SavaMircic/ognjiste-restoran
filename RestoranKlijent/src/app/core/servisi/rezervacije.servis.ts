import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';

import { environment } from '../../../environments/environment';
import { Dostupnost, KreirajRezervaciju, Rezervacija } from '../modeli/api.modeli';
import { uParametre } from './upitni-parametri';

@Injectable({ providedIn: 'root' })
export class RezervacijeServis {
  private readonly http = inject(HttpClient);
  private readonly osnova = `${environment.apiUrl}/rezervacije`;

  dostupnost(datumVremeUtc: string, brojGostiju: number): Observable<Dostupnost> {
    return this.http.get<Dostupnost>(`${this.osnova}/dostupnost`, {
      params: uParametre({ datumVreme: datumVremeUtc, brojGostiju }),
    });
  }

  kreiraj(podaci: KreirajRezervaciju): Observable<Rezervacija> {
    return this.http.post<Rezervacija>(this.osnova, podaci);
  }

  moje(): Observable<Rezervacija[]> {
    return this.http.get<Rezervacija[]>(`${this.osnova}/moje`);
  }

  otkazi(id: number): Observable<void> {
    return this.http.delete<void>(`${this.osnova}/${id}`);
  }

  zaDanas(): Observable<Rezervacija[]> {
    return this.http.get<Rezervacija[]>(`${this.osnova}/danas`);
  }

  prijaviDolazak(id: number): Observable<void> {
    return this.http.patch<void>(`${this.osnova}/${id}/prijavi-dolazak`, {});
  }

  oznaciIsteklom(id: number): Observable<void> {
    return this.http.patch<void>(`${this.osnova}/${id}/istekla`, {});
  }
}
