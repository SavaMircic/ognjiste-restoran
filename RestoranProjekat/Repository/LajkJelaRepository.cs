using Domain.Entiteti;
using Microsoft.EntityFrameworkCore;
using Repository.Interfejsi;

namespace Repository;

public class LajkJelaRepository : BazniRepository<LajkJela>, ILajkJelaRepository
{
    public LajkJelaRepository(RestoranDbContext kontekst) : base(kontekst) { }

    public async Task<LajkJela?> DobaviAsync(string korisnikId, int stavkaMenijaId) =>
        await _dbSet.FirstOrDefaultAsync(l => l.KorisnikId == korisnikId && l.StavkaMenijaId == stavkaMenijaId);

    public async Task<List<StavkaMenija>> ListirajOmiljenaJelaAsync(string korisnikId) =>
        await _dbSet
            .Where(l => l.KorisnikId == korisnikId)
            .OrderByDescending(l => l.DatumKreiranja)
            .Include(l => l.StavkaMenija).ThenInclude(s => s.Kategorija)
            .Select(l => l.StavkaMenija)
            .ToListAsync();

    public async Task<Dictionary<int, int>> BrojLajkovaZaStavkeAsync(IEnumerable<int> stavkaMenijaIds)
    {
        var ids = stavkaMenijaIds.ToList();
        if (ids.Count == 0) return new Dictionary<int, int>();

        var grupisano = await _dbSet
            .Where(l => ids.Contains(l.StavkaMenijaId))
            .GroupBy(l => l.StavkaMenijaId)
            .Select(g => new { StavkaMenijaId = g.Key, Broj = g.Count() })
            .ToListAsync();

        return grupisano.ToDictionary(x => x.StavkaMenijaId, x => x.Broj);
    }
}
