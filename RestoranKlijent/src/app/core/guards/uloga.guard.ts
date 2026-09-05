import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';

import { Uloga } from '../modeli/api.modeli';
import { AuthServis } from '../servisi/auth.servis';

export function zahtevaUlogu(...uloge: Uloga[]): CanActivateFn {
  return (_ruta, stanje) => {
    const auth = inject(AuthServis);
    const router = inject(Router);

    if (!auth.prijavljen()) {
      return router.createUrlTree(['/prijava'], { queryParams: { povratak: stanje.url } });
    }

    if (auth.imaUlogu(uloge)) return true;

    return router.createUrlTree(['/nemate-pristup']);
  };
}
