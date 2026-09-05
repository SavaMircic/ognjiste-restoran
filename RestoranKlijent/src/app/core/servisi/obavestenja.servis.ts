import { Injectable, inject } from '@angular/core';
import { MatSnackBar } from '@angular/material/snack-bar';

import { porukaGreske } from '../greske/poruke-gresaka';

@Injectable({ providedIn: 'root' })
export class ObavestenjaServis {
  private readonly snackBar = inject(MatSnackBar);

  uspeh(poruka: string): void {
    this.prikazi(poruka, 'obavestenje-uspeh', 3500);
  }

  greska(greska: unknown, rezerva?: string): void {
    this.prikazi(porukaGreske(greska, rezerva), 'obavestenje-greska', 6000);
  }

  info(poruka: string): void {
    this.prikazi(poruka, 'obavestenje-info', 4000);
  }

  private prikazi(poruka: string, klasa: string, trajanje: number): void {
    this.snackBar.open(poruka, 'U redu', {
      duration: trajanje,
      panelClass: klasa,
      horizontalPosition: 'center',
      verticalPosition: 'bottom',
    });
  }
}
