function imaZonu(s: string): boolean {
  return /(?:Z|[+-]\d{2}:?\d{2})$/.test(s);
}

function jeSamoDatum(s: string): boolean {
  return /^\d{4}-\d{2}-\d{2}$/.test(s);
}

export function izServera(s: string | null | undefined): Date | null {
  if (!s) return null;

  if (jeSamoDatum(s)) {
    const [g, m, d] = s.split('-').map(Number);
    return new Date(g, m - 1, d);
  }

  const datum = new Date(imaZonu(s) ? s : `${s}Z`);
  return Number.isNaN(datum.getTime()) ? null : datum;
}

export function uUtcIso(lokalno: string): string {
  return new Date(lokalno).toISOString();
}

export function zaDatetimeLocal(datum: Date): string {
  const p = (n: number) => String(n).padStart(2, '0');
  return (
    `${datum.getFullYear()}-${p(datum.getMonth() + 1)}-${p(datum.getDate())}` +
    `T${p(datum.getHours())}:${p(datum.getMinutes())}`
  );
}

export function zaSatVremena(): string {
  return zaDatetimeLocal(new Date(Date.now() + 60 * 60 * 1000));
}
