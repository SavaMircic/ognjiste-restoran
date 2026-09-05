import { Injectable, effect, inject, signal } from '@angular/core';
import { HubConnection, HubConnectionBuilder, HubConnectionState, LogLevel } from '@microsoft/signalr';
import { Subject } from 'rxjs';

import { environment } from '../../../environments/environment';
import { NovaStavkaNotifikacija, StavkaSpremnaNotifikacija } from '../modeli/api.modeli';
import { AuthServis } from './auth.servis';

export type StanjeVeze = 'nepovezano' | 'povezivanje' | 'povezano' | 'prekinuto';

@Injectable({ providedIn: 'root' })
export class KuhinjaVezaServis {
  private readonly auth = inject(AuthServis);

  private veza: HubConnection | null = null;

  private readonly _stanje = signal<StanjeVeze>('nepovezano');
  readonly stanje = this._stanje.asReadonly();

  readonly stavkaSpremna = new Subject<StavkaSpremnaNotifikacija>();

  readonly novaStavka = new Subject<NovaStavkaNotifikacija>();

  constructor() {
    effect(() => {
      if (!this.auth.prijavljen()) void this.prekini();
    });
  }

  async povezi(): Promise<void> {
    if (this.veza) return;

    const veza = new HubConnectionBuilder()
      .withUrl(`${environment.slikeUrl}/hubs/kuhinja`, {
        accessTokenFactory: () => this.auth.accessToken ?? '',
      })
      .withAutomaticReconnect()
      .configureLogging(environment.produkcija ? LogLevel.Error : LogLevel.Information)
      .build();

    veza.onreconnecting(() => this._stanje.set('povezivanje'));
    veza.onreconnected(() => this._stanje.set('povezano'));
    veza.onclose(() => this._stanje.set('prekinuto'));

    veza.on('StavkaSpremna', (podaci: StavkaSpremnaNotifikacija) => this.stavkaSpremna.next(podaci));
    veza.on('NovaStavka', (podaci: NovaStavkaNotifikacija) => this.novaStavka.next(podaci));

    this.veza = veza;
    this._stanje.set('povezivanje');

    try {
      await veza.start();
      this._stanje.set('povezano');
    } catch {
      this._stanje.set('prekinuto');
      this.veza = null;
    }
  }

  async prekini(): Promise<void> {
    const veza = this.veza;
    this.veza = null;
    this._stanje.set('nepovezano');

    if (veza && veza.state !== HubConnectionState.Disconnected) {
      await veza.stop();
    }
  }
}
