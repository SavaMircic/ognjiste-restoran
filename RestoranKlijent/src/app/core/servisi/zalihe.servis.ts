import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';

import { environment } from '../../../environments/environment';
import {
  DodajURecepturu,
  IzmenaKolicine,
  IzmenaNamirnice,
  KorekcijaKolicine,
  KreirajNamirnicu,
  NabavkaStavka,
  Namirnica,
  ReceptStavka,
} from '../modeli/api.modeli';

@Injectable({ providedIn: 'root' })
export class ZaliheServis {
  private readonly http = inject(HttpClient);
  private readonly namirniceUrl = `${environment.apiUrl}/menadzment/namirnice`;
  private readonly meniUrl = `${environment.apiUrl}/administracija/meni`;

  namirnice(): Observable<Namirnica[]> {
    return this.http.get<Namirnica[]>(this.namirniceUrl);
  }

  listaZaNabavku(): Observable<NabavkaStavka[]> {
    return this.http.get<NabavkaStavka[]>(`${this.namirniceUrl}/lista-za-nabavku`);
  }

  kreirajNamirnicu(podaci: KreirajNamirnicu): Observable<Namirnica> {
    return this.http.post<Namirnica>(this.namirniceUrl, podaci);
  }

  izmeniNamirnicu(id: number, podaci: IzmenaNamirnice): Observable<void> {
    return this.http.put<void>(`${this.namirniceUrl}/${id}`, podaci);
  }

  korigujKolicinu(id: number, podaci: KorekcijaKolicine): Observable<Namirnica> {
    return this.http.patch<Namirnica>(`${this.namirniceUrl}/${id}/kolicina`, podaci);
  }

  receptura(stavkaMenijaId: number): Observable<ReceptStavka[]> {
    return this.http.get<ReceptStavka[]>(`${this.meniUrl}/stavke/${stavkaMenijaId}/receptura`);
  }

  dodajURecepturu(stavkaMenijaId: number, podaci: DodajURecepturu): Observable<unknown> {
    return this.http.post(`${this.meniUrl}/stavke/${stavkaMenijaId}/receptura`, podaci);
  }

  izmeniKolicinuURecepturi(
    stavkaMenijaId: number,
    namirnicaId: number,
    podaci: IzmenaKolicine,
  ): Observable<void> {
    return this.http.put<void>(
      `${this.meniUrl}/stavke/${stavkaMenijaId}/receptura/${namirnicaId}`,
      podaci,
    );
  }

  ukloniIzRecepture(stavkaMenijaId: number, namirnicaId: number): Observable<void> {
    return this.http.delete<void>(
      `${this.meniUrl}/stavke/${stavkaMenijaId}/receptura/${namirnicaId}`,
    );
  }
}
