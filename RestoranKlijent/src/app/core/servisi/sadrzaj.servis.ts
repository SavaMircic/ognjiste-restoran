import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable, shareReplay } from 'rxjs';

import { environment } from '../../../environments/environment';
import { ClanTima, GalerijaSlika, Postavke } from '../modeli/api.modeli';

@Injectable({ providedIn: 'root' })
export class SadrzajServis {
  private readonly http = inject(HttpClient);

  private readonly postavke$ = this.http
    .get<Postavke>(`${environment.apiUrl}/postavke`)
    .pipe(shareReplay({ bufferSize: 1, refCount: false }));

  postavke(): Observable<Postavke> {
    return this.postavke$;
  }

  galerija(): Observable<GalerijaSlika[]> {
    return this.http.get<GalerijaSlika[]>(`${environment.apiUrl}/galerija`);
  }

  nasTim(): Observable<ClanTima[]> {
    return this.http.get<ClanTima[]>(`${environment.apiUrl}/nas-tim`);
  }
}
