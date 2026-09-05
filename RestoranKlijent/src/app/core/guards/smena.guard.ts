import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';

import { pocetnaZaUlogu } from '../navigacija';
import { AuthServis } from '../servisi/auth.servis';

export const samoGosti: CanActivateFn = () => {
  const auth = inject(AuthServis);
  const router = inject(Router);

  if (!auth.radniRezim()) return true;
  return router.createUrlTree([pocetnaZaUlogu(auth.uloge())]);
};
