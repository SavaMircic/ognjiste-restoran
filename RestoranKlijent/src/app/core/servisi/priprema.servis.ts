import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';

import { environment } from '../../../environments/environment';
import { Odrediste, RedCekanjaStavka } from '../modeli/api.modeli';

const PUTANJA: Record<Odrediste, string> = {
  Kuhinja: 'kuhinja',
  Sank: 'sank',
};

@Injectable({ providedIn: 'root' })
export class PripremaServis {
  private readonly http = inject(HttpClient);

  redCekanja(odrediste: Odrediste): Observable<RedCekanjaStavka[]> {
    return this.http.get<RedCekanjaStavka[]>(
      `${environment.apiUrl}/${PUTANJA[odrediste]}/red-cekanja`,
    );
  }

  preuzmi(stavkaId: number): Observable<void> {
    return this.http.patch<void>(`${environment.apiUrl}/stavke-porudzbine/${stavkaId}/preuzmi`, {});
  }

  zavrsi(stavkaId: number): Observable<void> {
    return this.http.patch<void>(`${environment.apiUrl}/stavke-porudzbine/${stavkaId}/zavrsi`, {});
  }
}
