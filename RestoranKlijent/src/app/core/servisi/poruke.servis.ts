import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';

import { environment } from '../../../environments/environment';
import { PosaljiPoruku } from '../modeli/api.modeli';

@Injectable({ providedIn: 'root' })
export class PorukeServis {
  private readonly http = inject(HttpClient);

  posalji(poruka: PosaljiPoruku): Observable<unknown> {
    return this.http.post(`${environment.apiUrl}/poruke`, poruka);
  }
}
