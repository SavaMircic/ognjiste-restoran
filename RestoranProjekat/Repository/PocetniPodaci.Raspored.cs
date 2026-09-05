using Domain.Entiteti;
using Microsoft.EntityFrameworkCore;

namespace Repository;

public static partial class PocetniPodaci
{
    private static async Task PopuniRasporedIBonuseAsync(RestoranDbContext kontekst)
    {
        var danas = DateOnly.FromDateTime(DateTime.Now);
        var ponedeljak = danas.AddDays(-(((int)danas.DayOfWeek + 6) % 7));
        var prosliPonedeljak = ponedeljak.AddDays(-7);
        var sledeciPonedeljak = ponedeljak.AddDays(7);

        var zaposleni = await kontekst.Zaposleni.Include(z => z.Korisnik).ToListAsync();

        Zaposleni? Po(string mejl) => zaposleni.FirstOrDefault(z => z.Korisnik.Email == mejl);

        var konobar = Po(MejlKonobar);
        var konobar2 = Po(MejlKonobar2);
        var sanker = Po(MejlSanker);
        var sanker2 = Po(MejlSanker2);
        var kuvar = Po(MejlKuvar);
        var kuvar2 = Po(MejlKuvar2);
        var menadzer = Po(MejlMenadzer);
        var admin = Po(MejlAdmin);

        if (konobar == null || konobar2 == null || sanker == null || sanker2 == null ||
            kuvar == null || kuvar2 == null || menadzer == null || admin == null) return;

        var prepodne = (Od: new TimeOnly(8, 0), Do: new TimeOnly(16, 0));
        var popodne = (Od: new TimeOnly(16, 0), Do: new TimeOnly(23, 0));
        var noc = (Od: new TimeOnly(22, 0), Do: new TimeOnly(2, 0));

        var odDatuma = prosliPonedeljak;
        var doDatuma = sledeciPonedeljak.AddDays(7);
        var popunjeniDani = (await kontekst.Smene
            .Where(s => s.Datum >= odDatuma && s.Datum < doDatuma)
            .Select(s => s.Datum)
            .Distinct()
            .ToListAsync()).ToHashSet();

        var smene = new List<Smena>();

        void Dodaj(Zaposleni z, DateOnly pocetakNedelje, int pomakDana, (TimeOnly Od, TimeOnly Do) termin) =>
            smene.Add(new Smena
            {
                ZaposleniId = z.Id,
                Datum = pocetakNedelje.AddDays(pomakDana),
                VremePocetka = termin.Od,
                VremeKraja = termin.Do
            });

        foreach (var (pocetak, danaUNedelji) in new[] { (prosliPonedeljak, 6), (ponedeljak, 6), (sledeciPonedeljak, 5) })
        {
            for (var dan = 0; dan <= danaUNedelji; dan++)
            {
                if (popunjeniDani.Contains(pocetak.AddDays(dan))) continue;

                var parni = dan % 2 == 0;

                Dodaj(parni ? konobar : konobar2, pocetak, dan, prepodne);
                Dodaj(parni ? konobar2 : konobar, pocetak, dan, popodne);
                Dodaj(parni ? kuvar : kuvar2, pocetak, dan, prepodne);
                Dodaj(parni ? kuvar2 : kuvar, pocetak, dan, popodne);
                Dodaj(parni ? sanker : sanker2, pocetak, dan, prepodne);

                if (dan is 4 or 5)
                    Dodaj(parni ? sanker2 : sanker, pocetak, dan, noc);
                else
                    Dodaj(parni ? sanker2 : sanker, pocetak, dan, popodne);

                if (dan <= 4) Dodaj(menadzer, pocetak, dan, prepodne);
                if (dan is 1 or 3) Dodaj(admin, pocetak, dan, prepodne);
            }
        }

        if (smene.Count > 0)
        {
            kontekst.Smene.AddRange(smene);
            await kontekst.SaveChangesAsync();
        }

        await PopuniBonuseAsync(kontekst, ponedeljak, sledeciPonedeljak, menadzer,
            new[] { konobar, konobar2, sanker, sanker2, kuvar, kuvar2 });
    }

    private static async Task PopuniBonuseAsync(
        RestoranDbContext kontekst, DateOnly ponedeljak, DateOnly sledeciPonedeljak,
        Zaposleni menadzer, Zaposleni[] osoblje)
    {
        if (await kontekst.Bonusi.AnyAsync()) return;

        var sada = DateTime.UtcNow;
        var bonusi = new List<Bonus>
        {
            new()
            {
                ZaposleniId = osoblje[0].Id, Iznos = 5000,
                DatumPocetka = ponedeljak.AddDays(-60), DatumKraja = ponedeljak.AddDays(-31),
                Razlog = "Najviše obrađenih stolova u prošlom mesecu",
                DodelioZaposleniId = menadzer.Id, DatumDodele = sada.AddDays(-62)
            },
            new()
            {
                ZaposleniId = osoblje[4].Id, Iznos = 4000,
                DatumPocetka = ponedeljak.AddDays(-30), DatumKraja = ponedeljak.AddDays(-1),
                Razlog = "Najbrža prosečna priprema u kuhinji",
                DodelioZaposleniId = menadzer.Id, DatumDodele = sada.AddDays(-31)
            },

            new()
            {
                ZaposleniId = osoblje[4].Id, Iznos = 7500,
                DatumPocetka = ponedeljak, DatumKraja = ponedeljak.AddDays(29),
                Razlog = "Bez ijedne reklamacije na pripremu tokom sezone",
                DodelioZaposleniId = menadzer.Id, DatumDodele = sada.AddDays(-2)
            },
            new()
            {
                ZaposleniId = osoblje[0].Id, Iznos = 6000,
                DatumPocetka = ponedeljak.AddDays(-3), DatumKraja = ponedeljak.AddDays(26),
                Razlog = "Najveći promet po stolu u prethodnom periodu",
                DodelioZaposleniId = menadzer.Id, DatumDodele = sada.AddDays(-4)
            },

            new()
            {
                ZaposleniId = osoblje[2].Id, Iznos = 3000,
                DatumPocetka = sledeciPonedeljak, DatumKraja = sledeciPonedeljak.AddDays(13),
                Razlog = "Preuzete dodatne noćne smene",
                DodelioZaposleniId = menadzer.Id, DatumDodele = sada.AddDays(-1)
            }
        };

        var trenutakDodele = sada.AddDays(-6);
        foreach (var radnik in osoblje)
        {
            bonusi.Add(new Bonus
            {
                ZaposleniId = radnik.Id,
                Iznos = 2500,
                DatumPocetka = ponedeljak.AddDays(-7),
                DatumKraja = ponedeljak.AddDays(21),
                Razlog = "Timski bonus za rekordan promet tokom gradske manifestacije",
                DodelioZaposleniId = menadzer.Id,
                DatumDodele = trenutakDodele
            });
        }

        kontekst.Bonusi.AddRange(bonusi);
        await kontekst.SaveChangesAsync();
    }
}
