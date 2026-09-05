import { HttpParams } from '@angular/common/http';

export function uParametre(filter: object): HttpParams {
  let parametri = new HttpParams();

  for (const [kljuc, vrednost] of Object.entries(filter)) {
    if (vrednost !== null && vrednost !== undefined && vrednost !== '') {
      parametri = parametri.set(kljuc, String(vrednost));
    }
  }

  return parametri;
}
