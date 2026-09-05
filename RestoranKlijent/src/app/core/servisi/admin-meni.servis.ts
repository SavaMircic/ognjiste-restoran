import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';

import { environment } from '../../../environments/environment';
import {
  KategorijaMenija,
  KategorijaMenijaUlaz,
  SlikaStavkeMenija,
  StavkaMenijaUlaz,
} from '../modeli/api.modeli';

interface KreiranaStavka {
  id: number;
}

interface PostavljenaSlika {
  slikaUrl: string;
}

@Injectable({ providedIn: 'root' })
export class AdminMeniServis {
  private readonly http = inject(HttpClient);
  private readonly osnova = `${environment.apiUrl}/administracija/meni`;

  kreirajKategoriju(ulaz: KategorijaMenijaUlaz): Observable<KategorijaMenija> {
    return this.http.post<KategorijaMenija>(`${this.osnova}/kategorije`, ulaz);
  }

  izmeniKategoriju(id: number, ulaz: KategorijaMenijaUlaz): Observable<void> {
    return this.http.put<void>(`${this.osnova}/kategorije/${id}`, ulaz);
  }

  obrisiKategoriju(id: number): Observable<void> {
    return this.http.delete<void>(`${this.osnova}/kategorije/${id}`);
  }

  kreirajStavku(ulaz: StavkaMenijaUlaz): Observable<KreiranaStavka> {
    return this.http.post<KreiranaStavka>(`${this.osnova}/stavke`, ulaz);
  }

  izmeniStavku(id: number, ulaz: StavkaMenijaUlaz): Observable<void> {
    return this.http.put<void>(`${this.osnova}/stavke/${id}`, ulaz);
  }

  promeniDostupnost(id: number, dostupno: boolean): Observable<void> {
    return this.http.patch<void>(`${this.osnova}/stavke/${id}/dostupnost`, { dostupno });
  }

  obrisiStavku(id: number): Observable<void> {
    return this.http.delete<void>(`${this.osnova}/stavke/${id}`);
  }

  postaviGlavnuSliku(id: number, slika: File): Observable<PostavljenaSlika> {
    const telo = new FormData();
    telo.append('slika', slika);
    return this.http.put<PostavljenaSlika>(`${this.osnova}/stavke/${id}/slika`, telo);
  }

  dodajDodatnuSliku(id: number, slika: File, opis?: string | null): Observable<SlikaStavkeMenija> {
    const telo = new FormData();
    telo.append('slika', slika);
    if (opis) telo.append('opis', opis);
    return this.http.post<SlikaStavkeMenija>(`${this.osnova}/stavke/${id}/slike`, telo);
  }

  listirajDodatneSlike(id: number): Observable<SlikaStavkeMenija[]> {
    return this.http.get<SlikaStavkeMenija[]>(`${this.osnova}/stavke/${id}/slike`);
  }

  obrisiDodatnuSliku(id: number, slikaId: number): Observable<void> {
    return this.http.delete<void>(`${this.osnova}/stavke/${id}/slike/${slikaId}`);
  }

  poredjajSlike(id: number, idRedom: number[]): Observable<void> {
    return this.http.put<void>(`${this.osnova}/stavke/${id}/slike/redosled`, { idRedom });
  }
}
