import { Pipe, PipeTransform } from '@angular/core';

import { environment } from '../../../environments/environment';

@Pipe({ name: 'slika' })
export class SlikaCev implements PipeTransform {
  transform(putanja: string | null | undefined): string {
    if (!putanja) return '';

    if (putanja.startsWith('http://') || putanja.startsWith('https://')) return putanja;

    return `${environment.slikeUrl}${putanja.startsWith('/') ? '' : '/'}${putanja}`;
  }
}
