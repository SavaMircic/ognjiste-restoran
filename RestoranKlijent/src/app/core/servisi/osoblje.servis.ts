import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';

import { environment } from '../../../environments/environment';
import {
  Bonus,
  IstorijaDan,
  PrijavaProblema,
  PrijaviProblem,
  Smena,
  UcinakZaposlenog,
} from '../modeli/api.modeli';
import { uParametre } from './upitni-parametri';

@Injectable({ providedIn: 'root' })
export class OsobljeServis {
  private readonly http = inject(HttpClient);

  mojRaspored(nedelja?: string | null): Observable<Smena[]> {
    return this.http.get<Smena[]>(`${environment.apiUrl}/raspored/moj`, {
      params: uParametre({ nedelja }),
    });
  }

  mojaStatistika(datumOd: string, datumDo: string): Observable<UcinakZaposlenog> {
    return this.http.get<UcinakZaposlenog>(`${environment.apiUrl}/korisnici/statistika`, {
      params: uParametre({ datumOd, datumDo }),
    });
  }

  mojaIstorija(datumOd: string, datumDo: string): Observable<IstorijaDan[]> {
    return this.http.get<IstorijaDan[]>(`${environment.apiUrl}/korisnici/istorija`, {
      params: uParametre({ datumOd, datumDo }),
    });
  }

  mojiBonusi(): Observable<Bonus[]> {
    return this.http.get<Bonus[]>(`${environment.apiUrl}/korisnici/bonusi`);
  }

  mojePrijave(): Observable<PrijavaProblema[]> {
    return this.http.get<PrijavaProblema[]>(`${environment.apiUrl}/prijave-problema/moje`);
  }

  prijaviProblem(prijava: PrijaviProblem): Observable<PrijavaProblema> {
    return this.http.post<PrijavaProblema>(`${environment.apiUrl}/prijave-problema`, prijava);
  }
}
