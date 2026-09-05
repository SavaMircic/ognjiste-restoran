using Domain.Entiteti;
using Domain.Enumi;
using Microsoft.EntityFrameworkCore;

namespace Repository;

public static partial class PocetniPodaci
{
    private static async Task PopuniPorukeAsync(RestoranDbContext kontekst)
    {
        if (await kontekst.Poruke.AnyAsync()) return;

        var gosti = await kontekst.Users
            .Where(k => k.Email!.EndsWith("@test.com"))
            .ToDictionaryAsync(k => k.Email!, k => k.Id);

        var admin = await ZaposleniPoMejluAsync(kontekst, MejlAdmin);
        var sada = DateTime.UtcNow;
        var poruke = new List<Poruka>();

        void OdKorisnika(string mejl, KategorijaPoruke kategorija, string tekst, double satiUnazad,
            StatusPoruke status, string? odgovor = null, DateTime? zeljeniTermin = null, int? zeljeniBroj = null)
        {
            if (!gosti.TryGetValue(mejl, out var korisnikId)) return;

            poruke.Add(new Poruka
            {
                KorisnikId = korisnikId,
                Kategorija = kategorija,
                Tekst = tekst,
                DatumSlanja = sada.AddHours(-satiUnazad),
                Status = status,
                Odgovor = odgovor,
                DatumOdgovora = odgovor != null ? sada.AddHours(-satiUnazad + 4) : null,
                ObradioZaposleniId = odgovor != null ? admin?.Id : null,
                ZeljeniDatumVreme = zeljeniTermin,
                ZeljeniBrojGostiju = zeljeniBroj
            });
        }

        void OdGosta(string ime, string mejl, string telefon, KategorijaPoruke kategorija, string tekst,
            double satiUnazad, StatusPoruke status, string? odgovor = null,
            DateTime? zeljeniTermin = null, int? zeljeniBroj = null)
        {
            poruke.Add(new Poruka
            {
                GostIme = ime,
                GostEmail = mejl,
                GostTelefon = telefon,
                Kategorija = kategorija,
                Tekst = tekst,
                DatumSlanja = sada.AddHours(-satiUnazad),
                Status = status,
                Odgovor = odgovor,
                DatumOdgovora = odgovor != null ? sada.AddHours(-satiUnazad + 4) : null,
                ObradioZaposleniId = odgovor != null ? admin?.Id : null,
                ZeljeniDatumVreme = zeljeniTermin,
                ZeljeniBrojGostiju = zeljeniBroj
            });
        }

        OdKorisnika("petar@test.com", KategorijaPoruke.Pitanje,
            "Da li imate parking za goste restorana?", 5, StatusPoruke.Novo);
        OdGosta("Ivana Krstić", "ivana.krstic@example.com", "0631234567", KategorijaPoruke.Pitanje,
            "Da li je bašta natkrivena ako pada kiša?", 30, StatusPoruke.Procitano);
        OdKorisnika("marko@test.com", KategorijaPoruke.Pitanje,
            "Imate li jela bez glutena i da li su označena u meniju?", 74, StatusPoruke.Odgovoreno,
            odgovor: "Trenutno nemamo posebnu oznaku u meniju, ali kuhinja može da prilagodi većinu jela — javite se konobaru.");

        OdKorisnika("ana@test.com", KategorijaPoruke.Sugestija,
            "Bilo bi lepo da uvedete dečji meni sa manjim porcijama.", 20, StatusPoruke.Novo);
        OdKorisnika("jelena@test.com", KategorijaPoruke.Sugestija,
            "Predlažem da limunadu radite i u bokalu od litra, za društvo.", 96, StatusPoruke.Odgovoreno,
            odgovor: "Odlična ideja — od sledeće nedelje limunada je i u bokalu. Hvala!");
        OdGosta("Nemanja Vuković", "nemanja.vukovic@example.com", "0692223334", KategorijaPoruke.Sugestija,
            "Muzika je vikendom preglasna, teško je razgovarati za stolom.", 50, StatusPoruke.Procitano);

        OdGosta("Jovan Mitrović", "jovan.mitrovic@example.com", "0651112223", KategorijaPoruke.Rezervacija,
            "Želeo bih da rezervišem sto za proslavu rođendana, ako je moguće u bašti.", 3, StatusPoruke.Novo,
            zeljeniTermin: sada.AddDays(5).Date.AddHours(20), zeljeniBroj: 8);
        OdKorisnika("stefan@test.com", KategorijaPoruke.Rezervacija,
            "Treba mi sto za poslovnu večeru, po mogućstvu odvojen od glavne sale.", 8, StatusPoruke.Novo,
            zeljeniTermin: sada.AddDays(3).Date.AddHours(19), zeljeniBroj: 6);
        OdGosta("Dragan Blagojević", "dragan.blagojevic@example.com", "0698889990", KategorijaPoruke.Rezervacija,
            "Rezervacija za deset osoba, potvrdite molim vas telefonom.", 46, StatusPoruke.Odgovoreno,
            odgovor: "Potvrđeno telefonom, sto 12 rezervisan. Vidimo se!",
            zeljeniTermin: sada.AddDays(1).Date.AddHours(20), zeljeniBroj: 10);

        OdGosta("Sonja Rakić", "sonja.rakic@example.com", "0644445556", KategorijaPoruke.Zalba,
            "Čekali smo predjelo skoro 40 minuta prošle subote, niko se nije izvinio.", 60, StatusPoruke.Procitano);
        OdKorisnika("milica@test.com", KategorijaPoruke.Zalba,
            "Račun je bio pogrešno sabran, naplaćeno mi je jedno piće više.", 26, StatusPoruke.Odgovoreno,
            odgovor: "Proverili smo račun, greška je naša — razlika Vam je vraćena. Izvinjavamo se.");
        OdKorisnika("tijana@test.com", KategorijaPoruke.Zalba,
            "Toalet nije bio uredan u petak uveče.", 14, StatusPoruke.Novo);

        OdKorisnika("ana@test.com", KategorijaPoruke.Pohvala,
            "Osoblje je bilo izuzetno ljubazno, sve pohvale!", 120, StatusPoruke.Odgovoreno,
            odgovor: "Hvala Vam puno, preneli smo pohvale timu!");
        OdKorisnika("petar@test.com", KategorijaPoruke.Pohvala,
            "Kuvar je izašao da pita da li je sve u redu. Mali detalj, veliki utisak.", 36, StatusPoruke.Procitano);

        kontekst.Poruke.AddRange(poruke);
        await kontekst.SaveChangesAsync();
    }
}
