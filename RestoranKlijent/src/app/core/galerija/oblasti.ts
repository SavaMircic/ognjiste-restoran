import { GalerijaOblast, GalerijaSlika } from '../modeli/api.modeli';

export function uOblasti(slike: GalerijaSlika[]): GalerijaOblast[] {
  const mapa = new Map<string, GalerijaSlika[]>();

  for (const slika of slike) {
    const oblast = slika.grupa?.trim() || 'Ostalo';
    const postojece = mapa.get(oblast);
    if (postojece) postojece.push(slika);
    else mapa.set(oblast, [slika]);
  }

  return [...mapa.entries()]
    .map(([naziv, sve]) => {
      const poredjane = [...sve].sort((a, b) => a.redosled - b.redosled);
      return { naziv, naslovna: poredjane[0], slike: poredjane };
    })
    .sort((a, b) => a.naslovna.redosled - b.naslovna.redosled);
}
