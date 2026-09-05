import { Uloga } from '../modeli/api.modeli';

const NAZIVI: Record<Uloga, string> = {
  Korisnik: 'Korisnik',
  Konobar: 'Konobar',
  Sanker: 'Šanker',
  Kuvar: 'Kuvar',
  Administrator: 'Administrator',
  Menadzer: 'Menadžer',
};

export function nazivUloge(uloga: Uloga): string {
  return NAZIVI[uloga] ?? uloga;
}
