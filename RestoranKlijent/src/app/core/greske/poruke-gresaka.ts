import { HttpErrorResponse } from '@angular/common/http';

import { ApiGreska } from '../modeli/api.modeli';

const IDENTITY_PREVODI: Record<string, string> = {
  'Invalid token.': 'Link je istekao ili je već iskorišćen.',
  'Incorrect password.': 'Trenutna lozinka nije tačna.',
  'Passwords must be at least 8 characters.': 'Lozinka mora imati bar 8 karaktera.',
  "Passwords must have at least one uppercase ('A'-'Z').": 'Lozinka mora sadržati bar jedno veliko slovo.',
  "Passwords must have at least one digit ('0'-'9').": 'Lozinka mora sadržati bar jedan broj.',
  'Passwords must have at least one non alphanumeric character.':
    'Lozinka mora sadržati bar jedan specijalan znak.',
};

function prevediIdentity(poruka: string): string {
  const delovi = poruka
    .split(/(?<=\.)\s+/)
    .map((d) => d.trim())
    .filter(Boolean);

  if (!delovi.length) return poruka;

  return delovi.map((d) => IDENTITY_PREVODI[d] ?? d).join(' ');
}

export function porukaGreske(greska: unknown, rezerva = 'Došlo je do greške. Pokušajte ponovo.'): string {
  if (!(greska instanceof HttpErrorResponse)) return rezerva;

  if (greska.status === 0) {
    return 'Server nije dostupan. Proverite da li je API pokrenut.';
  }

  if (greska.status >= 200 && greska.status < 300) {
    return 'Server je vratio odgovor koji nije JSON. Proverite `apiUrl` u environment fajlu.';
  }

  const telo = greska.error as Partial<ApiGreska> | string | null;

  if (typeof telo === 'string' && telo.trim()) return prevediIdentity(telo);

  if (telo && typeof telo === 'object') {
    const opsta = telo.greske?.[''];
    if (opsta?.length) return opsta[0];

    const prvoPolje = Object.values(telo.greske ?? {}).find((p) => p?.length);
    if (prvoPolje?.length) return prvoPolje[0];

    if (telo.poruka) return prevediIdentity(telo.poruka);
  }

  if (greska.status === 401) return 'Niste prijavljeni.';
  if (greska.status === 403) return 'Nemate pravo na ovu radnju.';
  if (greska.status === 404) return 'Traženi podatak ne postoji.';
  if (greska.status === 429) return 'Previše zahteva. Pokušajte ponovo za nekoliko minuta.';

  return rezerva;
}

export function greskePoPolju(greska: unknown): Record<string, string[]> {
  if (!(greska instanceof HttpErrorResponse)) return {};
  const telo = greska.error as Partial<ApiGreska> | null;
  return telo && typeof telo === 'object' ? (telo.greske ?? {}) : {};
}
