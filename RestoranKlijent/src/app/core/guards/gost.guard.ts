import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';

import { AuthServis } from '../servisi/auth.servis';

export const AUTH_STRANE = ['/prijava', '/registracija', '/zaboravljena-lozinka', '/posalji-ponovo-potvrdu'];

export function jeAuthStrana(url: string): boolean {
  return AUTH_STRANE.includes(url.split(/[?#]/)[0]);
}

export const samoNeprijavljeni: CanActivateFn = () => {
  const auth = inject(AuthServis);
  const router = inject(Router);

  return auth.prijavljen() ? router.createUrlTree(['/']) : true;
};
