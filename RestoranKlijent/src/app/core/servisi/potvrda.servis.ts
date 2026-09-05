import { ApplicationRef, EnvironmentInjector, Injectable, createComponent, inject } from '@angular/core';

import { PotvrdaDijalog, PotvrdaSadrzaj } from '../../funkcije/zajednicko/potvrda-dijalog/potvrda-dijalog';

export interface PotvrdaOpcije {
  naslov: string;
  tekst?: string;
  potvrdi?: string;
  odustani?: string;
  opasno?: boolean;
}

@Injectable({ providedIn: 'root' })
export class PotvrdaServis {
  private readonly app = inject(ApplicationRef);
  private readonly injektor = inject(EnvironmentInjector);

  pitaj(opcije: PotvrdaOpcije): Promise<boolean> {
    const sadrzaj: PotvrdaSadrzaj = {
      naslov: opcije.naslov,
      tekst: opcije.tekst,
      potvrdi: opcije.potvrdi ?? 'Potvrdi',
      odustani: opcije.odustani ?? 'Odustani',
      opasno: opcije.opasno ?? true,
    };

    const domacin = document.createElement('div');
    document.body.appendChild(domacin);

    const komponenta = createComponent(PotvrdaDijalog, {
      environmentInjector: this.injektor,
      hostElement: domacin,
    });
    komponenta.setInput('sadrzaj', sadrzaj);

    this.app.attachView(komponenta.hostView);

    const stariOverflow = document.body.style.overflow;
    document.body.style.overflow = 'hidden';

    return new Promise<boolean>((resolve) => {
      const pretplata = komponenta.instance.odgovor.subscribe((odgovor: boolean) => {
        pretplata.unsubscribe();
        this.app.detachView(komponenta.hostView);
        komponenta.destroy();
        domacin.remove();
        document.body.style.overflow = stariOverflow;
        resolve(odgovor);
      });
    });
  }
}
