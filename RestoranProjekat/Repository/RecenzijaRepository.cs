using Domain.Entiteti;
using Domain.Enumi;
using Microsoft.EntityFrameworkCore;
using Repository.Interfejsi;

namespace Repository;

public class RecenzijaRepository : BazniRepository<Recenzija>, IRecenzijaRepository
{
    public RecenzijaRepository(RestoranDbContext kontekst) : base(kontekst) { }

    public async Task<(List<Recenzija> Podaci, int Ukupno)> PretraziAsync(
        int? stavkaMenijaId, TipRecenzije? tipRecenzije, string? sortiranje, int strana, int velicinaStrane)
    {
        var upit = _dbSet
            .Include(r => r.Korisnik).Include(r => r.StavkaMenija)
            .Where(r => r.Aktivan);

        if (stavkaMenijaId.HasValue)
            upit = upit.Where(r => r.StavkaMenijaId == stavkaMenijaId.Value);

        if (tipRecenzije.HasValue)
            upit = upit.Where(r => r.TipRecenzije == tipRecenzije.Value);

        upit = sortiranje switch
        {
            "najbolje-ocenjeno" => upit.OrderByDescending(r => r.Ocena).ThenByDescending(r => r.DatumKreiranja),
            _ => upit.OrderByDescending(r => r.DatumKreiranja)
        };

        var ukupno = await upit.CountAsync();

        var podaci = await upit
            .Skip((strana - 1) * velicinaStrane)
            .Take(velicinaStrane)
            .ToListAsync();

        return (podaci, ukupno);
    }

    public async Task<List<Recenzija>> ListirajZaKorisnikaAsync(string korisnikId, int maks) =>
        await _dbSet
            .Include(r => r.Korisnik).Include(r => r.StavkaMenija)
            .Where(r => r.KorisnikId == korisnikId)
            .OrderByDescending(r => r.DatumKreiranja)
            .Take(maks)
            .ToListAsync();

    public async Task<Recenzija?> DobaviPostojecuAsync(string korisnikId, TipRecenzije tipRecenzije, int? stavkaMenijaId) =>
        await _dbSet.FirstOrDefaultAsync(r =>
            r.KorisnikId == korisnikId && r.TipRecenzije == tipRecenzije && r.StavkaMenijaId == stavkaMenijaId);

    public async Task<Dictionary<int, (double ProsecnaOcena, int BrojRecenzija)>> StatistikaZaStavkeAsync(IEnumerable<int> stavkaMenijaIds)
    {
        var ids = stavkaMenijaIds.ToList();
        if (ids.Count == 0) return new Dictionary<int, (double, int)>();

        var grupisano = await _dbSet
            .Where(r => r.Aktivan && r.StavkaMenijaId != null && ids.Contains(r.StavkaMenijaId.Value))
            .GroupBy(r => r.StavkaMenijaId!.Value)
            .Select(g => new { StavkaMenijaId = g.Key, Prosek = g.Average(r => (double)r.Ocena), Broj = g.Count() })
            .ToListAsync();

        return grupisano.ToDictionary(x => x.StavkaMenijaId, x => (x.Prosek, x.Broj));
    }
    public async Task<int> BrojBezOdgovoraAsync() =>
        await _dbSet.CountAsync(r => r.Aktivan && r.OdgovorRestorana == null);
}
