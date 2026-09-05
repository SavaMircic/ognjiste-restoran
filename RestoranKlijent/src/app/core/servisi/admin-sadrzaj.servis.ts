import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';

import { environment } from '../../../environments/environment';
import {
  AdminAkcija,
  AdminAkcijePretraga,
  AuditLog,
  AuditLogPretraga,
  GalerijaSlika,
  IzmenaGalerijeSlike,
  IzmenaPostavki,
  IzmenaProfilaNaSajtu,
  KreirajAdminRezervaciju,
  OdgovorNaPoruku,
  PaginiranaLista,
  Poruka,
  PorukePretraga,
  Postavke,
  Rezervacija,
  RezervacijePretraga,
} from '../modeli/api.modeli';
import { uParametre } from './upitni-parametri';

interface PostavljenaSlika {
  slikaUrl: string;
}

@Injectable({ providedIn: 'root' })
export class AdminSadrzajServis {
  private readonly http = inject(HttpClient);
  private readonly osnova = `${environment.apiUrl}/administracija`;

  pretraziRezervacije(filter: RezervacijePretraga): Observable<PaginiranaLista<Rezervacija>> {
    return this.http.get<PaginiranaLista<Rezervacija>>(`${this.osnova}/rezervacije`, {
      params: uParametre(filter),
    });
  }

  kreirajRezervaciju(podaci: KreirajAdminRezervaciju): Observable<Rezervacija> {
    return this.http.post<Rezervacija>(`${this.osnova}/rezervacije`, podaci);
  }

  pretraziPoruke(filter: PorukePretraga): Observable<PaginiranaLista<Poruka>> {
    return this.http.get<PaginiranaLista<Poruka>>(`${this.osnova}/poruke`, {
      params: uParametre(filter),
    });
  }

  odgovoriNaPoruku(id: number, podaci: OdgovorNaPoruku): Observable<void> {
    return this.http.put<void>(`${this.osnova}/poruke/${id}/odgovor`, podaci);
  }

  oznaciProcitanom(id: number): Observable<void> {
    return this.http.patch<void>(`${this.osnova}/poruke/${id}/procitano`, {});
  }

  galerija(): Observable<GalerijaSlika[]> {
    return this.http.get<GalerijaSlika[]>(`${this.osnova}/galerija`);
  }

  dodajUGaleriju(
    slika: File,
    naslov: string,
    grupa: string,
    opis?: string | null,
  ): Observable<GalerijaSlika> {
    const telo = new FormData();
    telo.append('slika', slika);
    telo.append('naslov', naslov);
    telo.append('grupa', grupa);
    if (opis) telo.append('opis', opis);
    return this.http.post<GalerijaSlika>(`${this.osnova}/galerija`, telo);
  }

  izmeniSlikuGalerije(id: number, podaci: IzmenaGalerijeSlike): Observable<void> {
    return this.http.put<void>(`${this.osnova}/galerija/${id}`, podaci);
  }

  obrisiSlikuGalerije(id: number): Observable<void> {
    return this.http.delete<void>(`${this.osnova}/galerija/${id}`);
  }

  izmeniProfilNaSajtu(zaposleniId: number, podaci: IzmenaProfilaNaSajtu): Observable<void> {
    return this.http.put<void>(`${this.osnova}/zaposleni/${zaposleniId}/profil-sajta`, podaci);
  }

  postaviSlikuZaposlenog(zaposleniId: number, slika: File): Observable<PostavljenaSlika> {
    const telo = new FormData();
    telo.append('slika', slika);
    return this.http.put<PostavljenaSlika>(`${this.osnova}/zaposleni/${zaposleniId}/slika`, telo);
  }

  postavke(): Observable<Postavke> {
    return this.http.get<Postavke>(`${environment.apiUrl}/postavke`);
  }

  izmeniPostavke(podaci: IzmenaPostavki): Observable<Postavke> {
    return this.http.put<Postavke>(`${this.osnova}/postavke`, podaci);
  }

  auditLog(filter: AuditLogPretraga): Observable<PaginiranaLista<AuditLog>> {
    return this.http.get<PaginiranaLista<AuditLog>>(`${this.osnova}/audit-log`, {
      params: uParametre(filter),
    });
  }

  adminAkcije(filter: AdminAkcijePretraga): Observable<PaginiranaLista<AdminAkcija>> {
    return this.http.get<PaginiranaLista<AdminAkcija>>(`${this.osnova}/admin-akcije`, {
      params: uParametre(filter),
    });
  }
}
