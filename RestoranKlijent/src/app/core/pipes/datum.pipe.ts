import { formatDate } from '@angular/common';
import { LOCALE_ID, Pipe, PipeTransform, inject } from '@angular/core';

import { izServera } from '../tekst/vreme';

@Pipe({ name: 'datumS' })
export class DatumCev implements PipeTransform {
  private readonly jezik = inject(LOCALE_ID) as string;

  transform(vrednost: string | null | undefined, format = "d. MMMM yyyy. 'u' HH:mm"): string {
    const datum = izServera(vrednost);
    return datum ? formatDate(datum, format, this.jezik) : '';
  }
}
