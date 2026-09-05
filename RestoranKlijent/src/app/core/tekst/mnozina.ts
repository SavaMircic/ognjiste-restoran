export function mnozina(broj: number, jedan: string, dvaDoCetiri: string, pet: string): string {
  const poslednjeDve = Math.abs(broj) % 100;
  const poslednja = poslednjeDve % 10;

  if (poslednjeDve >= 11 && poslednjeDve <= 14) return pet;
  if (poslednja === 1) return jedan;
  if (poslednja >= 2 && poslednja <= 4) return dvaDoCetiri;
  return pet;
}

export function saBrojem(broj: number, jedan: string, dvaDoCetiri: string, pet: string): string {
  return `${broj} ${mnozina(broj, jedan, dvaDoCetiri, pet)}`;
}
