import { HttpClient } from '@angular/common/http';
import { Injectable, computed, inject, signal } from '@angular/core';
import { Observable, tap } from 'rxjs';

import { environment } from '../../../environments/environment';
import {
  EmailZahtev,
  PorukaOdgovor,
  PotvrdaEmailZahtev,
  PrijavaZahtev,
  RegistracijaZahtev,
  ResetLozinkeZahtev,
  TokenOdgovor,
  ULOGE_OSOBLJA,
  Uloga,
} from '../modeli/api.modeli';

const KLJUC_SESIJE = 'ognjiste.sesija';

@Injectable({ providedIn: 'root' })
export class AuthServis {
  private readonly http = inject(HttpClient);

  private readonly _sesija = signal<TokenOdgovor | null>(procitajSesiju());

  readonly sesija = this._sesija.asReadonly();
  readonly prijavljen = computed(() => this._sesija() !== null);
  readonly uloge = computed<Uloga[]>(() => this._sesija()?.uloge ?? []);
  readonly punoIme = computed(() => {
    const s = this._sesija();
    return s ? `${s.ime} ${s.prezime}` : '';
  });

  readonly radniRezim = computed(() => this.imaUlogu(ULOGE_OSOBLJA));

  imaUlogu(trazene: Uloga[]): boolean {
    if (trazene.length === 0) return this.prijavljen();
    const moje = this.uloge();
    return trazene.some((u) => moje.includes(u));
  }

  get accessToken(): string | null {
    return this._sesija()?.accessToken ?? null;
  }

  get refreshToken(): string | null {
    return this._sesija()?.refreshToken ?? null;
  }

  prijava(podaci: PrijavaZahtev): Observable<TokenOdgovor> {
    return this.http
      .post<TokenOdgovor>(`${environment.apiUrl}/auth/prijava`, podaci)
      .pipe(tap((odgovor) => this.zapamti(odgovor)));
  }

  osveziToken(): Observable<TokenOdgovor> {
    return this.http
      .post<TokenOdgovor>(`${environment.apiUrl}/auth/osvezi-token`, {
        accessToken: this.accessToken ?? '',
        refreshToken: this.refreshToken ?? '',
      })
      .pipe(tap((odgovor) => this.zapamti(odgovor)));
  }

  registracija(podaci: RegistracijaZahtev): Observable<PorukaOdgovor> {
    return this.http.post<PorukaOdgovor>(`${environment.apiUrl}/auth/registracija`, podaci);
  }

  potvrdaEmaila(podaci: PotvrdaEmailZahtev): Observable<PorukaOdgovor> {
    return this.http.post<PorukaOdgovor>(`${environment.apiUrl}/auth/potvrda-email`, podaci);
  }

  posaljiPonovoPotvrdu(podaci: EmailZahtev): Observable<PorukaOdgovor> {
    return this.http.post<PorukaOdgovor>(`${environment.apiUrl}/auth/posalji-ponovo-potvrdu`, podaci);
  }

  zaboravljenaLozinka(podaci: EmailZahtev): Observable<PorukaOdgovor> {
    return this.http.post<PorukaOdgovor>(`${environment.apiUrl}/auth/zaboravljena-lozinka`, podaci);
  }

  resetujLozinku(podaci: ResetLozinkeZahtev): Observable<PorukaOdgovor> {
    return this.http.post<PorukaOdgovor>(`${environment.apiUrl}/auth/resetuj-lozinku`, podaci);
  }

  odjava(): void {
    this._sesija.set(null);
    localStorage.removeItem(KLJUC_SESIJE);
  }

  private zapamti(odgovor: TokenOdgovor): void {
    this._sesija.set(odgovor);
    localStorage.setItem(KLJUC_SESIJE, JSON.stringify(odgovor));
  }
}

function procitajSesiju(): TokenOdgovor | null {
  try {
    const sirovo = localStorage.getItem(KLJUC_SESIJE);
    if (!sirovo) return null;

    const sesija = JSON.parse(sirovo) as TokenOdgovor;
    return sesija?.accessToken && Array.isArray(sesija.uloge) ? sesija : null;
  } catch {
    return null;
  }
}
