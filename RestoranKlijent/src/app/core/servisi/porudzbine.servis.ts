import { HttpClient } from '@angular/common/http';
import { Injectable, inject, signal } from '@angular/core';
import { Observable, tap } from 'rxjs';

import { environment } from '../../../environments/environment';
import {
  DodajStavke,
  OtvoriPorudzbinu,
  PorudzbinaAktivna,
  PorudzbinaDetalj,
  Sto,
  ZatvoriPorudzbinu,
} from '../modeli/api.modeli';

interface OtvorenaPorudzbina {
  porudzbinaId: number;
}

const KLJUC_ISPORUCENO = 'ognjiste.isporucene-stavke';

@Injectable({ providedIn: 'root' })
export class PorudzbineServis {
  private readonly http = inject(HttpClient);
  private readonly osnova = `${environment.apiUrl}/porudzbine`;

  private readonly _isporucene = signal<Record<number, number[]>>(procitajIsporucene());
  readonly isporucene = this._isporucene.asReadonly();

  stolovi(): Observable<Sto[]> {
    return this.http.get<Sto[]>(`${environment.apiUrl}/stolovi`);
  }

  aktivne(): Observable<PorudzbinaAktivna[]> {
    return this.http.get<PorudzbinaAktivna[]>(`${this.osnova}/aktivne`);
  }

  detalj(id: number): Observable<PorudzbinaDetalj> {
    return this.http.get<PorudzbinaDetalj>(`${this.osnova}/${id}`);
  }

  otvori(podaci: OtvoriPorudzbinu): Observable<OtvorenaPorudzbina> {
    return this.http.post<OtvorenaPorudzbina>(this.osnova, podaci);
  }

  dodajStavke(id: number, podaci: DodajStavke): Observable<unknown> {
    return this.http.post(`${this.osnova}/${id}/stavke`, podaci);
  }

  otkaziStavku(id: number, stavkaId: number): Observable<void> {
    return this.http.delete<void>(`${this.osnova}/${id}/stavke/${stavkaId}`);
  }

  zatvori(id: number, podaci: ZatvoriPorudzbinu): Observable<void> {
    return this.http.patch<void>(`${this.osnova}/${id}/zatvori`, podaci).pipe(
      tap(() => this.zaboraviPorudzbinu(id)),
    );
  }

  racun(id: number): Observable<Blob> {
    return this.http.get(`${this.osnova}/${id}/racun`, { responseType: 'blob' });
  }

  isporuciStavku(porudzbinaId: number, stavkaId: number): Observable<void> {
    return this.http
      .patch<void>(`${environment.apiUrl}/stavke-porudzbine/${stavkaId}/isporuci`, {})
      .pipe(tap(() => this.zapamtiIsporuku(porudzbinaId, stavkaId)));
  }

  jeIsporucena(porudzbinaId: number, stavkaId: number): boolean {
    return (this._isporucene()[porudzbinaId] ?? []).includes(stavkaId);
  }

  private zapamtiIsporuku(porudzbinaId: number, stavkaId: number): void {
    this._isporucene.update((trenutno) => {
      const zaSto = trenutno[porudzbinaId] ?? [];
      if (zaSto.includes(stavkaId)) return trenutno;

      const novo = { ...trenutno, [porudzbinaId]: [...zaSto, stavkaId] };
      sacuvajIsporucene(novo);
      return novo;
    });
  }

  private zaboraviPorudzbinu(porudzbinaId: number): void {
    this._isporucene.update((trenutno) => {
      if (!(porudzbinaId in trenutno)) return trenutno;

      const novo = { ...trenutno };
      delete novo[porudzbinaId];
      sacuvajIsporucene(novo);
      return novo;
    });
  }
}

function procitajIsporucene(): Record<number, number[]> {
  try {
    const sirovo = localStorage.getItem(KLJUC_ISPORUCENO);
    if (!sirovo) return {};

    const zapis = JSON.parse(sirovo) as Record<number, number[]>;
    return zapis && typeof zapis === 'object' ? zapis : {};
  } catch {
    return {};
  }
}

function sacuvajIsporucene(zapis: Record<number, number[]>): void {
  try {
    localStorage.setItem(KLJUC_ISPORUCENO, JSON.stringify(zapis));
  } catch {
  }
}
