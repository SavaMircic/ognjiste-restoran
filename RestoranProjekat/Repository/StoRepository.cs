using Domain.Entiteti;
using Microsoft.EntityFrameworkCore;
using Repository.Interfejsi;

namespace Repository;

public class StoRepository : BazniRepository<Sto>, IStoRepository
{
    public StoRepository(RestoranDbContext kontekst) : base(kontekst) { }

    public async Task<List<Sto>> ListirajAsync() =>
        await _dbSet.OrderBy(s => s.BrojStola).ToListAsync();

    public async Task<bool> PostojiBrojStolaAsync(int brojStola, int? iskljuciId = null) =>
        await _dbSet.AnyAsync(s => s.BrojStola == brojStola && (!iskljuciId.HasValue || s.Id != iskljuciId.Value));
}
