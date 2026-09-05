using Domain.Entiteti;
using Microsoft.EntityFrameworkCore;

namespace Repository;

public static partial class PocetniPodaci
{
    private static async Task PopuniGalerijuIPostavkeAsync(RestoranDbContext kontekst)
    {
        var slike = new[]
        {
            Slika("enterijer-sala", "Sala", "Glavna sala", "Četrdeset mesta, drvo i topla svetla.", 10),
            Slika("detalj-sto", "Sala", "Detalj sa stola", "Posluženje na domaćoj keramici.", 11),
            Slika("sala-3", "Sala", "Pogled ka ulazu", "Sala gledana sa kraja, uveče.", 12),
            Slika("sala-4", "Sala", "Sto za osmoro", "Veliki sto uz prozor, za društva.", 13),

            Slika("stara-fasada", "Sala", "Stara fasada", "Pre renoviranja 2024.", 19, aktivan: false),

            Slika("basta-leto", "Bašta", "Bašta preko leta", "Četrdeset mesta u hladu, otvorena od maja do oktobra.", 20),
            Slika("basta-2", "Bašta", "Bašta uveče", "Kad se upale lampioni.", 21),
            Slika("basta-3", "Bašta", "Hlad nad stolovima", "Loza i platno, bez direktnog sunca.", 22),
            Slika("basta-4", "Bašta", "Ulaz u baštu", "Sa ulice, kroz kapiju.", 23),

            Slika("sank", "Šank", "Šank", "Domaće limunade i kafa iz italijanske mešavine.", 30),
            Slika("sank-2", "Šank", "Karta pića", "Rakije i domaća vina po čaši.", 31),
            Slika("sank-3", "Šank", "Limunade", "Sveže ceđeno, pravi se po porudžbini.", 32),
            Slika("sank-4", "Šank", "Barske stolice", "Pet mesta za one koji svrate na kratko.", 33),

            Slika("kuhinja", "Kuhinja", "Kuhinja", "Otvorena kuhinja — gosti vide kako se sprema.", 40),
            Slika("kuhinja-2", "Kuhinja", "Priprema", "Jutarnja priprema, pre otvaranja.", 41),
            Slika("kuhinja-3", "Kuhinja", "Hladna kuhinja", "Salate i dezerti, odvojen deo.", 42),
            Slika("kuhinja-4", "Kuhinja", "Peć", "Zemljani sudovi idu ovde.", 43),

            Slika("rostilj", "Roštilj", "Roštilj", "Sve sa žara, po porudžbini.", 50),
            Slika("rostilj-2", "Roštilj", "Mešano meso", "Porcija za dvoje, sa žara.", 51),
            Slika("rostilj-3", "Roštilj", "Žar izbliza", "Bukova drva, ne briketi.", 52),
            Slika("rostilj-4", "Roštilj", "Ćevapi", "Prave se svakog jutra.", 53),

            Slika("proslava", "Proslave", "Proslave", "Sala se izdvaja za rođendane i poslovne večere.", 60),
            Slika("proslava-2", "Proslave", "Postavljen sto", "Za dvadeset gostiju, u jednom komadu.", 61),
            Slika("proslava-3", "Proslave", "Torta", "Dezert se dogovara unapred.", 62),
            Slika("proslava-4", "Proslave", "Izdvojeni deo", "Zadnji deo sale, sa svojim ulazom.", 63)
        };

        foreach (var slika in slike)
        {
            if (!await kontekst.GalerijaSlike.AnyAsync(g => g.SlikaUrl == slika.SlikaUrl))
                kontekst.GalerijaSlike.Add(slika);
        }
        await kontekst.SaveChangesAsync();

        if (await kontekst.PostavkeRestorana.AnyAsync()) return;

        kontekst.PostavkeRestorana.Add(new PostavkeRestorana
        {
            Id = 1,
            Adresa = "Bulevar oslobođenja 42, Novi Sad",
            Telefon = "021/555-123",
            Email = "kontakt@ognjiste.rs",
            RadnoVreme = "Pon–Čet 08:00–24:00, Pet–Sub 08:00–01:00, Ned 10:00–23:00",
            OpisRestorana = "Kuvamo ono što bismo dali svojima: čorba se kuva od jutros, meso ide " +
                            "na žar po porudžbini, a sarma i đuveč iz zemljanog suda čekaju koliko " +
                            "treba. Namirnice stižu od dobavljača sa kojima radimo godinama. " +
                            "Bašta sa 40 mesta radi od aprila do prvih hladnoća.",
            GeoSirina = 45.253300m,
            GeoDuzina = 19.842600m,
            FacebookUrl = "https://www.facebook.com/",
            InstagramUrl = "https://www.instagram.com/"
        });

        await kontekst.SaveChangesAsync();
    }

    private static GalerijaSlika Slika(
        string datoteka, string grupa, string naslov, string opis, int redosled, bool aktivan = true) => new()
        {
            SlikaUrl = $"/slike/galerija/{datoteka}.jpg",
            Grupa = grupa,
            Naslov = naslov,
            Opis = opis,
            Redosled = redosled,
            Aktivan = aktivan
        };
}
