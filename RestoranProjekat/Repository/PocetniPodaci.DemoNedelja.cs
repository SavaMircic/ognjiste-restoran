using Domain.Entiteti;
using Domain.Enumi;
using Domain.Konstante;
using Microsoft.EntityFrameworkCore;

namespace Repository;

public static partial class PocetniPodaci
{
    private static readonly DateOnly DanOdbrane = new(2026, 9, 8);

    private static async Task OsveziDemoPodatkeAsync(RestoranDbContext kontekst)
    {
        await DopuniRacuneDoDanasAsync(kontekst);
        await DopuniRezervacijeNedeljeAsync(kontekst);
        await OsveziIstekleKazneAsync(kontekst);

        await UskladiStatuseStolovaAsync(kontekst);
    }

    private static DateTime UtcOdLokalnog(DateOnly dan, int sat, int minut = 0) =>
        DateTime.SpecifyKind(dan.ToDateTime(new TimeOnly(sat, minut)), DateTimeKind.Local).ToUniversalTime();

    private static async Task DopuniRacuneDoDanasAsync(RestoranDbContext kontekst)
    {
        var danas = DateTime.UtcNow.Date;
        var od = danas.AddDays(-BrojDanaIstorije);

        var daniSaRacunom = await kontekst.Porudzbine
            .Where(p => p.Status == StatusPorudzbine.Zatvorena && p.VremeOtvaranja >= od)
            .Select(p => p.VremeOtvaranja.Date)
            .Distinct()
            .ToListAsync();

        var popunjeni = daniSaRacunom.ToHashSet();

        var racun = await UcitajRacunKontekstAsync(kontekst, seme: 20260908);
        if (racun == null) return;

        var stolovi = await kontekst.Stolovi.ToListAsync();
        if (stolovi.Count == 0) return;

        var sadaLokalno = DateTime.Now;
        var porudzbine = new List<Porudzbina>();

        for (var pomak = BrojDanaIstorije; pomak >= 0; pomak--)
        {
            var dan = danas.AddDays(-pomak);
            if (popunjeni.Contains(dan)) continue;

            var poslednjiSat = pomak == 0 ? sadaLokalno.Hour - 1 : 21;

            if (poslednjiSat < 12) continue;

            var danLokalno = DateOnly.FromDateTime(dan);
            var brojRacuna = BrojRacunaZaDan(racun.Nasumicno, dan);

            for (var i = 0; i < brojRacuna; i++)
            {
                var otvaranje = UtcOdLokalnog(danLokalno, racun.Nasumicno.Next(11, poslednjiSat + 1))
                    .AddMinutes(racun.Nasumicno.Next(0, 60));

                porudzbine.Add(NapraviRacun(
                    racun, stolovi[racun.Nasumicno.Next(stolovi.Count)].Id, otvaranje, rezervacijaId: null));
            }
        }

        if (porudzbine.Count == 0) return;

        kontekst.Porudzbine.AddRange(porudzbine);
        await kontekst.SaveChangesAsync();
    }

    private static async Task ZatvoriZaostaleRacuneAsync(RestoranDbContext kontekst)
    {
        var pocetakDana = DateTime.UtcNow.Date;

        var zaostali = await kontekst.Porudzbine
            .Include(p => p.Stavke)
            .Where(p => p.Status == StatusPorudzbine.Otvorena && p.VremeOtvaranja < pocetakDana)
            .ToListAsync();

        if (zaostali.Count == 0) return;

        var nasumicno = new Random(20260908);

        foreach (var porudzbina in zaostali)
        {
            foreach (var stavka in porudzbina.Stavke.Where(s => s.Status != StatusStavkePorudzbine.Spremno))
            {
                stavka.Status = StatusStavkePorudzbine.Spremno;
                stavka.VremePreuzimanja ??= stavka.VremeSlanja.AddMinutes(3);
                stavka.VremeZavrsetka ??= stavka.VremePreuzimanja.Value.AddMinutes(nasumicno.Next(5, 20));
            }

            porudzbina.Status = StatusPorudzbine.Zatvorena;
            porudzbina.VremeZatvaranja = porudzbina.VremeOtvaranja.AddMinutes(nasumicno.Next(45, 100));
            porudzbina.NacinPlacanja = nasumicno.Next(2) == 0 ? NacinPlacanja.Gotovina : NacinPlacanja.Kartica;
        }

        var oslobodjeni = zaostali.Select(p => p.StoId).Distinct().ToList();
        var josOtvoreni = await kontekst.Porudzbine
            .Where(p => p.Status == StatusPorudzbine.Otvorena && p.VremeOtvaranja >= pocetakDana)
            .Select(p => p.StoId)
            .ToListAsync();

        var stolovi = await kontekst.Stolovi
            .Where(s => oslobodjeni.Contains(s.Id) && !josOtvoreni.Contains(s.Id))
            .ToListAsync();

        foreach (var sto in stolovi.Where(s => s.TrenutniStatus == StatusStola.Zauzet))
            sto.TrenutniStatus = StatusStola.Slobodan;

        await kontekst.SaveChangesAsync();
    }

    private readonly record struct PlanRezervacije(
        int PomakDana,
        int Sat,
        int Minut,
        int BrojGostiju,
        int[] Stolovi,
        string? Mejl,
        string? GostIme,
        string? GostMejl,
        string? GostTelefon,
        NacinKreiranjaRezervacije Nacin,
        StatusRezervacije Status = StatusRezervacije.Aktivna);

    private static async Task DopuniRezervacijeNedeljeAsync(RestoranDbContext kontekst)
    {
        var danas = DateOnly.FromDateTime(DateTime.Now);

        var ponedeljak = danas.AddDays(-(((int)danas.DayOfWeek + 6) % 7));
        var krajProzora = ponedeljak.AddDays(13);

        var pocetakSedmice = DanOdbrane >= danas && DanOdbrane <= krajProzora
            ? DanOdbrane.AddDays(-1)
            : danas;

        var gosti = await kontekst.Users
            .Where(k => k.Email!.EndsWith("@test.com") && k.Aktivan)
            .ToDictionaryAsync(k => k.Email!, k => k.Id);

        var stolovi = await kontekst.Stolovi.ToDictionaryAsync(s => s.BrojStola, s => s);
        if (gosti.Count == 0 || stolovi.Count == 0) return;

        var admin = await ZaposleniPoMejluAsync(kontekst, MejlAdmin);
        var nasumicno = new Random(20260908);

        var kodovi = (await kontekst.Rezervacije.Select(r => r.KodRezervacije).ToListAsync()).ToHashSet();

        var plan = new[]
        {
            new PlanRezervacije(0, 19, 0, 2, [2], "petar@test.com", null, null, null, NacinKreiranjaRezervacije.Samostalno),
            new PlanRezervacije(0, 20, 0, 6, [6], null, "Nenad Ivanović", "nenad.ivanovic@example.com", "064 777 8889", NacinKreiranjaRezervacije.TelefonAdmin),

            new PlanRezervacije(1, 12, 30, 2, [1], "petar@test.com", null, null, null, NacinKreiranjaRezervacije.Samostalno),
            new PlanRezervacije(1, 13, 0, 4, [3], null, "Jovan Mitrović", "jovan.mitrovic@example.com", "065 111 2223", NacinKreiranjaRezervacije.TelefonAdmin),
            new PlanRezervacije(1, 13, 30, 4, [5], "ana@test.com", null, null, null, NacinKreiranjaRezervacije.Samostalno),
            new PlanRezervacije(1, 18, 0, 6, [7], "marko@test.com", null, null, null, NacinKreiranjaRezervacije.Samostalno),
            new PlanRezervacije(1, 19, 0, 13, [8, 13], "milica@test.com", null, null, null, NacinKreiranjaRezervacije.Samostalno),
            new PlanRezervacije(1, 20, 0, 2, [9], null, "Sara Kostić", "sara.kostic@example.com", "064 444 5556", NacinKreiranjaRezervacije.TelefonAdmin),
            new PlanRezervacije(1, 20, 30, 4, [4], "jelena@test.com", null, null, null, NacinKreiranjaRezervacije.Samostalno, StatusRezervacije.Otkazana),

            new PlanRezervacije(2, 13, 0, 2, [9], "ana@test.com", null, null, null, NacinKreiranjaRezervacije.Samostalno),
            new PlanRezervacije(2, 19, 30, 4, [5], "marko@test.com", null, null, null, NacinKreiranjaRezervacije.Samostalno),
            new PlanRezervacije(2, 20, 0, 4, [11], "stefan@test.com", null, null, null, NacinKreiranjaRezervacije.KontaktForma),

            new PlanRezervacije(3, 20, 0, 6, [7], "jelena@test.com", null, null, null, NacinKreiranjaRezervacije.Samostalno),
            new PlanRezervacije(3, 20, 30, 16, [12, 8], null, "Dragan Blagojević", "dragan.blagojevic@example.com", "069 888 9990", NacinKreiranjaRezervacije.TelefonAdmin),

            new PlanRezervacije(4, 19, 0, 4, [3], "stefan@test.com", null, null, null, NacinKreiranjaRezervacije.Samostalno),
            new PlanRezervacije(4, 20, 0, 2, [1], "petar@test.com", null, null, null, NacinKreiranjaRezervacije.Samostalno),
            new PlanRezervacije(4, 21, 0, 8, [8], null, "Sonja Rakić", "sonja.rakic@example.com", "064 222 3334", NacinKreiranjaRezervacije.KontaktForma),

            new PlanRezervacije(5, 13, 0, 6, [13], "ana@test.com", null, null, null, NacinKreiranjaRezervacije.Samostalno),
            new PlanRezervacije(5, 19, 0, 20, [12, 8, 7], "marko@test.com", null, null, null, NacinKreiranjaRezervacije.Samostalno),
            new PlanRezervacije(5, 20, 30, 4, [10], "milica@test.com", null, null, null, NacinKreiranjaRezervacije.Samostalno),

            new PlanRezervacije(6, 13, 30, 4, [5], "jelena@test.com", null, null, null, NacinKreiranjaRezervacije.Samostalno),
            new PlanRezervacije(6, 14, 0, 2, [2], null, "Ivana Krstić", "ivana.krstic@example.com", "065 333 4445", NacinKreiranjaRezervacije.TelefonAdmin)
        };

        var sada = DateTime.UtcNow;
        var nove = new List<Rezervacija>();

        foreach (var stavka in plan)
        {
            var dan = pocetakSedmice.AddDays(stavka.PomakDana);
            var termin = UtcOdLokalnog(dan, stavka.Sat, stavka.Minut);

            if (termin <= sada) continue;

            string? korisnikId = null;
            if (stavka.Mejl != null && !gosti.TryGetValue(stavka.Mejl, out korisnikId)) continue;

            var trazeni = stavka.Stolovi
                .Select(broj => stolovi.GetValueOrDefault(broj))
                .Where(s => s != null)
                .Select(s => s!)
                .ToList();

            if (trazeni.Count != stavka.Stolovi.Length) continue;

            var slobodno = true;
            foreach (var sto in trazeni)
            {
                if (await VecZasejanoAsync(kontekst, sto.Id, termin) ||
                    await PostojiRezervacijaUTerminuAsync(kontekst, sto.Id, termin) ||
                    nove.Any(r => r.StoId == sto.Id && Preklapa(r.DatumVreme, termin)))
                {
                    slobodno = false;
                    break;
                }
            }

            if (!slobodno) continue;

            var kod = NovKod(nasumicno, kodovi);
            var kreirano = sada.AddDays(-nasumicno.Next(1, 6));

            foreach (var sto in trazeni)
            {
                nove.Add(new Rezervacija
                {
                    KorisnikId = korisnikId,
                    GostIme = stavka.GostIme,
                    GostEmail = stavka.GostMejl,
                    GostTelefon = stavka.GostTelefon,
                    StoId = sto.Id,
                    DatumVreme = termin,
                    BrojGostiju = stavka.BrojGostiju,
                    KodRezervacije = kod,
                    Status = stavka.Status,
                    NacinKreiranja = stavka.Nacin,
                    KreiraoZaposleniId = stavka.Nacin == NacinKreiranjaRezervacije.Samostalno ? null : admin?.Id,
                    DatumKreiranja = kreirano
                });
            }
        }

        await DodajRezervacijuZaNepunSatAsync(kontekst, gosti, stolovi, nove, kodovi, nasumicno);

        if (nove.Count == 0) return;

        kontekst.Rezervacije.AddRange(nove);
        await kontekst.SaveChangesAsync();
    }

    private static async Task DodajRezervacijuZaNepunSatAsync(
        RestoranDbContext kontekst, Dictionary<string, string> gosti, Dictionary<int, Sto> stolovi,
        List<Rezervacija> nove, HashSet<string> kodovi, Random nasumicno)
    {
        var sada = DateTime.UtcNow;
        var granica = sada.AddHours(1);

        var vecPostoji = await kontekst.Rezervacije.AnyAsync(r =>
            r.Status == StatusRezervacije.Aktivna && r.DatumVreme > sada && r.DatumVreme <= granica);

        if (vecPostoji || nove.Any(r => r.DatumVreme <= granica)) return;
        if (!gosti.TryGetValue("milica@test.com", out var korisnikId)) return;

        var termin = sada.AddMinutes(50);

        Sto? sto = null;
        foreach (var kandidat in stolovi.Values
                     .Where(s => s.Kapacitet >= 2 && s.TrenutniStatus == StatusStola.Slobodan)
                     .OrderBy(s => s.Kapacitet))
        {
            if (nove.Any(r => r.StoId == kandidat.Id && Preklapa(r.DatumVreme, termin))) continue;
            if (await PostojiRezervacijaUTerminuAsync(kontekst, kandidat.Id, termin)) continue;

            sto = kandidat;
            break;
        }

        if (sto == null) return;

        nove.Add(new Rezervacija
        {
            KorisnikId = korisnikId,
            StoId = sto.Id,
            DatumVreme = termin,
            BrojGostiju = 2,
            KodRezervacije = NovKod(nasumicno, kodovi),
            Status = StatusRezervacije.Aktivna,
            NacinKreiranja = NacinKreiranjaRezervacije.Samostalno,
            DatumKreiranja = sada.AddDays(-1)
        });

        if (sto.TrenutniStatus == StatusStola.Slobodan)
            sto.TrenutniStatus = StatusStola.Rezervisan;
    }

    private static Task<bool> VecZasejanoAsync(RestoranDbContext kontekst, int stoId, DateTime termin) =>
        kontekst.Rezervacije.AnyAsync(r => r.StoId == stoId && r.DatumVreme == termin);

    private static bool Preklapa(DateTime a, DateTime b) =>
        Math.Abs((a - b).TotalHours) < PoslovnaPravila.TrajanjeRezervacijeSati;

    private static async Task<bool> PostojiRezervacijaUTerminuAsync(
        RestoranDbContext kontekst, int stoId, DateTime termin)
    {
        var pocetak = termin.AddHours(-PoslovnaPravila.TrajanjeRezervacijeSati);
        var kraj = termin.AddHours(PoslovnaPravila.TrajanjeRezervacijeSati);

        return await kontekst.Rezervacije.AnyAsync(r =>
            r.StoId == stoId &&
            (r.Status == StatusRezervacije.Aktivna || r.Status == StatusRezervacije.Realizovana) &&
            r.DatumVreme > pocetak && r.DatumVreme < kraj);
    }

    private static string NovKod(Random nasumicno, HashSet<string> zauzeti)
    {
        string kod;
        do { kod = Kod(nasumicno); } while (!zauzeti.Add(kod));
        return kod;
    }

    private static async Task UskladiStatuseStolovaAsync(RestoranDbContext kontekst)
    {
        var sada = DateTime.UtcNow;
        var granica = sada.AddHours(1);

        var zauzeti = (await kontekst.Porudzbine
            .Where(p => p.Status == StatusPorudzbine.Otvorena)
            .Select(p => p.StoId)
            .ToListAsync()).ToHashSet();

        var rezervisani = (await kontekst.Rezervacije
            .Where(r => r.Status == StatusRezervacije.Aktivna && r.DatumVreme > sada && r.DatumVreme <= granica)
            .Select(r => r.StoId)
            .ToListAsync()).ToHashSet();

        var stolovi = await kontekst.Stolovi.ToListAsync();
        var promenjeno = false;

        foreach (var sto in stolovi)
        {
            var zeljeni = zauzeti.Contains(sto.Id) ? StatusStola.Zauzet
                : rezervisani.Contains(sto.Id) ? StatusStola.Rezervisan
                : StatusStola.Slobodan;

            if (sto.TrenutniStatus == zeljeni) continue;

            sto.TrenutniStatus = zeljeni;
            promenjeno = true;
        }

        if (promenjeno) await kontekst.SaveChangesAsync();
    }

    private static async Task OsveziIstekleKazneAsync(RestoranDbContext kontekst)
    {
        var sada = DateTime.UtcNow;

        var istekle = await kontekst.Users
            .Where(k => (k.BlokiranDo != null && k.BlokiranDo < sada) ||
                        (k.ZabranaKomentarisanjaDo != null && k.ZabranaKomentarisanjaDo < sada))
            .ToListAsync();

        if (istekle.Count == 0) return;

        foreach (var korisnik in istekle)
        {
            if (korisnik.BlokiranDo is not null && korisnik.BlokiranDo < sada)
                korisnik.BlokiranDo = sada.AddDays(9);

            if (korisnik.ZabranaKomentarisanjaDo is not null && korisnik.ZabranaKomentarisanjaDo < sada)
                korisnik.ZabranaKomentarisanjaDo = sada.AddDays(20);
        }

        await kontekst.SaveChangesAsync();
    }
}
