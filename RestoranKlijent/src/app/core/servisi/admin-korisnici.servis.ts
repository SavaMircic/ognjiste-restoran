import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';

import { environment } from '../../../environments/environment';
import {
  BlokirajKorisnika,
  IzmenaZaposlenog,
  KorisnikAdmin,
  KorisnikDetalj,
  KorisniciPretraga,
  KreirajZaposlenog,
  OdgovorRecenzije,
  PaginiranaLista,
  PorukaOdgovor,
  ZabraniKomentarisanje,
  Zaposleni,
  ZaposleniPretraga,
} from '../modeli/api.modeli';
import { uParametre } from './upitni-parametri';

interface KreiranZaposleni {
  zaposleniId: number;
}

@Injectable({ providedIn: 'root' })
export class AdminKorisniciServis {
  private readonly http = inject(HttpClient);
  private readonly osnova = `${environment.apiUrl}/administracija`;

  pretrazi(filter: KorisniciPretraga): Observable<PaginiranaLista<KorisnikAdmin>> {
    return this.http.get<PaginiranaLista<KorisnikAdmin>>(`${this.osnova}/korisnici`, {
      params: uParametre(filter),
    });
  }

  detalj(id: string): Observable<KorisnikDetalj> {
    return this.http.get<KorisnikDetalj>(`${this.osnova}/korisnici/${id}`);
  }

  blokiraj(id: string, podaci: BlokirajKorisnika): Observable<PorukaOdgovor> {
    return this.http.put<PorukaOdgovor>(`${this.osnova}/korisnici/${id}/blokiraj`, podaci);
  }

  odblokiraj(id: string): Observable<PorukaOdgovor> {
    return this.http.put<PorukaOdgovor>(`${this.osnova}/korisnici/${id}/odblokiraj`, {});
  }

  zabraniKomentarisanje(id: string, podaci: ZabraniKomentarisanje): Observable<PorukaOdgovor> {
    return this.http.put<PorukaOdgovor>(
      `${this.osnova}/korisnici/${id}/zabrani-komentarisanje`,
      podaci,
    );
  }

  ukiniZabranuKomentarisanja(id: string): Observable<PorukaOdgovor> {
    return this.http.put<PorukaOdgovor>(
      `${this.osnova}/korisnici/${id}/ukini-zabranu-komentarisanja`,
      {},
    );
  }

  pretraziZaposlene(filter: ZaposleniPretraga): Observable<PaginiranaLista<Zaposleni>> {
    return this.http.get<PaginiranaLista<Zaposleni>>(`${this.osnova}/zaposleni`, {
      params: uParametre(filter),
    });
  }

  kreirajZaposlenog(podaci: KreirajZaposlenog): Observable<KreiranZaposleni> {
    return this.http.post<KreiranZaposleni>(`${this.osnova}/zaposleni`, podaci);
  }

  izmeniZaposlenog(id: number, podaci: IzmenaZaposlenog): Observable<void> {
    return this.http.put<void>(`${this.osnova}/zaposleni/${id}`, podaci);
  }

  deaktivirajZaposlenog(id: number): Observable<void> {
    return this.http.delete<void>(`${this.osnova}/zaposleni/${id}`);
  }

  odgovoriNaRecenziju(id: number, podaci: OdgovorRecenzije): Observable<void> {
    return this.http.put<void>(`${this.osnova}/recenzije/${id}/odgovor`, podaci);
  }

  obrisiRecenziju(id: number, razlog: string): Observable<void> {
    return this.http.delete<void>(`${this.osnova}/recenzije/${id}`, {
      params: uParametre({ razlog }),
    });
  }
}
