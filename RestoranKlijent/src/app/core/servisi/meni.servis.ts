import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';

import { environment } from '../../../environments/environment';
import { KategorijaMenija, MeniPretraga, PaginiranaLista, StavkaMenija } from '../modeli/api.modeli';
import { uParametre } from './upitni-parametri';

@Injectable({ providedIn: 'root' })
export class MeniServis {
  private readonly http = inject(HttpClient);
  private readonly osnova = `${environment.apiUrl}/meni`;

  kategorije(): Observable<KategorijaMenija[]> {
    return this.http.get<KategorijaMenija[]>(`${this.osnova}/kategorije`);
  }

  stavke(filter: MeniPretraga): Observable<PaginiranaLista<StavkaMenija>> {
    return this.http.get<PaginiranaLista<StavkaMenija>>(`${this.osnova}/stavke`, {
      params: uParametre(filter),
    });
  }

  stavka(id: number): Observable<StavkaMenija> {
    return this.http.get<StavkaMenija>(`${this.osnova}/stavke/${id}`);
  }
}
