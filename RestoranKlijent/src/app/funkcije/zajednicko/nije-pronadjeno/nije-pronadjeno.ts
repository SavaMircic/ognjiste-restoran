import { ChangeDetectionStrategy, Component } from '@angular/core';
import { RouterLink } from '@angular/router';

@Component({
  selector: 'og-nije-pronadjeno',
  imports: [RouterLink],
  changeDetection: ChangeDetectionStrategy.OnPush,
  templateUrl: './nije-pronadjeno.html',
  styleUrl: './nije-pronadjeno.scss',
})
export class NijePronadjeno {}
