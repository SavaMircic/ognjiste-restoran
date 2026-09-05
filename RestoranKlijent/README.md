# Ognjište — Angular klijent

Frontend aplikacije za upravljanje restoranom „Ognjište · Novi Sad". Angular 21, standalone
komponente, signali, `OnPush` i **zoneless** promena stanja; rute se učitavaju lenjo
(`loadComponent`), pa gost koji gleda meni nikad ne preuzme administratorski deo.

Backend je zaseban projekat: `../RestoranProjekat` (ASP.NET Core, .NET 10).

## Pokretanje

Klijent očekuje API na `http://localhost:5070` (`src/environments/environment.development.ts`),
pa se **prvo pokreće backend**:

```bash
cd ../RestoranProjekat
dotnet run --project API --launch-profile http
```

Zatim, u drugom terminalu:

```bash
npm install     # samo prvi put
npm start       # ng serve, http://localhost:4200
```

## Produkcijski build

```bash
npm run build   # rezultat ide u dist/
```

## Struktura

| Putanja | Šta je unutra |
|---|---|
| `src/app/core/` | servisi, modeli, interceptori, čuvari ruta, cevi, direktive |
| `src/app/funkcije/` | ekrani, grupisani **po ulozi** (gost, nalog, konobar, priprema, menadžment, administracija) |
| `src/app/layout/` | zaglavlje, podnožje, logo |
| `src/styles/` | tokeni, tema i zajednički stilovi formi |

Ekrani su grupisani po ulozi, a ne po backend modulu, jer se i lenjo učitavanje deli po
ulozi — prijavljeni konobar preuzme samo svoj deo aplikacije.

## Dogovori u kodu

- Svaka komponenta su **tri datoteke** — `.ts`, `.html`, `.scss`. Nema `template:` ni `styles:` u dekoratoru.
- Nazivi na srpskom, osim tehničkih foldera (`core`, `guards`, `pipes`).
- Boje i razmaci idu isključivo kroz CSS promenljive iz `src/styles/_tokens.scss`.
