using Domain.Entiteti;
using Domain.Enumi;
using Microsoft.EntityFrameworkCore;
using Repository.Interfejsi;
using Repository.Rezultati;

namespace Repository;

public class IzvestajRepository : IIzvestajRepository
{
    private readonly RestoranDbContext _kontekst;

    public IzvestajRepository(RestoranDbContext kontekst)
    {
        _kontekst = kontekst;
    }

    private IQueryable<StavkaPorudzbine> StavkeZatvorenihRacuna(DateTime od, DateTime doIsklj) =>
        _kontekst.StavkePorudzbine
            .Where(s => s.Porudzbina.Status == StatusPorudzbine.Zatvorena &&
                        s.Porudzbina.VremeZatvaranja >= od && s.Porudzbina.VremeZatvaranja < doIsklj);

    public async Task<List<PrihodPoDanuRezultat>> PrihodPoDanuAsync(DateTime od, DateTime doIsklj) =>
        await StavkeZatvorenihRacuna(od, doIsklj)
            .GroupBy(s => s.Porudzbina.VremeZatvaranja!.Value.Date)
            .Select(g => new PrihodPoDanuRezultat
            {
                Dan = g.Key,
                Prihod = g.Sum(s => s.CenaUTrenutkuNarudzbine * s.Kolicina),
                Kolicina = g.Sum(s => s.Kolicina)
            })
            .OrderBy(r => r.Dan)
            .ToListAsync();

    public async Task<List<PrihodPoGrupiRezultat>> PrihodPoKategorijiAsync(DateTime od, DateTime doIsklj) =>
        await StavkeZatvorenihRacuna(od, doIsklj)
            .GroupBy(s => s.StavkaMenija.Kategorija.Naziv)
            .Select(g => new PrihodPoGrupiRezultat
            {
                Grupa = g.Key,
                Prihod = g.Sum(s => s.CenaUTrenutkuNarudzbine * s.Kolicina),
                Kolicina = g.Sum(s => s.Kolicina)
            })
            .OrderByDescending(r => r.Prihod)
            .ToListAsync();

    public async Task<List<PrihodPoGrupiRezultat>> PrihodPoArtikluAsync(DateTime od, DateTime doIsklj) =>
        await StavkeZatvorenihRacuna(od, doIsklj)
            .GroupBy(s => s.StavkaMenija.Naziv)
            .Select(g => new PrihodPoGrupiRezultat
            {
                Grupa = g.Key,
                Prihod = g.Sum(s => s.CenaUTrenutkuNarudzbine * s.Kolicina),
                Kolicina = g.Sum(s => s.Kolicina)
            })
            .OrderByDescending(r => r.Prihod)
            .ToListAsync();

    public async Task<ZbirPrihodaRezultat> ZbirPrihodaAsync(DateTime od, DateTime doIsklj)
    {
        var porudzbine = _kontekst.Porudzbine
            .Where(p => p.Status == StatusPorudzbine.Zatvorena &&
                        p.VremeZatvaranja >= od && p.VremeZatvaranja < doIsklj);

        return new ZbirPrihodaRezultat
        {
            BrojPorudzbina = await porudzbine.CountAsync(),
            UkupnaNapojnica = await porudzbine.SumAsync(p => (decimal?)p.IznosNapojnice) ?? 0,
            UkupanPrihod = await StavkeZatvorenihRacuna(od, doIsklj)
                .SumAsync(s => (decimal?)(s.CenaUTrenutkuNarudzbine * s.Kolicina)) ?? 0
        };
    }

    public async Task<List<UcinakKonobaraRezultat>> UcinakKonobaraAsync(DateTime od, DateTime doIsklj, int? zaposleniId) =>
        await _kontekst.Porudzbine
            .Where(p => p.Status == StatusPorudzbine.Zatvorena &&
                        p.VremeZatvaranja >= od && p.VremeZatvaranja < doIsklj &&
                        (!zaposleniId.HasValue || p.KonobarId == zaposleniId.Value))
            .GroupBy(p => new { p.KonobarId, p.Konobar.Korisnik.Ime, p.Konobar.Korisnik.Prezime })
            .Select(g => new UcinakKonobaraRezultat
            {
                ZaposleniId = g.Key.KonobarId,
                Ime = g.Key.Ime + " " + g.Key.Prezime,
                BrojStolova = g.Count(),
                Napojnica = g.Sum(p => (decimal?)p.IznosNapojnice) ?? 0,
                VrednostStolova = g.Sum(p => p.Stavke.Sum(s => (decimal?)(s.CenaUTrenutkuNarudzbine * s.Kolicina)) ?? 0)
            })
            .ToListAsync();

    public async Task<List<UcinakPripremeRezultat>> UcinakPripremeAsync(DateTime od, DateTime doIsklj, int? zaposleniId) =>
        await _kontekst.StavkePorudzbine
            .Where(s => s.PripremioZaposleniId != null &&
                        s.VremeZavrsetka >= od && s.VremeZavrsetka < doIsklj &&
                        (!zaposleniId.HasValue || s.PripremioZaposleniId == zaposleniId.Value))
            .GroupBy(s => new
            {
                ZaposleniId = s.PripremioZaposleniId!.Value,
                s.PripremioZaposleni!.Korisnik.Ime,
                s.PripremioZaposleni!.Korisnik.Prezime
            })
            .Select(g => new UcinakPripremeRezultat
            {
                ZaposleniId = g.Key.ZaposleniId,
                Ime = g.Key.Ime + " " + g.Key.Prezime,
                BrojStavki = g.Count(),
                UkupnaKolicina = g.Sum(s => s.Kolicina),
                VrednostStavki = g.Sum(s => s.CenaUTrenutkuNarudzbine * s.Kolicina)
            })
            .ToListAsync();

    public async Task<List<Porudzbina>> MojiZatvoreniRacuniAsync(DateTime od, DateTime doIsklj, int zaposleniId) =>
        await _kontekst.Porudzbine
            .AsNoTracking()
            .Include(p => p.Sto)
            .Include(p => p.Stavke).ThenInclude(s => s.StavkaMenija)
            .Where(p => p.Status == StatusPorudzbine.Zatvorena &&
                        p.VremeZatvaranja >= od && p.VremeZatvaranja < doIsklj &&
                        p.KonobarId == zaposleniId)
            .OrderByDescending(p => p.VremeZatvaranja)
            .ToListAsync();

    public async Task<List<StavkaPorudzbine>> MojePripremljeneStavkeAsync(DateTime od, DateTime doIsklj, int zaposleniId) =>
        await _kontekst.StavkePorudzbine
            .AsNoTracking()
            .Include(s => s.StavkaMenija)
            .Include(s => s.Porudzbina).ThenInclude(p => p.Sto)
            .Where(s => s.PripremioZaposleniId == zaposleniId &&
                        s.VremeZavrsetka >= od && s.VremeZavrsetka < doIsklj)
            .OrderByDescending(s => s.VremeZavrsetka)
            .ToListAsync();

    public async Task<List<TopJeloRezultat>> ProdajaPoArtikluAsync(DateTime od, DateTime doIsklj) =>
        await _kontekst.StavkeMenija
            .Select(m => new TopJeloRezultat
            {
                StavkaMenijaId = m.Id,
                Naziv = m.Naziv,
                Kategorija = m.Kategorija.Naziv,
                UkupnaKolicina = _kontekst.StavkePorudzbine
                    .Where(s => s.StavkaMenijaId == m.Id &&
                                s.Porudzbina.Status == StatusPorudzbine.Zatvorena &&
                                s.Porudzbina.VremeZatvaranja >= od && s.Porudzbina.VremeZatvaranja < doIsklj)
                    .Sum(s => (int?)s.Kolicina) ?? 0,
                BrojPorudzbina = _kontekst.StavkePorudzbine
                    .Where(s => s.StavkaMenijaId == m.Id &&
                                s.Porudzbina.Status == StatusPorudzbine.Zatvorena &&
                                s.Porudzbina.VremeZatvaranja >= od && s.Porudzbina.VremeZatvaranja < doIsklj)
                    .Select(s => s.PorudzbinaId).Distinct().Count(),
                Prihod = _kontekst.StavkePorudzbine
                    .Where(s => s.StavkaMenijaId == m.Id &&
                                s.Porudzbina.Status == StatusPorudzbine.Zatvorena &&
                                s.Porudzbina.VremeZatvaranja >= od && s.Porudzbina.VremeZatvaranja < doIsklj)
                    .Sum(s => (decimal?)(s.CenaUTrenutkuNarudzbine * s.Kolicina)) ?? 0
            })
            .ToListAsync();
}
