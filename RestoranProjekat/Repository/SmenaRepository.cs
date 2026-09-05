using Domain.Entiteti;
using Microsoft.EntityFrameworkCore;
using Repository.Interfejsi;

namespace Repository;

public class SmenaRepository : BazniRepository<Smena>, ISmenaRepository
{
    public SmenaRepository(RestoranDbContext kontekst) : base(kontekst) { }

    public async Task<List<Smena>> ListirajUOpseguAsync(DateOnly od, DateOnly do_) =>
        await _dbSet
            .Include(s => s.Zaposleni).ThenInclude(z => z.Korisnik)
            .Where(s => s.Datum >= od && s.Datum <= do_)
            .OrderBy(s => s.Datum).ThenBy(s => s.VremePocetka)
            .ToListAsync();

    public async Task<List<Smena>> ListirajZaZaposlenogAsync(int zaposleniId, DateOnly? od = null, DateOnly? do_ = null)
    {
        var upit = _dbSet
            .Include(s => s.Zaposleni).ThenInclude(z => z.Korisnik)
            .Where(s => s.ZaposleniId == zaposleniId);

        if (od.HasValue) upit = upit.Where(s => s.Datum >= od.Value);
        if (do_.HasValue) upit = upit.Where(s => s.Datum <= do_.Value);

        return await upit.OrderBy(s => s.Datum).ThenBy(s => s.VremePocetka).ToListAsync();
    }

    public async Task<List<Smena>> ListirajZaProveruPreklapanjaAsync(int zaposleniId, DateOnly datum, int? iskljuciSmenuId = null)
    {
        var od = datum.AddDays(-1);
        var do_ = datum.AddDays(1);

        return await _dbSet
            .Where(s => s.ZaposleniId == zaposleniId &&
                        s.Datum >= od && s.Datum <= do_ &&
                        (!iskljuciSmenuId.HasValue || s.Id != iskljuciSmenuId.Value))
            .ToListAsync();
    }
}
