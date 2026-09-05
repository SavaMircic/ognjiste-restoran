import { Directive, ElementRef, inject } from '@angular/core';

import { environment } from '../../../environments/environment';

export const REZERVNA_SLIKA = `${environment.slikeUrl}/slike/jela/placeholder.svg`;

@Directive({
  selector: 'img[ogRezervna]',
  host: {
    '(error)': 'naGresku()',
  },
})
export class RezervnaSlika {
  private readonly element = inject<ElementRef<HTMLImageElement>>(ElementRef);
  private pokusano = false;

  protected naGresku(): void {
    if (this.pokusano) return;
    this.pokusano = true;

    const img = this.element.nativeElement;
    img.src = REZERVNA_SLIKA;
    img.classList.add('slika-rezervna');
  }
}
