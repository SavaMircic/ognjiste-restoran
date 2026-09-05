using Domain.Entiteti;
using Domain.Enumi;
using Domain.Konstante;
using Microsoft.EntityFrameworkCore;

namespace Repository;

public static partial class PocetniPodaci
{
    private static async Task PopuniAdministracijuAsync(RestoranDbContext kontekst)
    {
        await PopuniAdminAkcijeAsync(kontekst);
        await PopuniPrijaveProblemaAsync(kontekst);
        await PopuniAuditLogAsync(kontekst);
    }

    private static async Task PopuniAdminAkcijeAsync(RestoranDbContext kontekst)
    {
        if (await kontekst.AdminAkcije.AnyAsync()) return;

        var admin = await ZaposleniPoMejluAsync(kontekst, MejlAdmin);
        if (admin == null) return;

        var gosti = await kontekst.Users
            .Where(k => k.Email!.EndsWith("@test.com"))
            .ToDictionaryAsync(k => k.Email!, k => k.Id);

        var sada = DateTime.UtcNow;

        var akcije = new (string Tip, string Mejl, string Opis, int DanaUnazad)[]
        {
            (TipoviAdminAkcija.Blokiranje, "marko@test.com",
                "Privremeno blokiran na 7 dana — uvredljiv rečnik u komentarima ispod recenzija.", 30),
            (TipoviAdminAkcija.Odblokiranje, "marko@test.com",
                "Rok istekao, korisnik se izvinio i uklonio sporni sadržaj.", 23),

            (TipoviAdminAkcija.BrisanjeRecenzije, "vuk@test.com",
                "Uklonjena recenzija restorana zbog nepristojnog rečnika (meko brisanje, sadržaj ostaje u bazi).", 24),
            (TipoviAdminAkcija.ZabranaKomentarisanja, "vuk@test.com",
                "Zabrana pisanja recenzija na 30 dana — ponovljeno kršenje pravila o rečniku.", 21),

            (TipoviAdminAkcija.Blokiranje, "dusan@test.com",
                "Nalog trajno deaktiviran — lažne rezervacije na tuđe ime, tri puta uzastopno.", 14),

            (TipoviAdminAkcija.Blokiranje, "tijana@test.com",
                "Privremeno blokirana — nije se pojavila na tri rezervisana termina bez otkazivanja.", 5)
        };

        foreach (var akcija in akcije)
        {
            gosti.TryGetValue(akcija.Mejl, out var ciljniId);

            kontekst.AdminAkcije.Add(new AdminAkcija
            {
                AdministratorId = admin.Id,
                TipAkcije = akcija.Tip,
                CiljniKorisnikId = ciljniId,
                Opis = akcija.Opis,
                Datum = sada.AddDays(-akcija.DanaUnazad)
            });
        }

        await kontekst.SaveChangesAsync();
    }

    private static async Task PopuniPrijaveProblemaAsync(RestoranDbContext kontekst)
    {
        if (await kontekst.PrijaveProblema.AnyAsync()) return;

        var menadzer = await ZaposleniPoMejluAsync(kontekst, MejlMenadzer);
        var konobar = await ZaposleniPoMejluAsync(kontekst, MejlKonobar);
        var konobar2 = await ZaposleniPoMejluAsync(kontekst, MejlKonobar2);
        var kuvar = await ZaposleniPoMejluAsync(kontekst, MejlKuvar);
        var sanker = await ZaposleniPoMejluAsync(kontekst, MejlSanker);
        var sanker2 = await ZaposleniPoMejluAsync(kontekst, MejlSanker2);

        if (menadzer == null || konobar == null || konobar2 == null ||
            kuvar == null || sanker == null || sanker2 == null) return;

        var sada = DateTime.UtcNow;

        kontekst.PrijaveProblema.AddRange(
            new PrijavaProblema
            {
                PrijavioZaposleniId = kuvar.Id,
                Naslov = "Rerna ne drži temperaturu",
                Opis = "Gornja rerna pada sa 220 na 180 stepeni tokom pečenja. Pečenja traju duže i gosti čekaju.",
                Kategorija = KategorijaProblema.Oprema,
                Prioritet = PrioritetProblema.Visok,
                Status = StatusPrijaveProblema.Resena,
                DatumPrijave = sada.AddDays(-12),
                ResioZaposleniId = menadzer.Id,
                Odgovor = "Serviser je zamenio termostat. Provereno u radu dva dana, drži temperaturu.",
                DatumResavanja = sada.AddDays(-9)
            },
            new PrijavaProblema
            {
                PrijavioZaposleniId = sanker.Id,
                Naslov = "Klimava stolica na terasi",
                Opis = "Stolica kod stola 11 se klati, gost se skoro prevrnuo.",
                Kategorija = KategorijaProblema.Oprema,
                Prioritet = PrioritetProblema.Srednji,
                Status = StatusPrijaveProblema.Resena,
                DatumPrijave = sada.AddDays(-8),
                ResioZaposleniId = menadzer.Id,
                Odgovor = "Stolica povučena iz upotrebe i zamenjena novom iz magacina.",
                DatumResavanja = sada.AddDays(-7)
            },
            new PrijavaProblema
            {
                PrijavioZaposleniId = kuvar.Id,
                Naslov = "Kajmak se stalno troši pre nabavke",
                Opis = "Karađorđeva i proja troše kajmak brže nego što stiže. Ostajemo bez njega vikendom.",
                Kategorija = KategorijaProblema.Namirnice,
                Prioritet = PrioritetProblema.Visok,
                Status = StatusPrijaveProblema.UObradi,
                DatumPrijave = sada.AddDays(-3)
            },
            new PrijavaProblema
            {
                PrijavioZaposleniId = konobar.Id,
                Naslov = "Slavina u toaletu curi",
                Opis = "Curi celo popodne, voda se skuplja oko lavaboa.",
                Kategorija = KategorijaProblema.Higijena,
                Prioritet = PrioritetProblema.Srednji,
                Status = StatusPrijaveProblema.Nova,
                DatumPrijave = sada.AddHours(-20)
            },
            new PrijavaProblema
            {
                PrijavioZaposleniId = konobar2.Id,
                Naslov = "Tablet u sali se gasi",
                Opis = "Tablet za unos porudžbina se ugasi na svaka dva sata, moram ponovo da se prijavljujem.",
                Kategorija = KategorijaProblema.Softver,
                Prioritet = PrioritetProblema.Nizak,
                Status = StatusPrijaveProblema.Odbijena,
                DatumPrijave = sada.AddDays(-6),
                ResioZaposleniId = menadzer.Id,
                Odgovor = "Nije kvar — baterija je stara. Nova nabavka je planirana za sledeći kvartal, do tada koristiti punjač u sali.",
                DatumResavanja = sada.AddDays(-5)
            },
            new PrijavaProblema
            {
                PrijavioZaposleniId = sanker2.Id,
                Naslov = "Nedovoljno suncobrana u bašti",
                Opis = "Popodne je pola bašte na suncu i gosti odbijaju te stolove.",
                Kategorija = KategorijaProblema.Ostalo,
                Prioritet = PrioritetProblema.Nizak,
                Status = StatusPrijaveProblema.Nova,
                DatumPrijave = sada.AddHours(-5)
            });

        await kontekst.SaveChangesAsync();
    }

    private static async Task PopuniAuditLogAsync(RestoranDbContext kontekst)
    {
        var granica = DateTime.UtcNow.Date.AddDays(-2);
        if (await kontekst.AuditLogovi.AnyAsync(a => a.Datum < granica)) return;

        var korisnici = await kontekst.Users
            .Select(k => new { k.Id, k.Email })
            .ToListAsync();
        if (korisnici.Count == 0) return;

        string? PoMejlu(string mejl) => korisnici.FirstOrDefault(k => k.Email == mejl)?.Id;

        var poUlogama = new (string[] Mejlovi, string[] Slucajevi)[]
        {
            (new[] { "petar@test.com", "ana@test.com", "marko@test.com", "jelena@test.com", "stefan@test.com", "milica@test.com" },
             new[] { "Prijava", "KreirajRezervaciju", "MojeRezervacije", "OtkaziRezervaciju", "KreirajRecenziju",
                     "IzmeniRecenziju", "LajkujJelo", "UkloniLajk", "OmiljenaJela", "PosaljiPoruku",
                     "MojePoruke", "IzmenaProfila", "PromenaLozinke", "ProveriDostupnost", "PostaviProfilnuSliku" }),

            (new[] { MejlKonobar, MejlKonobar2 },
             new[] { "Prijava", "ListirajStolove", "OtvoriPorudzbinu", "DodajStavkeUPorudzbinu", "IsporuciStavku",
                     "ZatvoriPorudzbinu", "PreuzmiRacun", "PrijaviDolazak", "DetaljPorudzbine", "MojRaspored",
                     "MojiBonusi", "MojaStatistika", "PrijaviProblem", "OtkaziStavkuPorudzbine" }),

            (new[] { MejlKuvar, MejlKuvar2 },
             new[] { "Prijava", "KuhinjaRedCekanja", "PreuzmiStavku", "ZavrsiStavku", "MojRaspored", "PrijaviProblem" }),

            (new[] { MejlSanker, MejlSanker2 },
             new[] { "Prijava", "SankRedCekanja", "PreuzmiStavku", "ZavrsiStavku", "MojRaspored", "MojiBonusi" }),

            (new[] { MejlMenadzer },
             new[] { "Prijava", "IzvestajPrihod", "IzvestajTopJela", "IzvestajUcinakZaposlenih", "MenadzerskiDashboard",
                     "RasporedNedelje", "KreirajSmenu", "IzmeniSmenu", "DodeliBonus", "PretraziBonuse",
                     "ListaZaNabavku", "KorigujKolicinuNamirnice", "PretraziPrijaveProblema", "ResiPrijavuProblema" }),

            (new[] { MejlAdmin },
             new[] { "Prijava", "AdministratorskiDashboard", "PretraziKorisnike", "DetaljKorisnika", "PretraziPoruke",
                     "OdgovoriNaPoruku", "OznaciPorukuProcitanom", "OdgovoriNaRecenziju", "PretraziAuditLog",
                     "PretraziAdminAkcije", "KreirajStavkuMenija", "IzmeniStavkuMenija", "PostaviSlikuStavkeMenija",
                     "DodajUGaleriju", "IzmeniPostavke", "PretraziZaposlene", "IzmeniProfilNaSajtu",
                     "KreirajAdminRezervaciju", "PretraziRezervacije" })
        };

        var nasumicno = new Random(20260722);
        var sada = DateTime.UtcNow;
        var zapisi = new List<AuditLog>();

        for (var dan = 30; dan >= 1; dan--)
        {
            var brojZaDan = nasumicno.Next(2, 5);

            for (var i = 0; i < brojZaDan; i++)
            {
                var grupa = poUlogama[nasumicno.Next(poUlogama.Length)];
                var mejl = grupa.Mejlovi[nasumicno.Next(grupa.Mejlovi.Length)];
                var slucaj = grupa.Slucajevi[nasumicno.Next(grupa.Slucajevi.Length)];

                var uspesno = nasumicno.Next(100) >= 12;

                zapisi.Add(new AuditLog
                {
                    KorisnikId = PoMejlu(mejl),
                    NazivSlucajaKoriscenja = slucaj,
                    Datum = sada.Date.AddDays(-dan).AddHours(nasumicno.Next(8, 23)).AddMinutes(nasumicno.Next(0, 60)),
                    UspesnoIzvrseno = uspesno,
                    Poruka = uspesno ? null : NeuspehZa(slucaj, nasumicno)
                });
            }
        }

        zapisi.Add(new AuditLog
        {
            KorisnikId = null, NazivSlucajaKoriscenja = "Prijava",
            Datum = sada.AddDays(-9).AddHours(-3), UspesnoIzvrseno = false,
            Poruka = "{\"Poruka\":\"Neispravni podaci za prijavu.\"}"
        });
        zapisi.Add(new AuditLog
        {
            KorisnikId = null, NazivSlucajaKoriscenja = "Registracija",
            Datum = sada.AddDays(-6).AddHours(-7), UspesnoIzvrseno = false,
            Poruka = "{\"Poruka\":\"Nalog sa ovom email adresom već postoji.\"}"
        });
        zapisi.Add(new AuditLog
        {
            KorisnikId = null, NazivSlucajaKoriscenja = "ZaboravljenaLozinka",
            Datum = sada.AddDays(-4).AddHours(-2), UspesnoIzvrseno = false,
            Poruka = "Odbijeno: prekoračen dozvoljen broj zahteva."
        });

        kontekst.AuditLogovi.AddRange(zapisi);
        await kontekst.SaveChangesAsync();
    }

    private static string NeuspehZa(string slucaj, Random nasumicno)
    {
        var poruke = slucaj switch
        {
            "Prijava" => new[] { "Neispravni podaci za prijavu.", "Nalog je blokiran." },
            "KreirajRezervaciju" => new[] { "Sto nije slobodan u traženom terminu.", "Sto nema dovoljan kapacitet za traženi broj gostiju." },
            "OtkaziRezervaciju" => new[] { "Rezervacija se može otkazati najkasnije 2h pre termina.", "Rezervacija se ne može otkazati." },
            "KreirajRecenziju" => new[] { "Već ste ostavili recenziju za ovu stavku.", "Privremeno Vam je zabranjeno komentarisanje." },
            "OtvoriPorudzbinu" => new[] { "Sto već ima otvorenu porudžbinu." },
            "ZatvoriPorudzbinu" => new[] { "Sve stavke moraju biti spremne pre zatvaranja računa." },
            "PreuzmiStavku" => new[] { "Stavka je već preuzeta." },
            "KreirajSmenu" => new[] { "Zaposleni već ima smenu u tom terminu." },
            "DodeliBonus" => new[] { "Datum kraja mora biti posle datuma početka." },
            "PrijaviDolazak" => new[] { "Sto već ima otvorenu porudžbinu.", "Rezervacija nije aktivna." },
            _ => new[] { "Traženi zapis ne postoji.", "Neispravni podaci u zahtevu." }
        };

        var poruka = poruke[nasumicno.Next(poruke.Length)];
        return $"{{\"Poruka\":\"{poruka}\",\"Greske\":{{}}}}";
    }
}
