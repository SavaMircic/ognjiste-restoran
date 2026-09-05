import {
  AfterViewInit,
  ChangeDetectionStrategy,
  Component,
  ElementRef,
  input,
  output,
  viewChild,
} from '@angular/core';

export interface PotvrdaSadrzaj {
  naslov: string;
  tekst?: string;
  potvrdi: string;
  odustani: string;
  opasno: boolean;
}

@Component({
  selector: 'og-potvrda-dijalog',
  changeDetection: ChangeDetectionStrategy.OnPush,
  host: {
    '(document:keydown.escape)': 'odgovor.emit(false)',
  },
  templateUrl: './potvrda-dijalog.html',
  styleUrl: './potvrda-dijalog.scss',
})
export class PotvrdaDijalog implements AfterViewInit {
  readonly sadrzaj = input.required<PotvrdaSadrzaj>();

  readonly odgovor = output<boolean>();

  private readonly dugmePotvrde = viewChild<ElementRef<HTMLButtonElement>>('potvrdi');

  ngAfterViewInit(): void {
    this.dugmePotvrde()?.nativeElement.focus();
  }
}
