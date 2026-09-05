import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';

import { environment } from '../../../environments/environment';
import {
  IzmenaProfila,
  KorisnikProfil,
  Poruka,
  PromenaLozinke,
  StavkaMenija,
} from '../modeli/api.modeli';

@Injectable({ providedIn: 'root' })
export class ProfilServis {
  private readonly http = inject(HttpClient);
  private readonly osnova = `${environment.apiUrl}/korisnici`;

  profil(): Observable<KorisnikProfil> {
    return this.http.get<KorisnikProfil>(`${this.osnova}/profil`);
  }

  izmeniProfil(podaci: IzmenaProfila): Observable<void> {
    return this.http.put<void>(`${this.osnova}/profil`, podaci);
  }

  promeniLozinku(podaci: PromenaLozinke): Observable<void> {
    return this.http.put<void>(`${this.osnova}/promeni-lozinku`, podaci);
  }

  postaviSliku(datoteka: File): Observable<{ slikaUrl: string }> {
    const telo = new FormData();
    telo.append('slika', datoteka);
    return this.http.put<{ slikaUrl: string }>(`${this.osnova}/profil/slika`, telo);
  }

  omiljenaJela(): Observable<StavkaMenija[]> {
    return this.http.get<StavkaMenija[]>(`${this.osnova}/omiljena-jela`);
  }

  mojePoruke(): Observable<Poruka[]> {
    return this.http.get<Poruka[]>(`${this.osnova}/poruke`);
  }
}
