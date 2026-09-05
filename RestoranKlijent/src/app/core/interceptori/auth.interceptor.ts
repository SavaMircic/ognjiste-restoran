import { HttpErrorResponse, HttpHandlerFn, HttpInterceptorFn, HttpRequest } from '@angular/common/http';
import { inject } from '@angular/core';
import { Router } from '@angular/router';
import { BehaviorSubject, Observable, catchError, filter, switchMap, take, throwError } from 'rxjs';

import { environment } from '../../../environments/environment';
import { jeAuthStrana } from '../guards/gost.guard';
import { AuthServis } from '../servisi/auth.servis';

let osvezavanjeUToku = false;
const noviToken$ = new BehaviorSubject<string | null>(null);

const BEZ_OSVEZAVANJA = ['/auth/prijava', '/auth/osvezi-token', '/auth/registracija'];

export const authInterceptor: HttpInterceptorFn = (zahtev, dalje) => {
  const auth = inject(AuthServis);
  const router = inject(Router);

  const nasApi = zahtev.url.startsWith(environment.apiUrl);
  const token = auth.accessToken;

  const poslati = nasApi && token ? saTokenom(zahtev, token) : zahtev;

  return dalje(poslati).pipe(
    catchError((greska: unknown) => {
      const jeIstekaoToken =
        greska instanceof HttpErrorResponse &&
        greska.status === 401 &&
        nasApi &&
        !!auth.refreshToken &&
        !BEZ_OSVEZAVANJA.some((p) => zahtev.url.includes(p));

      if (!jeIstekaoToken) return throwError(() => greska);

      return osveziPaPonovi(zahtev, dalje, auth, router);
    }),
  );
};

function saTokenom(zahtev: HttpRequest<unknown>, token: string): HttpRequest<unknown> {
  return zahtev.clone({ setHeaders: { Authorization: `Bearer ${token}` } });
}

function osveziPaPonovi(
  zahtev: HttpRequest<unknown>,
  dalje: HttpHandlerFn,
  auth: AuthServis,
  router: Router,
): Observable<any> {
  if (osvezavanjeUToku) {
    return noviToken$.pipe(
      filter((t): t is string => t !== null),
      take(1),
      switchMap((t) => dalje(saTokenom(zahtev, t))),
    );
  }

  osvezavanjeUToku = true;
  noviToken$.next(null);

  return auth.osveziToken().pipe(
    switchMap((odgovor) => {
      osvezavanjeUToku = false;
      noviToken$.next(odgovor.accessToken);
      return dalje(saTokenom(zahtev, odgovor.accessToken));
    }),
    catchError((greska) => {
      osvezavanjeUToku = false;
      auth.odjava();

      const povratak = jeAuthStrana(router.url) ? {} : { povratak: router.url };
      void router.navigate(['/prijava'], { queryParams: povratak });
      return throwError(() => greska);
    }),
  );
}
