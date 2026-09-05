import { Uloga } from './modeli/api.modeli';

export interface AlatPozicije {
  uloga: Uloga;
  putanja: string;
  naziv: string;
}

export const ALATI_POZICIJE: AlatPozicije[] = [
  { uloga: 'Konobar', putanja: '/konobar', naziv: 'Konobarska tabla' },
  { uloga: 'Kuvar', putanja: '/kuhinja', naziv: 'Kuhinja' },
  { uloga: 'Sanker', putanja: '/sank', naziv: 'Šank' },
  { uloga: 'Administrator', putanja: '/administracija', naziv: 'Administracija' },
  { uloga: 'Menadzer', putanja: '/menadzment', naziv: 'Menadžment' },
];

export function alatiZa(uloge: Uloga[]): AlatPozicije[] {
  return ALATI_POZICIJE.filter((a) => uloge.includes(a.uloga));
}

export function pocetnaZaUlogu(uloge: Uloga[]): string {
  return ALATI_POZICIJE.find((a) => uloge.includes(a.uloga))?.putanja ?? '/osoblje';
}
