export interface PaginiranaLista<T> {
  podaci: T[];
  ukupnoZapisa: number;
  trenutnaStrana: number;
  ukupnoStrana: number;
}

export interface ApiGreska {
  poruka: string;
  greske: Record<string, string[]>;
}

export type Uloga = 'Korisnik' | 'Konobar' | 'Sanker' | 'Kuvar' | 'Administrator' | 'Menadzer';

export const ULOGE_OSOBLJA: Uloga[] = ['Konobar', 'Sanker', 'Kuvar', 'Administrator', 'Menadzer'];

export const ULOGE_SMENE: Uloga[] = ['Konobar', 'Sanker', 'Kuvar'];

export interface PrijavaZahtev {
  email: string;
  lozinka: string;
}

export interface RegistracijaZahtev {
  ime: string;
  prezime: string;
  email: string;
  lozinka: string;
  brojTelefona?: string | null;
}

export interface PotvrdaEmailZahtev {
  korisnikId: string;
  token: string;
}

export interface ResetLozinkeZahtev {
  email: string;
  token: string;
  novaLozinka: string;
}

export interface EmailZahtev {
  email: string;
}

export interface PorukaOdgovor {
  poruka: string;
}

export interface TokenOdgovor {
  accessToken: string;
  refreshToken: string;
  isticeAccessToken: string;
  korisnikId: string;
  ime: string;
  prezime: string;
  email: string;
  uloge: Uloga[];
}

export type Odrediste = 'Kuhinja' | 'Sank';

export interface KategorijaMenija {
  id: number;
  naziv: string;
  opis: string | null;
  redosled: number;
  odrediste: Odrediste;
}

export interface SlikaStavkeMenija {
  id: number;
  slikaUrl: string;
  opis: string | null;
  redosled: number;
}

export interface StavkaMenija {
  id: number;
  naziv: string;
  opis: string;
  detaljanOpis: string | null;
  cena: number;
  slikaUrl: string;
  dodatneSlike: SlikaStavkeMenija[];
  kategorijaId: number;
  kategorijaNaziv: string;
  dostupno: boolean;
  popust: number | null;
  cenaSaPopustom: number;
  datumKreiranja: string;
  prosecnaOcena: number | null;
  brojRecenzija: number;
  brojLajkova: number;
}

export interface KategorijaMenijaUlaz {
  naziv: string;
  opis?: string | null;
  redosled: number;
  odrediste: Odrediste;
}

export interface StavkaMenijaUlaz {
  naziv: string;
  opis: string;
  detaljanOpis?: string | null;
  cena: number;
  kategorijaId: number;
  popust?: number | null;
}

export interface MeniPretraga {
  kategorijaId?: number | null;
  pretraga?: string | null;
  sortiranje?: string | null;
  strana?: number;
  velicinaStrane?: number;
}

export interface KorisnikProfil {
  id: string;
  ime: string;
  prezime: string;
  email: string;
  brojTelefona: string | null;
  slikaUrl: string | null;
  datumRegistracije: string;
}

export interface IzmenaProfila {
  ime: string;
  prezime: string;
  brojTelefona?: string | null;
}

export interface PromenaLozinke {
  staraLozinka: string;
  novaLozinka: string;
}

export type StatusRezervacije = 'Aktivna' | 'Realizovana' | 'Istekla' | 'Otkazana';
export type NacinKreiranjaRezervacije = 'Samostalno' | 'TelefonAdmin' | 'KontaktForma';
export type StatusStola = 'Slobodan' | 'Zauzet' | 'Rezervisan';

export interface Sto {
  id: number;
  brojStola: number;
  kapacitet: number;
  trenutniStatus: StatusStola;
}

export interface Rezervacija {
  id: number;
  korisnikId: string | null;
  imeZaPrikaz: string;
  stoId: number;
  brojStola: number;
  brojeviStolova: number[];
  datumVreme: string;
  vaziOd: string;
  vaziDo: string;
  tolerancijaDo: string;
  brojGostiju: number;
  kodRezervacije: string;
  status: StatusRezervacije;
  nacinKreiranja: NacinKreiranjaRezervacije;
  datumKreiranja: string;
}

export interface KreirajRezervaciju {
  stoIds: number[];
  datumVreme: string;
  brojGostiju: number;
}

export interface PredlogSpajanja {
  stolovi: Sto[];
  ukupnoMesta: number;
  visakMesta: number;
}

export interface Dostupnost {
  stolovi: Sto[];
  predlozi: PredlogSpajanja[];
  maksGostijuOnline: number;
  telefon: string | null;
}

export type StatusPoruke = 'Novo' | 'Procitano' | 'Odgovoreno';

export interface Poruka {
  id: number;
  korisnikId: string | null;
  posiljalacIme: string;
  posiljalacEmail: string | null;
  posiljalacTelefon: string | null;
  kategorija: KategorijaPoruke;
  tekst: string;
  datumSlanja: string;
  status: StatusPoruke;
  odgovor: string | null;
  datumOdgovora: string | null;
  zeljeniDatumVreme: string | null;
  zeljeniBrojGostiju: number | null;
  prioritetna: boolean;
}

export type TipRecenzije = 'Jelo' | 'Usluga' | 'Restoran';

export interface KreirajRecenziju {
  tipRecenzije: TipRecenzije;
  stavkaMenijaId?: number | null;
  ocena: number;
  naslov?: string | null;
  tekst: string;
}

export interface IzmenaRecenzije {
  ocena: number;
  naslov?: string | null;
  tekst: string;
}

export interface Recenzija {
  id: number;
  korisnikId: string;
  autorIme: string;
  autorSlikaUrl: string | null;
  tipRecenzije: TipRecenzije;
  stavkaMenijaId: number | null;
  nazivStavke: string | null;
  ocena: number;
  naslov: string | null;
  tekst: string;
  datumKreiranja: string;
  datumIzmene: string | null;
  odgovorRestorana: string | null;
  datumOdgovora: string | null;
}

export interface RecenzijePretraga {
  stavkaMenijaId?: number | null;
  tipRecenzije?: TipRecenzije | null;
  sortiranje?: 'najbolje-ocenjeno' | null;
  strana?: number;
  velicinaStrane?: number;
}

export interface Postavke {
  adresa: string;
  telefon: string;
  email: string;
  radnoVreme: string;
  opisRestorana: string;
  geoSirina: number | null;
  geoDuzina: number | null;
  facebookUrl: string | null;
  instagramUrl: string | null;
}

export interface GalerijaSlika {
  id: number;
  slikaUrl: string;
  naslov: string;
  opis: string | null;
  grupa: string;
  redosled: number;
  aktivan: boolean;
  datumDodavanja: string;
}

export interface GalerijaOblast {
  naziv: string;
  naslovna: GalerijaSlika;
  slike: GalerijaSlika[];
}

export interface Smena {
  id: number;
  zaposleniId: number;
  imeZaposlenog: string;
  datum: string;
  vremePocetka: string;
  vremeKraja: string;
  trajanjeSati: number;
  prelaziPonoc: boolean;
  prosla: boolean;
}

export interface RasporedNedelje {
  pocetakNedelje: string;
  krajNedelje: string;
  zaposleni: RasporedZaposlenog[];
}

export interface RasporedZaposlenog {
  zaposleniId: number;
  imeZaposlenog: string;
  smene: Smena[];
  ukupnoSati: number;
}

export interface KreirajSmenu {
  zaposleniId: number;
  datum: string;
  vremePocetka: string;
  vremeKraja: string;
}

export interface IzmenaSmene {
  datum: string;
  vremePocetka: string;
  vremeKraja: string;
}

export interface UcinakZaposlenog {
  zaposleniId: number;
  imeZaposlenog: string;
  brojStolova: number;
  vrednostStolova: number;
  ukupnaNapojnica: number;
  brojPripremljenihStavki: number;
  ukupnaKolicinaPripremljena: number;
  vrednostPripremljenih: number;
  prosecanRacun: number;
}

export interface Bonus {
  id: number;
  zaposleniId: number;
  imeZaposlenog: string;
  iznos: number;
  datumPocetka: string;
  datumKraja: string;
  razlog: string;
  dodelioZaposleniId: number;
  dodelioIme: string;
  datumDodele: string;
  vazi: boolean;
}

export type KategorijaProblema = 'Oprema' | 'Namirnice' | 'Higijena' | 'Softver' | 'Ostalo';
export type PrioritetProblema = 'Nizak' | 'Srednji' | 'Visok';
export type StatusPrijaveProblema = 'Nova' | 'UObradi' | 'Resena' | 'Odbijena';

export interface PrijavaProblema {
  id: number;
  prijavioZaposleniId: number;
  prijavioIme: string;
  naslov: string;
  opis: string;
  kategorija: KategorijaProblema;
  prioritet: PrioritetProblema;
  status: StatusPrijaveProblema;
  datumPrijave: string;
  resioIme: string | null;
  odgovor: string | null;
  datumResavanja: string | null;
}

export interface PrijaviProblem {
  naslov: string;
  opis: string;
  kategorija: KategorijaProblema;
  prioritet: PrioritetProblema;
}

export interface ClanTima {
  ime: string;
  prezime: string;
  uloga: Uloga;
  slikaUrl: string | null;
  biografija: string | null;
  linkedInUrl: string | null;
  instagramUrl: string | null;
}

export type KategorijaPoruke = 'Pitanje' | 'Sugestija' | 'Rezervacija' | 'Zalba' | 'Pohvala';

export interface PosaljiPoruku {
  kategorija: KategorijaPoruke;
  tekst: string;
  ime?: string | null;
  email?: string | null;
  telefon?: string | null;
  zeljeniDatumVreme?: string | null;
  zeljeniBrojGostiju?: number | null;
}

export type StatusPorudzbine = 'Otvorena' | 'Zatvorena';
export type NacinPlacanja = 'Gotovina' | 'Kartica';

export type StatusStavkePorudzbine = 'Poslato' | 'UPripremi' | 'Spremno';

export interface PorudzbinaAktivna {
  id: number;
  brojStola: number;
  rezervacijaId: number | null;
  vremeOtvaranja: string;
  brojStavkiPoslato: number;
  brojStavkiUPripremi: number;
  brojStavkiSpremno: number;
}

export interface StavkaPorudzbine {
  id: number;
  stavkaMenijaId: number;
  nazivStavke: string;
  kolicina: number;
  napomena: string | null;
  cenaUTrenutkuNarudzbine: number;
  status: StatusStavkePorudzbine;
  pripremioZaposleniId: number | null;
  vremeSlanja: string;
  vremePreuzimanja: string | null;
  vremeZavrsetka: string | null;
}

export interface PorudzbinaDetalj {
  id: number;
  stoId: number;
  brojStola: number;
  konobarId: number;
  konobarIme: string;
  vremeOtvaranja: string;
  vremeZatvaranja: string | null;
  status: StatusPorudzbine;
  nacinPlacanja: NacinPlacanja | null;
  iznosNapojnice: number | null;
  stavke: StavkaPorudzbine[];
}

export interface OtvoriPorudzbinu {
  stoId: number;
  rezervacijaId?: number | null;
}

export interface StavkaZaDodavanje {
  stavkaMenijaId: number;
  kolicina: number;
  napomena?: string | null;
}

export interface DodajStavke {
  stavke: StavkaZaDodavanje[];
}

export interface ZatvoriPorudzbinu {
  nacinPlacanja: NacinPlacanja;
  iznosNapojnice?: number | null;
}

export interface RedCekanjaStavka {
  id: number;
  brojStola: number;
  nazivStavke: string;
  kolicina: number;
  napomena: string | null;
  vremeSlanja: string;
  status: StatusStavkePorudzbine;
  pripremioIme: string | null;
  mojaStavka: boolean;
}

export interface NovaStavkaNotifikacija {
  stavkaPorudzbineId: number;
  brojStola: number;
  nazivStavke: string;
  kolicina: number;
  napomena: string | null;
}

export interface StavkaSpremnaNotifikacija {
  stavkaPorudzbineId: number;
  brojStola: number;
  nazivStavke: string;
}

export interface KorisnikAdmin {
  id: string;
  ime: string;
  prezime: string;
  email: string;
  brojTelefona: string | null;
  datumRegistracije: string;
  aktivan: boolean;
  blokiranDo: string | null;
  zabranaKomentarisanjaDo: string | null;
  uloge: Uloga[];
  blokiran: boolean;
}

export interface KorisniciPretraga {
  pretraga?: string | null;
  blokiran?: boolean | null;
  strana?: number;
  velicinaStrane?: number;
}

export interface BlokirajKorisnika {
  razlog: string;
  datumDo?: string | null;
}

export interface ZabraniKomentarisanje {
  brojDana: number;
  razlog: string;
}

export interface RecenzijaPregled {
  id: number;
  tipRecenzije: TipRecenzije;
  nazivStavke: string | null;
  ocena: number;
  naslov: string | null;
  datumKreiranja: string;
  aktivan: boolean;
}

export interface RezervacijaPregled {
  id: number;
  kodRezervacije: string;
  datumVreme: string;
  brojGostiju: number;
  status: StatusRezervacije;
}

export interface PorukaPregled {
  id: number;
  kategorija: KategorijaPoruke;
  status: StatusPoruke;
  datumSlanja: string;
}

export interface RacunGosta {
  porudzbinaId: number;
  kodRezervacije: string | null;
  vremeZatvaranja: string | null;
  brojStola: number;
  ukupno: number;
  napojnica: number;
}

export interface PouzdanostGosta {
  realizovane: number;
  istekle: number;
  otkazane: number;
  aktivne: number;
  procenatPojavljivanja: number | null;
}

export interface KorisnikDetalj {
  profil: KorisnikAdmin;
  recenzije: RecenzijaPregled[];
  rezervacije: RezervacijaPregled[];
  poruke: PorukaPregled[];
  racuni: RacunGosta[];
  pouzdanost: PouzdanostGosta;
  ukupnoPotroseno: number;
  ukupnaNapojnica: number;
}

export interface Zaposleni {
  id: number;
  korisnikId: string;
  ime: string;
  prezime: string;
  email: string;
  uloga: Uloga;
  datumZaposlenja: string;
  aktivan: boolean;
  slikaUrl: string | null;
  biografija: string | null;
  linkedInUrl: string | null;
  instagramUrl: string | null;
  redosled: number;
  prikaziNaSajtu: boolean;
}

export interface ZaposleniPretraga {
  pretraga?: string | null;
  uloga?: string | null;
  aktivan?: boolean | null;
  strana?: number;
  velicinaStrane?: number;
}

export interface KreirajZaposlenog {
  ime: string;
  prezime: string;
  email: string;
  privremenaLozinka: string;
  uloga: Uloga;
  datumZaposlenja: string;
}

export interface IzmenaZaposlenog {
  ime: string;
  prezime: string;
  uloga: Uloga;
}

export interface OdgovorRecenzije {
  odgovorRestorana: string;
}

export interface RezervacijePretraga {
  datum?: string | null;
  status?: StatusRezervacije | null;
  pretraga?: string | null;
  strana?: number;
  velicinaStrane?: number;
}

export interface KreirajAdminRezervaciju {
  stoId: number;
  datumVreme: string;
  brojGostiju: number;
  nacinKreiranja: 'TelefonAdmin' | 'KontaktForma';
  korisnikId?: string | null;
  gostIme?: string | null;
  gostEmail?: string | null;
  gostTelefon?: string | null;
}

export interface PorukePretraga {
  status?: StatusPoruke | null;
  kategorija?: KategorijaPoruke | null;
  pretraga?: string | null;
  strana?: number;
  velicinaStrane?: number;
}

export interface OdgovorNaPoruku {
  odgovor: string;
}

export interface IzmenaGalerijeSlike {
  naslov: string;
  opis?: string | null;
  grupa: string;
  redosled: number;
  aktivan: boolean;
}

export interface IzmenaPostavki {
  adresa: string;
  telefon: string;
  email: string;
  radnoVreme: string;
  opisRestorana: string;
  geoSirina?: number | null;
  geoDuzina?: number | null;
  facebookUrl?: string | null;
  instagramUrl?: string | null;
}

export interface IzmenaProfilaNaSajtu {
  biografija?: string | null;
  linkedInUrl?: string | null;
  instagramUrl?: string | null;
  redosled: number;
  prikaziNaSajtu: boolean;
}

export interface AuditLog {
  id: number;
  korisnikId: string | null;
  korisnikEmail: string | null;
  nazivSlucajaKoriscenja: string;
  datum: string;
  uspesnoIzvrseno: boolean;
  poruka: string | null;
}

export interface IstorijaStavka {
  naziv: string;
  kolicina: number;
  cena: number;
  ukupno: number;
}

export interface IstorijaRacun {
  porudzbinaId: number;
  brojStola: number;
  vremeOtvaranja: string;
  vremeZatvaranja: string | null;
  nacinPlacanja: string | null;
  napojnica: number;
  ukupno: number;
  stavke: IstorijaStavka[];
}

export interface IstorijaPriprema {
  stavkaId: number;
  naziv: string;
  kolicina: number;
  brojStola: number;
  vremeZavrsetka: string;
  vrednost: number;
}

export interface IstorijaDan {
  datum: string;

  racuni: IstorijaRacun[];
  pripreme: IstorijaPriprema[];

  brojRacuna: number;
  prometRacuna: number;
  napojnica: number;

  brojPripremljenihStavki: number;
  kolicinaPripremljena: number;
  vrednostPripremljenog: number;
}

export interface AuditLogPretraga {
  korisnikId?: string | null;
  nazivSlucajaKoriscenja?: string | null;
  datumOd?: string | null;
  datumDo?: string | null;
  pretraga?: string | null;
  strana?: number;
  velicinaStrane?: number;
}

export interface AdminAkcija {
  id: number;
  administratorId: number;
  administratorIme: string;
  tipAkcije: string;
  ciljniKorisnikId: string | null;
  ciljniKorisnikEmail: string | null;
  opis: string;
  datum: string;
}

export interface AdminAkcijePretraga {
  tipAkcije?: string | null;
  ciljniKorisnikId?: string | null;
  datumOd?: string | null;
  datumDo?: string | null;
  pretraga?: string | null;
  strana?: number;
  velicinaStrane?: number;
}

export type JedinicaMere = 'Kg' | 'L' | 'Kom';

export interface Namirnica {
  id: number;
  naziv: string;
  jedinicaMere: JedinicaMere;
  trenutnaKolicina: number;
  minimalniPrag: number;
  ispodPraga: boolean;
}

export interface KreirajNamirnicu {
  naziv: string;
  jedinicaMere: JedinicaMere;
  minimalniPrag: number;
  pocetnaKolicina: number;
}

export interface IzmenaNamirnice {
  naziv: string;
  minimalniPrag: number;
}

export interface KorekcijaKolicine {
  novaKolicina?: number | null;
  delta?: number | null;
  razlog: string;
}

export interface NabavkaStavka {
  namirnicaId: number;
  naziv: string;
  jedinicaMere: JedinicaMere;
  trenutnaKolicina: number;
  minimalniPrag: number;
  predlozenaKolicina: number;
}

export interface ReceptStavka {
  namirnicaId: number;
  namirnicaNaziv: string;
  jedinicaMere: JedinicaMere;
  kolicina: number;
}

export interface DodajURecepturu {
  namirnicaId: number;
  kolicina: number;
}

export interface IzmenaKolicine {
  kolicina: number;
}

export type GrupisanjePrihoda = 'Dan' | 'Nedelja' | 'Mesec' | 'Kategorija' | 'Artikal';
export type RedosledTopJela = 'Najprodavanije' | 'NajmanjeProdavano';

export interface IzvestajPeriod {
  datumOd: string;
  datumDo: string;
}

export interface PrihodUpit extends IzvestajPeriod {
  grupisanjePo: GrupisanjePrihoda;
}

export interface PrihodStavka {
  grupa: string;
  prihod: number;
  kolicina: number;
  udeoProcenat: number;
}

export interface PrihodIzvestaj {
  datumOd: string;
  datumDo: string;
  grupisanjePo: GrupisanjePrihoda;
  ukupanPrihod: number;
  ukupnaNapojnica: number;
  brojPorudzbina: number;
  prosecanRacun: number;
  stavke: PrihodStavka[];
}

export interface UcinakUpit extends IzvestajPeriod {
  zaposleniId?: number | null;
}

export interface TopJelaUpit extends IzvestajPeriod {
  redosled: RedosledTopJela;
}

export interface TopJelo {
  rang: number;
  stavkaMenijaId: number;
  naziv: string;
  kategorija: string;
  ukupnaKolicina: number;
  brojPorudzbina: number;
  prihod: number;
}

export interface BonusiPretraga {
  zaposleniId?: number | null;
  datumOd?: string | null;
  datumDo?: string | null;
  strana?: number;
  velicinaStrane?: number;
}

export interface KreirajBonus {
  zaposleniIds: number[];
  iznos: number;
  datumPocetka: string;
  datumKraja: string;
  razlog: string;
}
