using Domain.Entiteti;
using Domain.Enumi;
using Domain.Konstante;
using Microsoft.EntityFrameworkCore;

namespace Repository;

public static partial class PocetniPodaci
{
    private static async Task PopuniStoloveAsync(RestoranDbContext kontekst)
    {
        var stolovi = new[]
        {
            new Sto { BrojStola = 1, Kapacitet = 2, TrenutniStatus = StatusStola.Zauzet },
            new Sto { BrojStola = 2, Kapacitet = 2, TrenutniStatus = StatusStola.Slobodan },
            new Sto { BrojStola = 3, Kapacitet = 4, TrenutniStatus = StatusStola.Slobodan },
            new Sto { BrojStola = 4, Kapacitet = 4, TrenutniStatus = StatusStola.Zauzet },
            new Sto { BrojStola = 5, Kapacitet = 4, TrenutniStatus = StatusStola.Slobodan },
            new Sto { BrojStola = 6, Kapacitet = 6, TrenutniStatus = StatusStola.Slobodan },
            new Sto { BrojStola = 7, Kapacitet = 6, TrenutniStatus = StatusStola.Slobodan },
            new Sto { BrojStola = 8, Kapacitet = 8, TrenutniStatus = StatusStola.Slobodan },
            new Sto { BrojStola = 9, Kapacitet = 2, TrenutniStatus = StatusStola.Slobodan },
            new Sto { BrojStola = 10, Kapacitet = 4, TrenutniStatus = StatusStola.Slobodan },
            new Sto { BrojStola = 11, Kapacitet = 4, TrenutniStatus = StatusStola.Slobodan },
            new Sto { BrojStola = 12, Kapacitet = 10, TrenutniStatus = StatusStola.Slobodan },
            new Sto { BrojStola = 13, Kapacitet = 6, TrenutniStatus = StatusStola.Slobodan }
        };

        foreach (var sto in stolovi)
        {
            if (!await kontekst.Stolovi.AnyAsync(s => s.BrojStola == sto.BrojStola))
                kontekst.Stolovi.Add(sto);
        }

        await kontekst.SaveChangesAsync();
    }

    private const int BrojDanaIstorije = 45;

    private static async Task PopuniRezervacijeAsync(RestoranDbContext kontekst)
    {
        if (await kontekst.Rezervacije.AnyAsync()) return;

        var gosti = await kontekst.Users
            .Where(k => k.Email!.EndsWith("@test.com"))
            .ToDictionaryAsync(k => k.Email!, k => k.Id);

        var stolovi = await kontekst.Stolovi.OrderBy(s => s.BrojStola).ToListAsync();
        if (stolovi.Count == 0 || gosti.Count == 0) return;

        var admin = await ZaposleniPoMejluAsync(kontekst, MejlAdmin);
        var nasumicno = new Random(20260901);
        var sada = DateTime.UtcNow;
        var rezervacije = new List<Rezervacija>();

        var istorijski = new (string Mejl, int DanaUnazad, int Sat, int BrojGostiju, StatusRezervacije Status, NacinKreiranjaRezervacije Nacin)[]
        {
            ("petar@test.com", 42, 19, 2, StatusRezervacije.Realizovana, NacinKreiranjaRezervacije.Samostalno),
            ("ana@test.com", 38, 20, 4, StatusRezervacije.Realizovana, NacinKreiranjaRezervacije.Samostalno),
            ("marko@test.com", 35, 18, 6, StatusRezervacije.Realizovana, NacinKreiranjaRezervacije.TelefonAdmin),
            ("petar@test.com", 31, 21, 2, StatusRezervacije.Istekla, NacinKreiranjaRezervacije.Samostalno),
            ("jelena@test.com", 28, 19, 4, StatusRezervacije.Realizovana, NacinKreiranjaRezervacije.Samostalno),
            ("stefan@test.com", 26, 20, 8, StatusRezervacije.Realizovana, NacinKreiranjaRezervacije.KontaktForma),
            ("ana@test.com", 24, 18, 2, StatusRezervacije.Otkazana, NacinKreiranjaRezervacije.Samostalno),
            ("milica@test.com", 21, 21, 4, StatusRezervacije.Realizovana, NacinKreiranjaRezervacije.Samostalno),
            ("marko@test.com", 19, 19, 2, StatusRezervacije.Istekla, NacinKreiranjaRezervacije.Samostalno),
            ("petar@test.com", 17, 20, 4, StatusRezervacije.Realizovana, NacinKreiranjaRezervacije.Samostalno),
            ("tijana@test.com", 15, 18, 6, StatusRezervacije.Realizovana, NacinKreiranjaRezervacije.TelefonAdmin),
            ("jelena@test.com", 13, 21, 2, StatusRezervacije.Otkazana, NacinKreiranjaRezervacije.Samostalno),
            ("ana@test.com", 11, 19, 4, StatusRezervacije.Realizovana, NacinKreiranjaRezervacije.Samostalno),
            ("stefan@test.com", 9, 20, 10, StatusRezervacije.Realizovana, NacinKreiranjaRezervacije.KontaktForma),
            ("vuk@test.com", 7, 18, 2, StatusRezervacije.Istekla, NacinKreiranjaRezervacije.Samostalno),
            ("petar@test.com", 6, 21, 4, StatusRezervacije.Realizovana, NacinKreiranjaRezervacije.Samostalno),
            ("milica@test.com", 4, 19, 6, StatusRezervacije.Realizovana, NacinKreiranjaRezervacije.Samostalno),
            ("marko@test.com", 3, 20, 2, StatusRezervacije.Otkazana, NacinKreiranjaRezervacije.Samostalno),
            ("ana@test.com", 2, 18, 4, StatusRezervacije.Realizovana, NacinKreiranjaRezervacije.Samostalno)
        };

        foreach (var stavka in istorijski)
        {
            if (!gosti.TryGetValue(stavka.Mejl, out var korisnikId)) continue;

            var sto = stolovi.Where(s => s.Kapacitet >= stavka.BrojGostiju)
                .OrderBy(_ => nasumicno.Next()).FirstOrDefault() ?? stolovi[^1];

            rezervacije.Add(new Rezervacija
            {
                KorisnikId = korisnikId,
                StoId = sto.Id,
                DatumVreme = sada.Date.AddDays(-stavka.DanaUnazad).AddHours(stavka.Sat).AddMinutes(nasumicno.Next(0, 4) * 15),
                BrojGostiju = stavka.BrojGostiju,
                KodRezervacije = Kod(nasumicno),
                Status = stavka.Status,
                NacinKreiranja = stavka.Nacin,
                KreiraoZaposleniId = stavka.Nacin == NacinKreiranjaRezervacije.Samostalno ? null : admin?.Id,
                DatumKreiranja = sada.Date.AddDays(-stavka.DanaUnazad - nasumicno.Next(1, 6))
            });
        }

        var gostiBezNaloga = new (string Ime, string Mejl, string Telefon, int DanaUnazad, int Sat, int Broj, StatusRezervacije Status, NacinKreiranjaRezervacije Nacin)[]
        {
            ("Jovan Mitrović", "jovan.mitrovic@example.com", "0651112223", 20, 20, 6, StatusRezervacije.Realizovana, NacinKreiranjaRezervacije.TelefonAdmin),
            ("Sara Kostić", "sara.kostic@example.com", "0644445556", 12, 19, 4, StatusRezervacije.Istekla, NacinKreiranjaRezervacije.KontaktForma),
            ("Nenad Ivanović", "nenad.ivanovic@example.com", "0637778889", 5, 21, 2, StatusRezervacije.Realizovana, NacinKreiranjaRezervacije.TelefonAdmin)
        };

        foreach (var gost in gostiBezNaloga)
        {
            var sto = stolovi.Where(s => s.Kapacitet >= gost.Broj)
                .OrderBy(_ => nasumicno.Next()).FirstOrDefault() ?? stolovi[^1];

            rezervacije.Add(new Rezervacija
            {
                GostIme = gost.Ime,
                GostEmail = gost.Mejl,
                GostTelefon = gost.Telefon,
                StoId = sto.Id,
                DatumVreme = sada.Date.AddDays(-gost.DanaUnazad).AddHours(gost.Sat),
                BrojGostiju = gost.Broj,
                KodRezervacije = Kod(nasumicno),
                Status = gost.Status,
                NacinKreiranja = gost.Nacin,
                KreiraoZaposleniId = admin?.Id,
                DatumKreiranja = sada.Date.AddDays(-gost.DanaUnazad - 2)
            });
        }

        var buduce = new (string? Mejl, string? GostIme, string? GostMejl, string? GostTelefon, double SatiUnapred, int Broj, NacinKreiranjaRezervacije Nacin)[]
        {
            ("milica@test.com", null, null, null, 0.83, 2, NacinKreiranjaRezervacije.Samostalno),
            ("petar@test.com", null, null, null, 3, 2, NacinKreiranjaRezervacije.Samostalno),
            ("ana@test.com", null, null, null, 6, 4, NacinKreiranjaRezervacije.Samostalno),
            ("marko@test.com", null, null, null, 27, 6, NacinKreiranjaRezervacije.Samostalno),
            ("jelena@test.com", null, null, null, 52, 2, NacinKreiranjaRezervacije.Samostalno),
            (null, "Dragan Blagojević", "dragan.blagojevic@example.com", "0698889990", 30, 10, NacinKreiranjaRezervacije.TelefonAdmin),
            ("stefan@test.com", null, null, null, 100, 8, NacinKreiranjaRezervacije.KontaktForma)
        };

        foreach (var stavka in buduce)
        {
            var sto = stolovi.Where(s => s.Kapacitet >= stavka.Broj)
                .OrderBy(_ => nasumicno.Next()).FirstOrDefault() ?? stolovi[^1];

            string? korisnikId = null;
            if (stavka.Mejl != null && !gosti.TryGetValue(stavka.Mejl, out korisnikId)) continue;

            rezervacije.Add(new Rezervacija
            {
                KorisnikId = korisnikId,
                GostIme = stavka.GostIme,
                GostEmail = stavka.GostMejl,
                GostTelefon = stavka.GostTelefon,
                StoId = sto.Id,
                DatumVreme = sada.AddHours(stavka.SatiUnapred),
                BrojGostiju = stavka.Broj,
                KodRezervacije = Kod(nasumicno),
                Status = StatusRezervacije.Aktivna,
                NacinKreiranja = stavka.Nacin,
                KreiraoZaposleniId = stavka.Nacin == NacinKreiranjaRezervacije.Samostalno ? null : admin?.Id,
                DatumKreiranja = sada.AddDays(-nasumicno.Next(1, 8))
            });
        }

        kontekst.Rezervacije.AddRange(rezervacije);
        await kontekst.SaveChangesAsync();

        var granica = sada.AddHours(1);
        var uskoro = rezervacije
            .Where(r => r.Status == StatusRezervacije.Aktivna && r.DatumVreme <= granica)
            .Select(r => r.StoId)
            .ToHashSet();

        if (uskoro.Count > 0)
        {
            foreach (var sto in stolovi.Where(s => uskoro.Contains(s.Id) && s.TrenutniStatus == StatusStola.Slobodan))
                sto.TrenutniStatus = StatusStola.Rezervisan;

            await kontekst.SaveChangesAsync();
        }
    }

    private const string KarakteriKoda = "ABCDEFGHJKLMNPQRSTUVWXYZ23456789";

    private static string Kod(Random nasumicno) =>
        new(Enumerable.Range(0, 8).Select(_ => KarakteriKoda[nasumicno.Next(KarakteriKoda.Length)]).ToArray());
}
