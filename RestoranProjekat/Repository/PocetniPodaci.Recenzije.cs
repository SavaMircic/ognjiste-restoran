using Domain.Entiteti;
using Domain.Enumi;
using Microsoft.EntityFrameworkCore;

namespace Repository;

public static partial class PocetniPodaci
{
    private static async Task PopuniRecenzijeILajkoveAsync(RestoranDbContext kontekst)
    {
        if (await kontekst.Recenzije.AnyAsync()) return;

        var gosti = await kontekst.Users
            .Where(k => k.Email!.EndsWith("@test.com"))
            .ToDictionaryAsync(k => k.Email!, k => k.Id);

        var jela = await kontekst.StavkeMenija.ToDictionaryAsync(s => s.Naziv, s => s.Id);
        if (gosti.Count == 0 || jela.Count == 0) return;

        var sada = DateTime.UtcNow;
        var recenzije = new List<Recenzija>();

        void Dodaj(string mejl, TipRecenzije tip, string? jelo, int ocena, string naslov, string tekst,
            int danaUnazad, string? odgovor = null, bool aktivan = true)
        {
            if (!gosti.TryGetValue(mejl, out var korisnikId)) return;

            int? jeloId = null;
            if (jelo != null)
            {
                if (!jela.TryGetValue(jelo, out var id)) return;
                jeloId = id;
            }

            recenzije.Add(new Recenzija
            {
                KorisnikId = korisnikId,
                TipRecenzije = tip,
                StavkaMenijaId = jeloId,
                Ocena = ocena,
                Naslov = naslov,
                Tekst = tekst,
                DatumKreiranja = sada.AddDays(-danaUnazad),
                Aktivan = aktivan,
                OdgovorRestorana = odgovor,
                DatumOdgovora = odgovor != null ? sada.AddDays(-danaUnazad + 1) : null
            });
        }

        Dodaj("petar@test.com", TipRecenzije.Jelo, "Šopska salata", 5, "Sveže i ukusno",
            "Najbolja šopska u gradu, salata je sveža a porcija velika.", 40);
        Dodaj("ana@test.com", TipRecenzije.Jelo, "Šopska salata", 4, "Dobra, ali malo sira",
            "Ukusna je, samo bih volela malo više fete.", 33);
        Dodaj("marko@test.com", TipRecenzije.Jelo, "Šopska salata", 5, "Standard",
            "Naručujem je svaki put i nikad nije razočarala.", 12);

        Dodaj("petar@test.com", TipRecenzije.Jelo, "Karađorđeva šnicla", 5, "Ogromna porcija",
            "Dvoje smo se najeli od jedne. Kajmak je pravi, ne onaj kupovni.", 36);
        Dodaj("jelena@test.com", TipRecenzije.Jelo, "Karađorđeva šnicla", 4, "Odlična, malo masna",
            "Ukus je vrhunski, ali je teška za veče. Sledeći put uz salatu.", 22);
        Dodaj("stefan@test.com", TipRecenzije.Jelo, "Karađorđeva šnicla", 5, "Vredna svake pare",
            "Za ovu cenu ovakva porcija je retkost.", 8);

        Dodaj("ana@test.com", TipRecenzije.Jelo, "Ćevapi (10 kom)", 4, "Dobri, malo slaniji",
            "Meso je mekano, ali malo previše slano za moj ukus.", 30,
            odgovor: "Hvala na povratnoj informaciji — preneli smo kuhinji, radimo na doslednosti začina.");
        Dodaj("marko@test.com", TipRecenzije.Jelo, "Ćevapi (10 kom)", 5, "Kao nekad",
            "Lepinja topla, luk sitno seckan, kajmak taman koliko treba.", 18);
        Dodaj("milica@test.com", TipRecenzije.Jelo, "Ćevapi (10 kom)", 3, "Očekivala sam više",
            "Nisu loši, ali sam probala bolje. Lepinja je bila suva.", 5);

        Dodaj("petar@test.com", TipRecenzije.Jelo, "Pileća supa", 5, "Prava domaća",
            "Kuvana od jutros, oseti se razlika.", 27);
        Dodaj("tijana@test.com", TipRecenzije.Jelo, "Pileća supa", 4, "Dobra",
            "Solidna supa, rezanci malo prekuvani.", 14);

        Dodaj("jelena@test.com", TipRecenzije.Jelo, "Palačinke sa Nutellom", 5, "Savršen kraj obroka",
            "Tanke, tople i taman slatke. Deca su oduševljena.", 25);
        Dodaj("ana@test.com", TipRecenzije.Jelo, "Palačinke sa Nutellom", 5, "Uvek iste, uvek dobre",
            "Poručujem ih godinama i nikad nisu podbacile.", 10);

        Dodaj("stefan@test.com", TipRecenzije.Jelo, "Mešano meso", 5, "Za dvoje i ostane",
            "Porcija je ogromna, sve sveže sa žara.", 20);
        Dodaj("marko@test.com", TipRecenzije.Jelo, "Mešano meso", 4, "Dobro, ali čekali smo",
            "Meso odlično, samo je priprema trajala skoro 40 minuta.", 9);

        Dodaj("milica@test.com", TipRecenzije.Jelo, "Baklava", 5, "Domaća, ne kupovna",
            "Odmah se oseti da je pravljena u kući. Orasi, ne mrvice.", 16);
        Dodaj("petar@test.com", TipRecenzije.Jelo, "Baklava", 4, "Slatka, ali dobra",
            "Za moj ukus malo preslatka, ali kvalitet je tu.", 6);

        Dodaj("jelena@test.com", TipRecenzije.Jelo, "Domaća limunada", 5, "Osveženje",
            "Sveže ceđena, sa nanom. Preko leta obavezna.", 11);
        Dodaj("tijana@test.com", TipRecenzije.Jelo, "Domaća limunada", 5, "Bolja od svih gaziranih",
            "Više je ni ne menjam za kolu.", 4);

        Dodaj("ana@test.com", TipRecenzije.Jelo, "Krempita", 3, "Prosečna",
            "Listovi nisu bili hrskavi, verovatno je stajala.", 13);
        Dodaj("stefan@test.com", TipRecenzije.Jelo, "Pljeskavica", 4, "Solidna",
            "Punjena kačkavaljem, sočna. Ajvar bi mogao biti ljući.", 7);
        Dodaj("marko@test.com", TipRecenzije.Jelo, "Punjena piletina", 5, "Prijatno iznenađenje",
            "Nisam očekivao ovoliko ukusa od piletine.", 15);
        Dodaj("petar@test.com", TipRecenzije.Jelo, "Riblja čorba", 2, "Previše ljuta",
            "Nisu me pitali koliko ljuto, a bila je nejestivo ljuta za mene.", 19,
            odgovor: "Izvinjavamo se — ljutina se sada obavezno proverava pri poručivanju.");
        Dodaj("milica@test.com", TipRecenzije.Jelo, "Đuveč", 4, "Dobra posna opcija",
            "Retko gde nađem pristojan posni obrok, ovde da.", 17);

        Dodaj("petar@test.com", TipRecenzije.Usluga, null, 5, "Brza usluga",
            "Konobari su ljubazni i brzi, preporučujem.", 35);
        Dodaj("ana@test.com", TipRecenzije.Usluga, null, 4, "Ljubazno osoblje",
            "Sve pohvale za ekipu u sali, samo je vikendom gužva.", 21);
        Dodaj("jelena@test.com", TipRecenzije.Usluga, null, 2, "Dugo čekanje",
            "Čekali smo predjelo skoro 40 minuta, bez izvinjenja.", 26,
            odgovor: "Žao nam je zbog čekanja. Te večeri smo imali dve grupe odjednom i pojačali smo smenu.");
        Dodaj("stefan@test.com", TipRecenzije.Usluga, null, 5, "Vrhunski",
            "Konobar je zapamtio šta smo poručili prošli put. To se ceni.", 3);

        Dodaj("marko@test.com", TipRecenzije.Restoran, null, 5, "Omiljeno mesto",
            "Dolazimo redovno već godinu dana. Ambijent, hrana i cena su u savršenom odnosu.", 29);
        Dodaj("milica@test.com", TipRecenzije.Restoran, null, 4, "Dobro, ali bučno",
            "Hrana odlična, samo je vikendom preglasno za razgovor.", 23);
        Dodaj("tijana@test.com", TipRecenzije.Restoran, null, 5, "Bašta je savršena",
            "Leti nema boljeg mesta u gradu. Hlad, mir i dobra hrana.", 2);

        Dodaj("vuk@test.com", TipRecenzije.Restoran, null, 1, "Ne preporučujem",
            "Uklonjeno zbog nepristojnog rečnika.", 24, aktivan: false);

        kontekst.Recenzije.AddRange(recenzije);

        var lajkovi = new (string Mejl, string Jelo, int DanaUnazad)[]
        {
            ("petar@test.com", "Karađorđeva šnicla", 30), ("petar@test.com", "Ćevapi (10 kom)", 22),
            ("petar@test.com", "Baklava", 6), ("ana@test.com", "Šopska salata", 28),
            ("ana@test.com", "Palačinke sa Nutellom", 10), ("marko@test.com", "Mešano meso", 20),
            ("marko@test.com", "Ćevapi (10 kom)", 18), ("marko@test.com", "Karađorđeva šnicla", 9),
            ("jelena@test.com", "Palačinke sa Nutellom", 25), ("jelena@test.com", "Domaća limunada", 11),
            ("stefan@test.com", "Mešano meso", 19), ("stefan@test.com", "Karađorđeva šnicla", 8),
            ("stefan@test.com", "Pljeskavica", 7), ("milica@test.com", "Baklava", 16),
            ("milica@test.com", "Đuveč", 17), ("tijana@test.com", "Domaća limunada", 4),
            ("tijana@test.com", "Pileća supa", 14), ("vuk@test.com", "Ćevapi (10 kom)", 21)
        };

        foreach (var (mejl, jelo, danaUnazad) in lajkovi)
        {
            if (!gosti.TryGetValue(mejl, out var korisnikId)) continue;
            if (!jela.TryGetValue(jelo, out var jeloId)) continue;

            kontekst.LajkoviJela.Add(new LajkJela
            {
                KorisnikId = korisnikId,
                StavkaMenijaId = jeloId,
                DatumKreiranja = sada.AddDays(-danaUnazad)
            });
        }

        await kontekst.SaveChangesAsync();
    }
}
