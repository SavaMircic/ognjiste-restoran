using Domain.Enumi;
using Repository.Interfejsi;
using Repository.Rezultati;
using Services.DTO;

namespace Services.Implementation;

public class IzvestajServis : IIzvestajServis
{
    private readonly IIzvestajRepository _izvestajRepository;
    private readonly IZaposleniRepository _zaposleniRepository;

    public IzvestajServis(IIzvestajRepository izvestajRepository, IZaposleniRepository zaposleniRepository)
    {
        _izvestajRepository = izvestajRepository;
        _zaposleniRepository = zaposleniRepository;
    }

    public async Task<PrihodIzvestajDto> PrihodAsync(PrihodUpitDto upit)
    {
        var (od, doIsklj) = Opseg(upit);

        var zbir = await _izvestajRepository.ZbirPrihodaAsync(od, doIsklj);

        var stavke = upit.GrupisanjePo switch
        {
            GrupisanjePrihoda.Dan => (await _izvestajRepository.PrihodPoDanuAsync(od, doIsklj))
                .Select(r => new PrihodStavkaDto
                {
                    Grupa = DateOnly.FromDateTime(r.Dan).ToString("yyyy-MM-dd"),
                    Prihod = r.Prihod,
                    Kolicina = r.Kolicina
                }).ToList(),

            GrupisanjePrihoda.Nedelja => SazmiPoNedeljama(await _izvestajRepository.PrihodPoDanuAsync(od, doIsklj)),
            GrupisanjePrihoda.Mesec => SazmiPoMesecima(await _izvestajRepository.PrihodPoDanuAsync(od, doIsklj)),

            GrupisanjePrihoda.Kategorija => Mapiraj(await _izvestajRepository.PrihodPoKategorijiAsync(od, doIsklj)),
            GrupisanjePrihoda.Artikal => Mapiraj(await _izvestajRepository.PrihodPoArtikluAsync(od, doIsklj)),

            _ => new List<PrihodStavkaDto>()
        };

        foreach (var stavka in stavke)
            stavka.UdeoProcenat = zbir.UkupanPrihod == 0 ? 0 : Math.Round(stavka.Prihod / zbir.UkupanPrihod * 100, 2);

        return new PrihodIzvestajDto
        {
            DatumOd = upit.DatumOd,
            DatumDo = upit.DatumDo,
            GrupisanjePo = upit.GrupisanjePo,
            UkupanPrihod = zbir.UkupanPrihod,
            UkupnaNapojnica = zbir.UkupnaNapojnica,
            BrojPorudzbina = zbir.BrojPorudzbina,
            ProsecanRacun = zbir.BrojPorudzbina == 0 ? 0 : Math.Round(zbir.UkupanPrihod / zbir.BrojPorudzbina, 2),
            Stavke = stavke
        };
    }

    public async Task<List<UcinakZaposlenogDto>> UcinakZaposlenihAsync(UcinakUpitDto upit)
    {
        var (od, doIsklj) = Opseg(upit);

        var konobari = await _izvestajRepository.UcinakKonobaraAsync(od, doIsklj, upit.ZaposleniId);
        var priprema = await _izvestajRepository.UcinakPripremeAsync(od, doIsklj, upit.ZaposleniId);

        return Spoji(konobari, priprema)
            .OrderByDescending(z => z.VrednostStolova + z.VrednostPripremljenih)
            .ToList();
    }

    public async Task<List<TopJeloDto>> TopJelaAsync(TopJelaUpitDto upit)
    {
        var (od, doIsklj) = Opseg(upit);

        var prodaja = await _izvestajRepository.ProdajaPoArtikluAsync(od, doIsklj);

        var poredjani = upit.Redosled == RedosledTopJela.Najprodavanije
            ? prodaja.OrderByDescending(r => r.UkupnaKolicina).ThenByDescending(r => r.Prihod)
            : prodaja.OrderBy(r => r.UkupnaKolicina).ThenBy(r => r.Prihod);

        return poredjani
            .Select((r, indeks) => new TopJeloDto
            {
                Rang = indeks + 1,
                StavkaMenijaId = r.StavkaMenijaId,
                Naziv = r.Naziv,
                Kategorija = r.Kategorija,
                UkupnaKolicina = r.UkupnaKolicina,
                BrojPorudzbina = r.BrojPorudzbina,
                Prihod = r.Prihod
            })
            .ToList();
    }

    public async Task<(bool Uspesno, string? Greska, UcinakZaposlenogDto? Statistika)> MojaStatistikaAsync(
        string trenutniKorisnikId, MojaStatistikaUpitDto upit)
    {
        var zaposleni = await _zaposleniRepository.DobaviPoKorisnikIdAsync(trenutniKorisnikId);
        if (zaposleni == null) return (false, "Nalog nije povezan sa zaposlenim.", null);

        var (od, doIsklj) = Opseg(upit);

        var konobari = await _izvestajRepository.UcinakKonobaraAsync(od, doIsklj, zaposleni.Id);
        var priprema = await _izvestajRepository.UcinakPripremeAsync(od, doIsklj, zaposleni.Id);

        var red = Spoji(konobari, priprema).FirstOrDefault()
                  ?? new UcinakZaposlenogDto { ZaposleniId = zaposleni.Id };

        return (true, null, red);
    }

    public async Task<(bool Uspesno, string? Greska, List<IstorijaDanDto>? Dani)> MojaIstorijaAsync(
        string trenutniKorisnikId, IstorijaRadaUpitDto upit)
    {
        var zaposleni = await _zaposleniRepository.DobaviPoKorisnikIdAsync(trenutniKorisnikId);
        if (zaposleni == null) return (false, "Nalog nije povezan sa zaposlenim.", null);

        var (od, doIsklj) = Opseg(upit);

        var racuni = await _izvestajRepository.MojiZatvoreniRacuniAsync(od, doIsklj, zaposleni.Id);
        var pripreme = await _izvestajRepository.MojePripremljeneStavkeAsync(od, doIsklj, zaposleni.Id);

        var dani = new Dictionary<DateOnly, IstorijaDanDto>();

        IstorijaDanDto Dan(DateTime trenutak)
        {
            var datum = DateOnly.FromDateTime(trenutak);
            if (!dani.TryGetValue(datum, out var dan))
            {
                dan = new IstorijaDanDto { Datum = datum };
                dani[datum] = dan;
            }
            return dan;
        }

        foreach (var p in racuni)
        {
            var dan = Dan(p.VremeZatvaranja!.Value);

            var stavke = p.Stavke.Select(s => new IstorijaStavkaDto
            {
                Naziv = s.StavkaMenija.Naziv,
                Kolicina = s.Kolicina,
                Cena = s.CenaUTrenutkuNarudzbine,
                Ukupno = s.CenaUTrenutkuNarudzbine * s.Kolicina
            }).ToList();

            var ukupno = stavke.Sum(s => s.Ukupno);

            dan.Racuni.Add(new IstorijaRacunDto
            {
                PorudzbinaId = p.Id,
                BrojStola = p.Sto.BrojStola,
                VremeOtvaranja = p.VremeOtvaranja,
                VremeZatvaranja = p.VremeZatvaranja,
                NacinPlacanja = p.NacinPlacanja?.ToString(),
                Napojnica = p.IznosNapojnice ?? 0,
                Ukupno = ukupno,
                Stavke = stavke
            });

            dan.BrojRacuna++;
            dan.PrometRacuna += ukupno;
            dan.Napojnica += p.IznosNapojnice ?? 0;
        }

        foreach (var s in pripreme)
        {
            var dan = Dan(s.VremeZavrsetka!.Value);
            var vrednost = s.CenaUTrenutkuNarudzbine * s.Kolicina;

            dan.Pripreme.Add(new IstorijaPripremaDto
            {
                StavkaId = s.Id,
                Naziv = s.StavkaMenija.Naziv,
                Kolicina = s.Kolicina,
                BrojStola = s.Porudzbina.Sto.BrojStola,
                VremeZavrsetka = s.VremeZavrsetka.Value,
                Vrednost = vrednost
            });

            dan.BrojPripremljenihStavki++;
            dan.KolicinaPripremljena += s.Kolicina;
            dan.VrednostPripremljenog += vrednost;
        }

        return (true, null, dani.Values.OrderByDescending(d => d.Datum).ToList());
    }

    private static List<UcinakZaposlenogDto> Spoji(
        List<UcinakKonobaraRezultat> konobari, List<UcinakPripremeRezultat> priprema)
    {
        var poId = new Dictionary<int, UcinakZaposlenogDto>();

        UcinakZaposlenogDto Red(int id, string ime)
        {
            if (!poId.TryGetValue(id, out var red))
            {
                red = new UcinakZaposlenogDto { ZaposleniId = id, ImeZaposlenog = ime };
                poId[id] = red;
            }
            return red;
        }

        foreach (var k in konobari)
        {
            var red = Red(k.ZaposleniId, k.Ime);
            red.BrojStolova = k.BrojStolova;
            red.VrednostStolova = k.VrednostStolova;
            red.UkupnaNapojnica = k.Napojnica;
            red.ProsecanRacun = k.BrojStolova == 0 ? 0 : Math.Round(k.VrednostStolova / k.BrojStolova, 2);
        }

        foreach (var p in priprema)
        {
            var red = Red(p.ZaposleniId, p.Ime);
            red.BrojPripremljenihStavki = p.BrojStavki;
            red.UkupnaKolicinaPripremljena = p.UkupnaKolicina;
            red.VrednostPripremljenih = p.VrednostStavki;
        }

        return poId.Values.ToList();
    }

    private static (DateTime Od, DateTime DoIskljucivo) Opseg(IzvestajPeriodDto upit) =>
        (upit.DatumOd.ToDateTime(TimeOnly.MinValue), upit.DatumDo.AddDays(1).ToDateTime(TimeOnly.MinValue));

    private static List<PrihodStavkaDto> Mapiraj(List<PrihodPoGrupiRezultat> rezultati) =>
        rezultati.Select(r => new PrihodStavkaDto
        {
            Grupa = r.Grupa,
            Prihod = r.Prihod,
            Kolicina = r.Kolicina
        }).ToList();

    private static List<PrihodStavkaDto> SazmiPoNedeljama(List<PrihodPoDanuRezultat> poDanu) =>
        poDanu
            .GroupBy(r =>
            {
                var dan = DateOnly.FromDateTime(r.Dan);
                return dan.AddDays(-(((int)dan.DayOfWeek + 6) % 7));
            })
            .OrderBy(g => g.Key)
            .Select(g => new PrihodStavkaDto
            {
                Grupa = $"{g.Key:yyyy-MM-dd} – {g.Key.AddDays(6):yyyy-MM-dd}",
                Prihod = g.Sum(r => r.Prihod),
                Kolicina = g.Sum(r => r.Kolicina)
            })
            .ToList();

    private static List<PrihodStavkaDto> SazmiPoMesecima(List<PrihodPoDanuRezultat> poDanu) =>
        poDanu
            .GroupBy(r => new { r.Dan.Year, r.Dan.Month })
            .OrderBy(g => g.Key.Year).ThenBy(g => g.Key.Month)
            .Select(g => new PrihodStavkaDto
            {
                Grupa = $"{g.Key.Year:0000}-{g.Key.Month:00}",
                Prihod = g.Sum(r => r.Prihod),
                Kolicina = g.Sum(r => r.Kolicina)
            })
            .ToList();
}
