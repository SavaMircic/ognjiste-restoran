using Domain.Entiteti;
using Microsoft.EntityFrameworkCore;
using Repository.Interfejsi;

namespace Repository;

public class GalerijaRepository : BazniRepository<GalerijaSlika>, IGalerijaRepository
{
    public GalerijaRepository(RestoranDbContext kontekst) : base(kontekst) { }

    public async Task<List<GalerijaSlika>> ListirajAsync(bool samoAktivne)
    {
        var upit = _dbSet.AsQueryable();
        if (samoAktivne) upit = upit.Where(g => g.Aktivan);

        return await upit.OrderBy(g => g.Redosled).ThenBy(g => g.Id).ToListAsync();
    }

    public async Task<int> NajveciRedosledAsync() =>
        await _dbSet.MaxAsync(g => (int?)g.Redosled) ?? 0;
}
