using Domain.Entiteti;
using Microsoft.EntityFrameworkCore;
using Repository.Interfejsi;

namespace Repository;

public class KategorijaMenijaRepository : BazniRepository<KategorijaMenija>, IKategorijaMenijaRepository
{
    public KategorijaMenijaRepository(RestoranDbContext kontekst) : base(kontekst) { }

    public async Task<List<KategorijaMenija>> ListirajPoRedosleduAsync() =>
        await _dbSet.OrderBy(k => k.Redosled).ToListAsync();

    public async Task<bool> ImaStavkiAsync(int kategorijaId) =>
        await _kontekst.StavkeMenija.AnyAsync(s => s.KategorijaId == kategorijaId);
}
