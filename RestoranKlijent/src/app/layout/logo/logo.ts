import { ChangeDetectionStrategy, Component, input } from '@angular/core';

@Component({
  selector: 'og-logo',
  changeDetection: ChangeDetectionStrategy.OnPush,
  templateUrl: './logo.html',
  styleUrl: './logo.scss',
})
export class Logo {
  readonly velicina = input(40);
}
