using Domain.Entiteti;
using Domain.Konstante;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Repository;

public static partial class PocetniPodaci
{
    private const string MejlAdmin = "admin@restoran.com";
    private const string MejlMenadzer = "menadzer@restoran.com";
    private const string MejlKonobar = "konobar@restoran.com";
    private const string MejlKonobar2 = "konobar2@restoran.com";
    private const string MejlSanker = "sanker@restoran.com";
    private const string MejlSanker2 = "sanker2@restoran.com";
    private const string MejlKuvar = "kuvar@restoran.com";
    private const string MejlKuvar2 = "kuvar2@restoran.com";

    private static async Task PopuniOsobljeAsync(UserManager<Korisnik> userManager, RestoranDbContext kontekst)
    {
        var sada = DateTime.UtcNow;

        await KreirajKorisnikaAsync(userManager, kontekst, MejlAdmin, "Admin1234", "Ana", "Popović", Uloge.Administrator, true, sada.AddYears(-4));
        await KreirajKorisnikaAsync(userManager, kontekst, MejlMenadzer, "Menadzer1234", "Mina", "Jovanović", Uloge.Menadzer, true, sada.AddYears(-3));
        await KreirajKorisnikaAsync(userManager, kontekst, MejlKonobar, "Konobar1234", "Kosta", "Ilić", Uloge.Konobar, true, sada.AddYears(-2));
        await KreirajKorisnikaAsync(userManager, kontekst, MejlKonobar2, "Konobar1234", "Nikola", "Radovanović", Uloge.Konobar, true, sada.AddMonths(-14));
        await KreirajKorisnikaAsync(userManager, kontekst, MejlSanker, "Sanker1234", "Sanja", "Đurić", Uloge.Sanker, true, sada.AddYears(-2));
        await KreirajKorisnikaAsync(userManager, kontekst, MejlSanker2, "Sanker1234", "Bojan", "Stanković", Uloge.Sanker, true, sada.AddMonths(-8));
        await KreirajKorisnikaAsync(userManager, kontekst, MejlKuvar, "Kuvar1234", "Miloš", "Vasić", Uloge.Kuvar, true, sada.AddYears(-3));
        await KreirajKorisnikaAsync(userManager, kontekst, MejlKuvar2, "Kuvar1234", "Marija", "Simić", Uloge.Kuvar, true, sada.AddMonths(-10));
    }

    private static async Task PopuniGosteAsync(UserManager<Korisnik> userManager, RestoranDbContext kontekst)
    {
        var sada = DateTime.UtcNow;

        await KreirajKorisnikaAsync(userManager, kontekst, "petar@test.com", "Korisnik1234", "Petar", "Lazić", Uloge.Korisnik, false, sada.AddMonths(-11));
        await KreirajKorisnikaAsync(userManager, kontekst, "ana@test.com", "Korisnik1234", "Ana", "Kovačević", Uloge.Korisnik, false, sada.AddMonths(-9));
        await KreirajKorisnikaAsync(userManager, kontekst, "marko@test.com", "Korisnik1234", "Marko", "Đorđević", Uloge.Korisnik, false, sada.AddMonths(-7));
        await KreirajKorisnikaAsync(userManager, kontekst, "jelena@test.com", "Korisnik1234", "Jelena", "Pavlović", Uloge.Korisnik, false, sada.AddMonths(-5));
        await KreirajKorisnikaAsync(userManager, kontekst, "stefan@test.com", "Korisnik1234", "Stefan", "Ristić", Uloge.Korisnik, false, sada.AddMonths(-4));
        await KreirajKorisnikaAsync(userManager, kontekst, "milica@test.com", "Korisnik1234", "Milica", "Tomić", Uloge.Korisnik, false, sada.AddMonths(-2));

        var dusan = await KreirajKorisnikaAsync(userManager, kontekst, "dusan@test.com", "Korisnik1234", "Dušan", "Savić", Uloge.Korisnik, false, sada.AddMonths(-6));
        var tijana = await KreirajKorisnikaAsync(userManager, kontekst, "tijana@test.com", "Korisnik1234", "Tijana", "Babić", Uloge.Korisnik, false, sada.AddMonths(-3));
        var vuk = await KreirajKorisnikaAsync(userManager, kontekst, "vuk@test.com", "Korisnik1234", "Vuk", "Perić", Uloge.Korisnik, false, sada.AddMonths(-8));

        var promenjeno = false;

        if (dusan is { Aktivan: true })
        {
            dusan.Aktivan = false;
            promenjeno = true;
        }

        if (tijana is { BlokiranDo: null })
        {
            tijana.BlokiranDo = sada.AddDays(9);
            promenjeno = true;
        }

        if (vuk is { ZabranaKomentarisanjaDo: null })
        {
            vuk.ZabranaKomentarisanjaDo = sada.AddDays(20);
            promenjeno = true;
        }

        if (promenjeno) await kontekst.SaveChangesAsync();

        await PopuniProfilneSlikeGostijuAsync(kontekst);
    }

    private static async Task PopuniProfilneSlikeGostijuAsync(RestoranDbContext kontekst)
    {
        var slike = new (string Mejl, string Slika)[]
        {
            ("petar@test.com", "/slike/profilne/petar-lazic.jpg"),
            ("ana@test.com", "/slike/profilne/ana-kovacevic.jpg"),
            ("marko@test.com", "/slike/profilne/marko-djordjevic.jpg"),
            ("jelena@test.com", "/slike/profilne/jelena-pavlovic.jpg")
        };

        var promenjeno = false;

        foreach (var (mejl, putanja) in slike)
        {
            var korisnik = await kontekst.Users.FirstOrDefaultAsync(k => k.Email == mejl);

            if (korisnik == null || korisnik.SlikaUrl != null) continue;

            korisnik.SlikaUrl = putanja;
            promenjeno = true;
        }

        if (promenjeno) await kontekst.SaveChangesAsync();
    }

    private static async Task PopuniProfileOsobljaAsync(RestoranDbContext kontekst)
    {
        var profili = new (string Mejl, string Slika, string Biografija, string? LinkedIn, string? Instagram, int Redosled, bool NaSajtu)[]
        {
            (MejlMenadzer, "/slike/zaposleni/mina-jovanovic.jpg",
                "Vodi restoran od otvaranja. Diplomirala je hotelijerstvo i verovala da se dobar servis meri time koliko se gost oseća očekivano, a ne koliko brzo dobije račun.",
                "https://www.linkedin.com/", "https://www.instagram.com/", 1, true),

            (MejlKuvar, "/slike/zaposleni/milos-vasic.jpg",
                "Glavni kuvar. Petnaest godina za šporetom, od toga sedam u Beču. Insistira da se čorba kuva od jutros, ne od juče, i zbog toga se povremeno svađa sa nabavkom.",
                null, "https://www.instagram.com/", 2, true),

            (MejlKuvar2, "/slike/zaposleni/marija-simic.jpg",
                "Kuvarica na hladnoj kuhinji i dezertima. Njena baklava je razlog zbog kog gosti ostavljaju mesta posle glavnog jela.",
                null, "https://www.instagram.com/", 3, true),

            (MejlKonobar, "/slike/zaposleni/kosta-ilic.jpg",
                "Šef sale. Pamti šta ste poručili prošli put i koliko ste čekali, pa se trudi da drugi put bude brže.",
                "https://www.linkedin.com/", null, 4, true),

            (MejlSanker, "/slike/zaposleni/sanja-djuric.jpg",
                "Za šankom od prvog dana. Napravila je kartu domaćih limunada koja je preko leta prodavanija od svih gaziranih pića zajedno.",
                null, "https://www.instagram.com/", 5, true),

            (MejlKonobar2, "/slike/zaposleni/nikola-radovanovic.jpg",
                "Konobar u popodnevnoj smeni. Radi i na terasi, koja preko leta ume da bude veća od cele sale.",
                null, null, 6, true),

            (MejlSanker2, "/slike/zaposleni/bojan-stankovic.jpg",
                "Šanker u noćnoj smeni. Došao je na ispomoć preko leta i ostao — sad drži petak i subotu do dva.",
                null, null, 7, true),

            (MejlAdmin, "/slike/zaposleni/ana-popovic.jpg",
                "Administrator sistema. Održava sajt, naloge i sadržaj — jedina u timu koju nećete sresti u sali.",
                "https://www.linkedin.com/", null, 8, true)
        };

        var promenjeno = false;

        foreach (var profil in profili)
        {
            var zaposleni = await ZaposleniPoMejluAsync(kontekst, profil.Mejl);

            if (zaposleni == null || zaposleni.Biografija != null) continue;

            zaposleni.SlikaUrl = profil.Slika;
            zaposleni.Biografija = profil.Biografija;
            zaposleni.LinkedInUrl = profil.LinkedIn;
            zaposleni.InstagramUrl = profil.Instagram;
            zaposleni.Redosled = profil.Redosled;
            zaposleni.PrikaziNaSajtu = profil.NaSajtu;
            promenjeno = true;
        }

        if (promenjeno) await kontekst.SaveChangesAsync();
    }
}
