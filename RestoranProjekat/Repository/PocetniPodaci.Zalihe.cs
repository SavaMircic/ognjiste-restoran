using Domain.Entiteti;
using Domain.Enumi;
using Microsoft.EntityFrameworkCore;

namespace Repository;

public static partial class PocetniPodaci
{
    private static async Task PopuniZaliheAsync(RestoranDbContext kontekst)
    {
        var namirnice = new[]
        {
            new Namirnica { Naziv = "Krompir", JedinicaMere = JedinicaMere.Kg, TrenutnaKolicina = 50, MinimalniPrag = 10 },
            new Namirnica { Naziv = "Piletina", JedinicaMere = JedinicaMere.Kg, TrenutnaKolicina = 30, MinimalniPrag = 5 },
            new Namirnica { Naziv = "Junetina", JedinicaMere = JedinicaMere.Kg, TrenutnaKolicina = 22, MinimalniPrag = 6 },
            new Namirnica { Naziv = "Svinjetina", JedinicaMere = JedinicaMere.Kg, TrenutnaKolicina = 25, MinimalniPrag = 6 },
            new Namirnica { Naziv = "Pastrmka", JedinicaMere = JedinicaMere.Kg, TrenutnaKolicina = 5, MinimalniPrag = 2 },
            new Namirnica { Naziv = "Mleko", JedinicaMere = JedinicaMere.L, TrenutnaKolicina = 20, MinimalniPrag = 5 },
            new Namirnica { Naziv = "Jaja", JedinicaMere = JedinicaMere.Kom, TrenutnaKolicina = 100, MinimalniPrag = 20 },
            new Namirnica { Naziv = "Paradajz", JedinicaMere = JedinicaMere.Kg, TrenutnaKolicina = 8, MinimalniPrag = 3 },
            new Namirnica { Naziv = "Krastavac", JedinicaMere = JedinicaMere.Kg, TrenutnaKolicina = 6, MinimalniPrag = 2 },
            new Namirnica { Naziv = "Crni luk", JedinicaMere = JedinicaMere.Kg, TrenutnaKolicina = 12, MinimalniPrag = 4 },
            new Namirnica { Naziv = "Paprika", JedinicaMere = JedinicaMere.Kg, TrenutnaKolicina = 7, MinimalniPrag = 2 },
            new Namirnica { Naziv = "Feta sir", JedinicaMere = JedinicaMere.Kg, TrenutnaKolicina = 4, MinimalniPrag = 2 },
            new Namirnica { Naziv = "Pirinač", JedinicaMere = JedinicaMere.Kg, TrenutnaKolicina = 9, MinimalniPrag = 3 },
            new Namirnica { Naziv = "Šećer", JedinicaMere = JedinicaMere.Kg, TrenutnaKolicina = 14, MinimalniPrag = 4 },
            new Namirnica { Naziv = "Limun", JedinicaMere = JedinicaMere.Kom, TrenutnaKolicina = 40, MinimalniPrag = 15 },
            new Namirnica { Naziv = "Kafa", JedinicaMere = JedinicaMere.Kg, TrenutnaKolicina = 6, MinimalniPrag = 2 },
            new Namirnica { Naziv = "Coca Cola limenka", JedinicaMere = JedinicaMere.Kom, TrenutnaKolicina = 120, MinimalniPrag = 24 },
            new Namirnica { Naziv = "Kiseli kupus", JedinicaMere = JedinicaMere.Kg, TrenutnaKolicina = 18, MinimalniPrag = 5 },
            new Namirnica { Naziv = "Suva rebra", JedinicaMere = JedinicaMere.Kg, TrenutnaKolicina = 6, MinimalniPrag = 2 },

            new Namirnica { Naziv = "Brašno", JedinicaMere = JedinicaMere.Kg, TrenutnaKolicina = 2, MinimalniPrag = 10 },
            new Namirnica { Naziv = "Ulje", JedinicaMere = JedinicaMere.L, TrenutnaKolicina = 1.5m, MinimalniPrag = 5 },
            new Namirnica { Naziv = "Kajmak", JedinicaMere = JedinicaMere.Kg, TrenutnaKolicina = 0.8m, MinimalniPrag = 2 },
            new Namirnica { Naziv = "Orasi", JedinicaMere = JedinicaMere.Kg, TrenutnaKolicina = 0.5m, MinimalniPrag = 3 }
        };

        foreach (var namirnica in namirnice)
        {
            if (!await kontekst.Namirnice.AnyAsync(n => n.Naziv == namirnica.Naziv))
                kontekst.Namirnice.Add(namirnica);
        }
        await kontekst.SaveChangesAsync();

        await PopuniReceptureAsync(kontekst);
        await PopuniKorekcijeAsync(kontekst);
    }

    private static async Task PopuniReceptureAsync(RestoranDbContext kontekst)
    {
        var sastavi = new (string Jelo, (string Namirnica, decimal Kolicina)[] Sastojci)[]
        {
            ("Šopska salata", new[] { ("Paradajz", 0.15m), ("Krastavac", 0.10m), ("Feta sir", 0.08m), ("Crni luk", 0.03m) }),
            ("Srpska salata", new[] { ("Paradajz", 0.20m), ("Crni luk", 0.05m), ("Paprika", 0.08m) }),
            ("Kajmak i proja", new[] { ("Kajmak", 0.10m), ("Brašno", 0.15m), ("Jaja", 1m) }),

            ("Pileća supa", new[] { ("Piletina", 0.30m), ("Krompir", 0.20m), ("Crni luk", 0.05m) }),
            ("Riblja čorba", new[] { ("Pastrmka", 0.25m), ("Paradajz", 0.10m), ("Paprika", 0.06m), ("Crni luk", 0.05m) }),

            ("Karađorđeva šnicla", new[] { ("Svinjetina", 0.25m), ("Kajmak", 0.06m), ("Jaja", 1m), ("Brašno", 0.05m) }),
            ("Punjena piletina", new[] { ("Piletina", 0.28m), ("Paprika", 0.10m), ("Ulje", 0.03m) }),
            ("Pastrmka na žaru", new[] { ("Pastrmka", 0.35m), ("Krompir", 0.20m), ("Ulje", 0.02m) }),
            ("Đuveč", new[] { ("Pirinač", 0.15m), ("Paradajz", 0.12m), ("Paprika", 0.10m), ("Ulje", 0.04m) }),
            ("Sarma", new[] { ("Kiseli kupus", 0.30m), ("Junetina", 0.12m), ("Svinjetina", 0.10m), ("Pirinač", 0.05m), ("Suva rebra", 0.08m), ("Crni luk", 0.04m) }),

            ("Ćevapi (10 kom)", new[] { ("Junetina", 0.22m), ("Svinjetina", 0.08m), ("Crni luk", 0.04m), ("Brašno", 0.10m) }),
            ("Pljeskavica", new[] { ("Junetina", 0.20m), ("Svinjetina", 0.10m), ("Crni luk", 0.03m) }),
            ("Mešano meso", new[] { ("Junetina", 0.25m), ("Svinjetina", 0.25m), ("Crni luk", 0.06m) }),

            ("Palačinke sa Nutellom", new[] { ("Brašno", 0.08m), ("Mleko", 0.15m), ("Jaja", 2m), ("Ulje", 0.02m) }),
            ("Baklava", new[] { ("Brašno", 0.10m), ("Orasi", 0.08m), ("Šećer", 0.12m) }),
            ("Krempita", new[] { ("Mleko", 0.20m), ("Jaja", 2m), ("Šećer", 0.10m), ("Brašno", 0.05m) }),

            ("Coca Cola 0.33l", new[] { ("Coca Cola limenka", 1m) }),
            ("Espresso", new[] { ("Kafa", 0.008m) }),
            ("Domaća limunada", new[] { ("Limun", 2m), ("Šećer", 0.03m) })
        };

        var jelaPoNazivu = await kontekst.StavkeMenija.ToDictionaryAsync(s => s.Naziv, s => s.Id);
        var namirnicePoNazivu = await kontekst.Namirnice.ToDictionaryAsync(n => n.Naziv, n => n.Id);

        foreach (var sastav in sastavi)
        {
            if (!jelaPoNazivu.TryGetValue(sastav.Jelo, out var jeloId)) continue;
            if (await kontekst.Recepture.AnyAsync(r => r.StavkaMenijaId == jeloId)) continue;

            foreach (var (naziv, kolicina) in sastav.Sastojci)
            {
                if (!namirnicePoNazivu.TryGetValue(naziv, out var namirnicaId)) continue;

                kontekst.Recepture.Add(new Receptura
                {
                    StavkaMenijaId = jeloId,
                    NamirnicaId = namirnicaId,
                    Kolicina = kolicina
                });
            }
        }

        await kontekst.SaveChangesAsync();
    }

    private static async Task PopuniKorekcijeAsync(RestoranDbContext kontekst)
    {
        if (await kontekst.KorekcijeZaliha.AnyAsync()) return;

        var menadzer = await ZaposleniPoMejluAsync(kontekst, MejlMenadzer);
        var kuvar = await ZaposleniPoMejluAsync(kontekst, MejlKuvar);
        if (menadzer == null || kuvar == null) return;

        var namirnice = await kontekst.Namirnice.ToDictionaryAsync(n => n.Naziv, n => n.Id);

        var korekcije = new (string Namirnica, decimal Stara, decimal Nova, string Razlog, int Zaposleni, int DanaUnazad)[]
        {
            ("Krompir", 42, 50, "Prijem robe od dobavljača", menadzer.Id, 12),
            ("Junetina", 30, 22, "Popis — utvrđen manjak", menadzer.Id, 9),
            ("Brašno", 5, 2, "Popis — utvrđen manjak", menadzer.Id, 5),
            ("Paradajz", 14, 8, "Kalo — deo robe neupotrebljiv", kuvar.Id, 3),
            ("Kajmak", 3.5m, 0.8m, "Veliki vikend, potrošeno više od plana", kuvar.Id, 2),
            ("Orasi", 3, 0.5m, "Utrošeno na pripremu baklave za proslavu", kuvar.Id, 1)
        };

        foreach (var korekcija in korekcije)
        {
            if (!namirnice.TryGetValue(korekcija.Namirnica, out var namirnicaId)) continue;

            kontekst.KorekcijeZaliha.Add(new KorekcijaZaliha
            {
                NamirnicaId = namirnicaId,
                StaraKolicina = korekcija.Stara,
                NovaKolicina = korekcija.Nova,
                Razlog = korekcija.Razlog,
                ZaposleniId = korekcija.Zaposleni,
                Datum = DateTime.UtcNow.AddDays(-korekcija.DanaUnazad)
            });
        }

        await kontekst.SaveChangesAsync();
    }
}
