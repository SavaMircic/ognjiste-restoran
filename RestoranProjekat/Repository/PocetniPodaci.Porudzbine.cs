using Domain.Entiteti;
using Domain.Enumi;
using Microsoft.EntityFrameworkCore;

namespace Repository;

public static partial class PocetniPodaci
{
    private static async Task PopuniOtvorenePorudzbineAsync(RestoranDbContext kontekst)
    {
        if (await kontekst.Porudzbine.AnyAsync(p => p.Status == StatusPorudzbine.Otvorena)) return;

        var konobar = await ZaposleniPoMejluAsync(kontekst, MejlKonobar);
        var konobar2 = await ZaposleniPoMejluAsync(kontekst, MejlKonobar2);
        var kuvar = await ZaposleniPoMejluAsync(kontekst, MejlKuvar);
        var sanker = await ZaposleniPoMejluAsync(kontekst, MejlSanker);
        if (konobar == null || konobar2 == null || kuvar == null || sanker == null) return;

        var sto1 = await kontekst.Stolovi.FirstOrDefaultAsync(s => s.BrojStola == 1);
        var sto4 = await kontekst.Stolovi.FirstOrDefaultAsync(s => s.BrojStola == 4);
        if (sto1 == null || sto4 == null) return;

        var meni = await kontekst.StavkeMenija.ToDictionaryAsync(s => s.Naziv);
        var sada = DateTime.UtcNow;

        StavkaPorudzbine? Stavka(string naziv, int kolicina, StatusStavkePorudzbine status, int? pripremio, int minutaUnazad)
        {
            if (!meni.TryGetValue(naziv, out var jelo)) return null;

            var slanje = sada.AddMinutes(-minutaUnazad);
            return new StavkaPorudzbine
            {
                StavkaMenijaId = jelo.Id,
                Kolicina = kolicina,
                CenaUTrenutkuNarudzbine = jelo.CenaSaPopustom,
                Status = status,
                PripremioZaposleniId = status == StatusStavkePorudzbine.Poslato ? null : pripremio,
                VremeSlanja = slanje,
                VremePreuzimanja = status == StatusStavkePorudzbine.Poslato ? null : slanje.AddMinutes(2),
                VremeZavrsetka = status == StatusStavkePorudzbine.Spremno ? slanje.AddMinutes(9) : null
            };
        }

        var prva = new Porudzbina
        {
            StoId = sto1.Id,
            KonobarId = konobar.Id,
            VremeOtvaranja = sada.AddMinutes(-22),
            Status = StatusPorudzbine.Otvorena
        };
        foreach (var stavka in new[]
                 {
                     Stavka("Šopska salata", 2, StatusStavkePorudzbine.Spremno, kuvar.Id, 20),
                     Stavka("Karađorđeva šnicla", 1, StatusStavkePorudzbine.UPripremi, kuvar.Id, 15),
                     Stavka("Ćevapi (10 kom)", 2, StatusStavkePorudzbine.Poslato, null, 4),
                     Stavka("Coca Cola 0.33l", 3, StatusStavkePorudzbine.Spremno, sanker.Id, 19)
                 })
        {
            if (stavka != null) prva.Stavke.Add(stavka);
        }

        var druga = new Porudzbina
        {
            StoId = sto4.Id,
            KonobarId = konobar2.Id,
            VremeOtvaranja = sada.AddMinutes(-8),
            Status = StatusPorudzbine.Otvorena
        };
        foreach (var stavka in new[]
                 {
                     Stavka("Espresso", 2, StatusStavkePorudzbine.UPripremi, sanker.Id, 6),
                     Stavka("Domaća limunada", 2, StatusStavkePorudzbine.Poslato, null, 3),
                     Stavka("Baklava", 2, StatusStavkePorudzbine.Poslato, null, 3)
                 })
        {
            if (stavka != null) druga.Stavke.Add(stavka);
        }

        foreach (var sto in new[] { sto1, sto4 })
            sto.TrenutniStatus = StatusStola.Zauzet;

        kontekst.Porudzbine.AddRange(prva, druga);
        await kontekst.SaveChangesAsync();
    }

    private static async Task PopuniIstorijuPorudzbinaAsync(RestoranDbContext kontekst)
    {
        var granicaIstorije = DateTime.UtcNow.Date.AddDays(-2);
        if (await kontekst.Porudzbine.AnyAsync(p => p.VremeOtvaranja < granicaIstorije)) return;

        var racun = await UcitajRacunKontekstAsync(kontekst, seme: 20260817);
        if (racun == null) return;

        var danas = DateTime.UtcNow.Date;
        var porudzbine = new List<Porudzbina>();
        var stolovi = await kontekst.Stolovi.ToListAsync();
        if (stolovi.Count == 0) return;

        var realizovane = await kontekst.Rezervacije
            .Where(r => r.Status == StatusRezervacije.Realizovana && r.DatumVreme < danas)
            .ToListAsync();

        foreach (var rezervacija in realizovane)
        {
            var otvaranje = rezervacija.DatumVreme.AddMinutes(racun.Nasumicno.Next(2, 20));
            porudzbine.Add(NapraviRacun(racun, rezervacija.StoId, otvaranje, rezervacija.Id));
        }

        for (var pomak = BrojDanaIstorije; pomak >= 1; pomak--)
        {
            var dan = danas.AddDays(-pomak);
            var brojRacuna = BrojRacunaZaDan(racun.Nasumicno, dan);

            for (var i = 0; i < brojRacuna; i++)
            {
                var otvaranje = dan.AddHours(racun.Nasumicno.Next(11, 22)).AddMinutes(racun.Nasumicno.Next(0, 60));
                porudzbine.Add(NapraviRacun(racun, stolovi[racun.Nasumicno.Next(stolovi.Count)].Id, otvaranje, null));
            }
        }

        kontekst.Porudzbine.AddRange(porudzbine);
        await kontekst.SaveChangesAsync();
    }

    private sealed record RacunKontekst(
        Random Nasumicno,
        List<StavkaMenija> Meni,
        List<Zaposleni> Konobari,
        List<Zaposleni> Kuvari,
        List<Zaposleni> Sankeri);

    private static async Task<RacunKontekst?> UcitajRacunKontekstAsync(RestoranDbContext kontekst, int seme)
    {
        var zaposleni = await kontekst.Zaposleni.Include(z => z.Korisnik).ToListAsync();
        var konobari = zaposleni.Where(z => z.Korisnik.Email == MejlKonobar || z.Korisnik.Email == MejlKonobar2).ToList();
        var kuvari = zaposleni.Where(z => z.Korisnik.Email == MejlKuvar || z.Korisnik.Email == MejlKuvar2).ToList();
        var sankeri = zaposleni.Where(z => z.Korisnik.Email == MejlSanker || z.Korisnik.Email == MejlSanker2).ToList();
        if (konobari.Count == 0 || kuvari.Count == 0 || sankeri.Count == 0) return null;

        var meni = await kontekst.StavkeMenija.Include(s => s.Kategorija).ToListAsync();
        if (meni.Count == 0) return null;

        return new RacunKontekst(new Random(seme), meni, konobari, kuvari, sankeri);
    }

    private static int BrojRacunaZaDan(Random nasumicno, DateTime dan) =>
        dan.DayOfWeek is DayOfWeek.Friday or DayOfWeek.Saturday
            ? nasumicno.Next(5, 10)
            : nasumicno.Next(2, 6);

    private static Porudzbina NapraviRacun(RacunKontekst k, int stoId, DateTime otvaranje, int? rezervacijaId)
    {
        var porudzbina = new Porudzbina
        {
            StoId = stoId,
            KonobarId = k.Konobari[k.Nasumicno.Next(k.Konobari.Count)].Id,
            RezervacijaId = rezervacijaId,
            VremeOtvaranja = otvaranje,
            VremeZatvaranja = otvaranje.AddMinutes(k.Nasumicno.Next(35, 110)),
            Status = StatusPorudzbine.Zatvorena,
            NacinPlacanja = k.Nasumicno.Next(2) == 0 ? NacinPlacanja.Gotovina : NacinPlacanja.Kartica
        };

        decimal ukupno = 0;
        foreach (var jelo in k.Meni.OrderBy(_ => k.Nasumicno.Next()).Take(k.Nasumicno.Next(2, 6)))
        {
            var kolicina = k.Nasumicno.Next(1, 4);
            ukupno += jelo.CenaSaPopustom * kolicina;

            var slanje = otvaranje.AddMinutes(k.Nasumicno.Next(1, 15));
            var preuzimanje = slanje.AddMinutes(k.Nasumicno.Next(1, 6));

            var pripremaoci = jelo.Kategorija.Odrediste == Odrediste.Sank ? k.Sankeri : k.Kuvari;

            porudzbina.Stavke.Add(new StavkaPorudzbine
            {
                StavkaMenijaId = jelo.Id,
                Kolicina = kolicina,
                CenaUTrenutkuNarudzbine = jelo.CenaSaPopustom,
                Status = StatusStavkePorudzbine.Spremno,
                PripremioZaposleniId = pripremaoci[k.Nasumicno.Next(pripremaoci.Count)].Id,
                VremeSlanja = slanje,
                VremePreuzimanja = preuzimanje,
                VremeZavrsetka = preuzimanje.AddMinutes(k.Nasumicno.Next(3, 20))
            });
        }

        if (k.Nasumicno.Next(100) < 60)
            porudzbina.IznosNapojnice = Math.Round(ukupno * k.Nasumicno.Next(5, 13) / 100m, 2);

        return porudzbina;
    }
}
