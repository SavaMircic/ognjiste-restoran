import { Routes } from '@angular/router';

import { ULOGE_OSOBLJA, ULOGE_SMENE } from './core/modeli/api.modeli';
import { samoNeprijavljeni } from './core/guards/gost.guard';
import { samoGosti } from './core/guards/smena.guard';
import { zahtevaUlogu } from './core/guards/uloga.guard';

export const routes: Routes = [
  {
    path: '',
    pathMatch: 'full',
    title: 'Ognjište — domaća kuhinja, Novi Sad',
    canActivate: [samoGosti],
    loadComponent: () => import('./funkcije/gost/pocetna/pocetna').then((m) => m.Pocetna),
  },
  {
    path: 'meni',
    title: 'Meni — Ognjište',
    canActivate: [samoGosti],
    loadComponent: () => import('./funkcije/gost/meni/meni').then((m) => m.Meni),
  },
  {
    path: 'meni/:id',
    title: 'Jelo — Ognjište',
    canActivate: [samoGosti],
    loadComponent: () => import('./funkcije/gost/detalj-jela/detalj-jela').then((m) => m.DetaljJela),
  },
  {
    path: 'recenzije',
    title: 'Recenzije — Ognjište',
    canActivate: [samoGosti],
    loadComponent: () => import('./funkcije/gost/recenzije/recenzije').then((m) => m.Recenzije),
  },
  {
    path: 'galerija',
    title: 'Galerija — Ognjište',
    canActivate: [samoGosti],
    loadComponent: () => import('./funkcije/gost/galerija/galerija').then((m) => m.Galerija),
  },
  {
    path: 'nas-tim',
    title: 'Naš tim — Ognjište',
    canActivate: [samoGosti],
    loadComponent: () => import('./funkcije/gost/nas-tim/nas-tim').then((m) => m.NasTim),
  },
  {
    path: 'kontakt',
    title: 'Kontakt — Ognjište',
    canActivate: [samoGosti],
    loadComponent: () => import('./funkcije/gost/kontakt/kontakt').then((m) => m.Kontakt),
  },
  {
    path: 'rezervacija',
    title: 'Rezervacija — Ognjište',
    canActivate: [samoGosti, zahtevaUlogu('Korisnik')],
    loadComponent: () => import('./funkcije/rezervacija/rezervacija').then((m) => m.Rezervacija),
  },
  {
    path: 'nalog',
    canActivate: [zahtevaUlogu()],
    loadComponent: () => import('./funkcije/nalog/nalog').then((m) => m.Nalog),
    children: [
      { path: '', pathMatch: 'full', redirectTo: 'profil' },
      {
        path: 'profil',
        title: 'Profil — Ognjište',
        loadComponent: () => import('./funkcije/nalog/profil/profil').then((m) => m.Profil),
      },
      {
        path: 'rezervacije',
        title: 'Moje rezervacije — Ognjište',
        canActivate: [zahtevaUlogu('Korisnik')],
        loadComponent: () =>
          import('./funkcije/nalog/rezervacije/rezervacije').then((m) => m.MojeRezervacije),
      },
      {
        path: 'recenzije',
        title: 'Moje recenzije — Ognjište',
        canActivate: [zahtevaUlogu('Korisnik')],
        loadComponent: () => import('./funkcije/nalog/recenzije/recenzije').then((m) => m.MojeRecenzije),
      },
      {
        path: 'omiljena',
        title: 'Omiljena jela — Ognjište',
        canActivate: [zahtevaUlogu('Korisnik')],
        loadComponent: () => import('./funkcije/nalog/omiljena/omiljena').then((m) => m.Omiljena),
      },
      {
        path: 'poruke',
        title: 'Moje poruke — Ognjište',
        canActivate: [zahtevaUlogu('Korisnik')],
        loadComponent: () => import('./funkcije/nalog/poruke/poruke').then((m) => m.MojePoruke),
      },
    ],
  },

  {
    path: 'osoblje',
    canActivate: [zahtevaUlogu(...ULOGE_OSOBLJA)],
    loadComponent: () => import('./funkcije/osoblje/osoblje').then((m) => m.Osoblje),
    children: [
      { path: '', pathMatch: 'full', redirectTo: 'raspored' },
      {
        path: 'raspored',
        title: 'Moj raspored — Ognjište',
        loadComponent: () => import('./funkcije/osoblje/raspored/raspored').then((m) => m.MojRaspored),
      },
      {
        path: 'statistika',
        title: 'Moj učinak — Ognjište',
        canActivate: [zahtevaUlogu(...ULOGE_SMENE)],
        loadComponent: () =>
          import('./funkcije/osoblje/statistika/statistika').then((m) => m.MojaStatistika),
      },
      {
        path: 'istorija',
        title: 'Istorija rada — Ognjište',
        canActivate: [zahtevaUlogu(...ULOGE_SMENE)],
        loadComponent: () =>
          import('./funkcije/osoblje/istorija/istorija').then((m) => m.IstorijaRada),
      },
      {
        path: 'bonusi',
        title: 'Moji bonusi — Ognjište',
        loadComponent: () => import('./funkcije/osoblje/bonusi/bonusi').then((m) => m.MojiBonusi),
      },
      {
        path: 'prijave',
        title: 'Prijave problema — Ognjište',
        loadComponent: () => import('./funkcije/osoblje/prijave/prijave').then((m) => m.Prijave),
      },
    ],
  },

  {
    path: 'konobar',
    canActivate: [zahtevaUlogu('Konobar')],
    loadComponent: () => import('./funkcije/konobar/konobar').then((m) => m.Konobar),
    children: [
      { path: '', pathMatch: 'full', redirectTo: 'stolovi' },
      {
        path: 'stolovi',
        title: 'Stolovi — Ognjište',
        loadComponent: () =>
          import('./funkcije/konobar/stolovi/stolovi').then((m) => m.KonobarStolovi),
      },
      {
        path: 'rezervacije',
        title: 'Rezervacije danas — Ognjište',
        loadComponent: () =>
          import('./funkcije/konobar/rezervacije/rezervacije').then((m) => m.RezervacijeDanas),
      },
      {
        path: 'porudzbina/:id',
        title: 'Porudžbina — Ognjište',
        loadComponent: () =>
          import('./funkcije/konobar/porudzbina/porudzbina').then((m) => m.KonobarPorudzbina),
      },
    ],
  },

  {
    path: 'administracija',
    canActivate: [zahtevaUlogu('Administrator')],
    loadComponent: () =>
      import('./funkcije/administracija/administracija').then((m) => m.Administracija),
    children: [
      { path: '', pathMatch: 'full', redirectTo: 'rezervacije' },
      {
        path: 'meni',
        title: 'Jela — Ognjište',
        loadComponent: () => import('./funkcije/administracija/meni/meni').then((m) => m.AdminMeni),
      },
      {
        path: 'meni/:id/slike',
        title: 'Slike jela — Ognjište',
        loadComponent: () =>
          import('./funkcije/administracija/meni/slike/slike').then((m) => m.AdminSlikeJela),
      },
      {
        path: 'meni/:id/receptura',
        title: 'Receptura — Ognjište',
        loadComponent: () =>
          import('./funkcije/administracija/meni/receptura/receptura').then((m) => m.AdminReceptura),
      },
      {
        path: 'kategorije',
        title: 'Kategorije menija — Ognjište',
        loadComponent: () =>
          import('./funkcije/administracija/kategorije/kategorije').then((m) => m.AdminKategorije),
      },
      {
        path: 'korisnici',
        title: 'Korisnici — Ognjište',
        loadComponent: () =>
          import('./funkcije/administracija/korisnici/korisnici').then((m) => m.AdminKorisnici),
      },
      {
        path: 'korisnici/:id',
        title: 'Korisnik — Ognjište',
        loadComponent: () =>
          import('./funkcije/administracija/korisnici/detalj/detalj').then(
            (m) => m.AdminKorisnikDetalj,
          ),
      },
      {
        path: 'zaposleni',
        title: 'Zaposleni — Ognjište',
        loadComponent: () =>
          import('./funkcije/administracija/zaposleni/zaposleni').then((m) => m.AdminZaposleni),
      },
      {
        path: 'recenzije',
        title: 'Moderacija recenzija — Ognjište',
        loadComponent: () =>
          import('./funkcije/administracija/recenzije/recenzije').then((m) => m.AdminRecenzije),
      },
      {
        path: 'rezervacije',
        title: 'Rezervacije — Ognjište',
        loadComponent: () =>
          import('./funkcije/administracija/rezervacije/rezervacije').then((m) => m.AdminRezervacije),
      },
      {
        path: 'poruke',
        title: 'Poruke — Ognjište',
        loadComponent: () =>
          import('./funkcije/administracija/poruke/poruke').then((m) => m.AdminPoruke),
      },
      {
        path: 'galerija',
        title: 'Galerija — Ognjište',
        loadComponent: () =>
          import('./funkcije/administracija/galerija/galerija').then((m) => m.AdminGalerija),
      },
      {
        path: 'tim',
        title: 'Naš tim — Ognjište',
        loadComponent: () => import('./funkcije/administracija/tim/tim').then((m) => m.AdminTim),
      },
      {
        path: 'postavke',
        title: 'Postavke — Ognjište',
        loadComponent: () =>
          import('./funkcije/administracija/postavke/postavke').then((m) => m.AdminPostavke),
      },
      {
        path: 'dnevnik',
        title: 'Dnevnik — Ognjište',
        loadComponent: () =>
          import('./funkcije/administracija/dnevnik/dnevnik').then((m) => m.AdminDnevnik),
      },
    ],
  },

  {
    path: 'menadzment',
    canActivate: [zahtevaUlogu('Menadzer')],
    loadComponent: () => import('./funkcije/menadzment/menadzment').then((m) => m.Menadzment),
    children: [
      { path: '', pathMatch: 'full', redirectTo: 'izvestaji' },
      {
        path: 'raspored',
        title: 'Raspored smena — Ognjište',
        loadComponent: () =>
          import('./funkcije/menadzment/raspored/raspored').then((m) => m.MenadzerRaspored),
      },
      {
        path: 'zalihe',
        title: 'Zalihe — Ognjište',
        loadComponent: () =>
          import('./funkcije/menadzment/zalihe/zalihe').then((m) => m.MenadzerZalihe),
      },
      {
        path: 'izvestaji',
        title: 'Izveštaji — Ognjište',
        loadComponent: () =>
          import('./funkcije/menadzment/izvestaji/izvestaji').then((m) => m.MenadzerIzvestaji),
      },
      {
        path: 'bonusi',
        title: 'Bonusi — Ognjište',
        loadComponent: () =>
          import('./funkcije/menadzment/bonusi/bonusi').then((m) => m.MenadzerBonusi),
      },
    ],
  },

  {
    path: 'kuhinja',
    title: 'Kuhinja — Ognjište',
    canActivate: [zahtevaUlogu('Kuvar')],
    data: { odrediste: 'Kuhinja' },
    loadComponent: () => import('./funkcije/priprema/red-cekanja').then((m) => m.RedCekanja),
  },
  {
    path: 'sank',
    title: 'Šank — Ognjište',
    canActivate: [zahtevaUlogu('Sanker')],
    data: { odrediste: 'Sank' },
    loadComponent: () => import('./funkcije/priprema/red-cekanja').then((m) => m.RedCekanja),
  },

  {
    path: 'prijava',
    title: 'Prijava — Ognjište',
    canActivate: [samoNeprijavljeni],
    loadComponent: () => import('./funkcije/auth/prijava/prijava').then((m) => m.Prijava),
  },
  {
    path: 'registracija',
    title: 'Registracija — Ognjište',
    canActivate: [samoNeprijavljeni],
    loadComponent: () => import('./funkcije/auth/registracija/registracija').then((m) => m.Registracija),
  },
  {
    path: 'potvrda-email',
    title: 'Potvrda naloga — Ognjište',
    loadComponent: () => import('./funkcije/auth/potvrda-email/potvrda-email').then((m) => m.PotvrdaEmail),
  },
  {
    path: 'posalji-ponovo-potvrdu',
    title: 'Nov link za potvrdu — Ognjište',
    canActivate: [samoNeprijavljeni],
    loadComponent: () => import('./funkcije/auth/posalji-ponovo/posalji-ponovo').then((m) => m.PosaljiPonovo),
  },
  {
    path: 'zaboravljena-lozinka',
    title: 'Zaboravljena lozinka — Ognjište',
    canActivate: [samoNeprijavljeni],
    loadComponent: () =>
      import('./funkcije/auth/zaboravljena-lozinka/zaboravljena-lozinka').then((m) => m.ZaboravljenaLozinka),
  },
  {
    path: 'reset-lozinke',
    title: 'Nova lozinka — Ognjište',
    loadComponent: () => import('./funkcije/auth/reset-lozinke/reset-lozinke').then((m) => m.ResetLozinke),
  },
  {
    path: 'nemate-pristup',
    title: 'Nemate pristup — Ognjište',
    loadComponent: () => import('./funkcije/zajednicko/nema-pristupa/nema-pristupa').then((m) => m.NemaPristupa),
  },
  {
    path: '**',
    title: 'Stranica nije pronađena — Ognjište',
    loadComponent: () => import('./funkcije/zajednicko/nije-pronadjeno/nije-pronadjeno').then((m) => m.NijePronadjeno),
  },
];
