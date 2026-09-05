import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable, map } from 'rxjs';

import { environment } from '../../../environments/environment';
import {
  IzmenaRecenzije,
  KreirajRecenziju,
  PaginiranaLista,
  Recenzija,
  RecenzijePretraga,
} from '../modeli/api.modeli';
import { uParametre } from './upitni-parametri';

const MAKS_STRANA = 100;

@Injectable({ providedIn: 'root' })
export class RecenzijeServis {
  private readonly http = inject(HttpClient);
  private readonly osnova = `${environment.apiUrl}/recenzije`;

  pretrazi(filter: RecenzijePretraga): Observable<PaginiranaLista<Recenzija>> {
    return this.http.get<PaginiranaLista<Recenzija>>(this.osnova, { params: uParametre(filter) });
  }

  moje(korisnikId: string): Observable<Recenzija[]> {
    return this.pretrazi({ strana: 1, velicinaStrane: MAKS_STRANA }).pipe(
      map((odgovor) => odgovor.podaci.filter((r) => r.korisnikId === korisnikId)),
    );
  }

  kreiraj(podaci: KreirajRecenziju): Observable<Recenzija> {
    return this.http.post<Recenzija>(this.osnova, podaci);
  }

  izmeni(id: number, podaci: IzmenaRecenzije): Observable<void> {
    return this.http.put<void>(`${this.osnova}/${id}`, podaci);
  }

  obrisi(id: number): Observable<void> {
    return this.http.delete<void>(`${this.osnova}/${id}`);
  }
}
