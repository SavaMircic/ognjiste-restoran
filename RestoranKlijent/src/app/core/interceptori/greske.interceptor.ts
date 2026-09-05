import { HttpErrorResponse, HttpInterceptorFn } from '@angular/common/http';
import { catchError, throwError } from 'rxjs';

import { environment } from '../../../environments/environment';
import { porukaGreske } from '../greske/poruke-gresaka';

export const greskeInterceptor: HttpInterceptorFn = (zahtev, dalje) =>
  dalje(zahtev).pipe(
    catchError((greska: unknown) => {
      if (!environment.produkcija) {
        const status = greska instanceof HttpErrorResponse ? greska.status : '—';
        console.error(
          `[HTTP ${status}] ${zahtev.method} ${zahtev.urlWithParams}\n  → ${porukaGreske(greska)}`,
          greska,
        );
      }
      return throwError(() => greska);
    }),
  );
