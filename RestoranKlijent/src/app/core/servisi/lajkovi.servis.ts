import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';

import { environment } from '../../../environments/environment';

@Injectable({ providedIn: 'root' })
export class LajkoviServis {
  private readonly http = inject(HttpClient);

  lajkuj(stavkaMenijaId: number): Observable<unknown> {
    return this.http.post(`${environment.apiUrl}/jela/${stavkaMenijaId}/lajk`, {});
  }

  ukloni(stavkaMenijaId: number): Observable<void> {
    return this.http.delete<void>(`${environment.apiUrl}/jela/${stavkaMenijaId}/lajk`);
  }
}
