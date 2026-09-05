using Domain.Entiteti;
using Microsoft.EntityFrameworkCore;
using Repository.Interfejsi;

namespace Repository;

public class BonusRepository : BazniRepository<Bonus>, IBonusRepository
{
    public BonusRepository(RestoranDbContext kontekst) : base(kontekst) { }

    public async Task<List<Bonus>> ListirajZaZaposlenogAsync(int zaposleniId) =>
        await _dbSet
            .Include(b => b.Zaposleni).ThenInclude(z => z.Korisnik)
            .Include(b => b.DodelioZaposleni).ThenInclude(z => z.Korisnik)
            .Where(b => b.ZaposleniId == zaposleniId)
            .OrderByDescending(b => b.DatumPocetka)
            .ToListAsync();

    public async Task<(List<Bonus> Podaci, int Ukupno)> PretraziAsync(
        int? zaposleniId, DateOnly? datumOd, DateOnly? datumDo, int strana, int velicinaStrane)
    {
        var upit = _dbSet
            .Include(b => b.Zaposleni).ThenInclude(z => z.Korisnik)
            .Include(b => b.DodelioZaposleni).ThenInclude(z => z.Korisnik)
            .AsQueryable();

        if (zaposleniId.HasValue)
            upit = upit.Where(b => b.ZaposleniId == zaposleniId.Value);

        if (datumOd.HasValue)
            upit = upit.Where(b => b.DatumKraja >= datumOd.Value);

        if (datumDo.HasValue)
            upit = upit.Where(b => b.DatumPocetka <= datumDo.Value);

        var ukupno = await upit.CountAsync();

        var podaci = await upit
            .OrderByDescending(b => b.DatumPocetka)
            .Skip((strana - 1) * velicinaStrane)
            .Take(velicinaStrane)
            .ToListAsync();

        return (podaci, ukupno);
    }
}
