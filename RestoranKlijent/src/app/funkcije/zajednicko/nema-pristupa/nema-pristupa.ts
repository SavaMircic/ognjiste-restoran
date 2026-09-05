import { ChangeDetectionStrategy, Component, inject } from '@angular/core';
import { RouterLink } from '@angular/router';

import { AuthServis } from '../../../core/servisi/auth.servis';

@Component({
  selector: 'og-nema-pristupa',
  imports: [RouterLink],
  changeDetection: ChangeDetectionStrategy.OnPush,
  templateUrl: './nema-pristupa.html',
  styleUrl: './nema-pristupa.scss',
})
export class NemaPristupa {
  protected readonly auth = inject(AuthServis);
}
