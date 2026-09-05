using Domain.Entiteti;
using Domain.Enumi;
using Microsoft.EntityFrameworkCore;
using Repository.Interfejsi;

namespace Repository;

public class PrijavaProblemaRepository : BazniRepository<PrijavaProblema>, IPrijavaProblemaRepository
{
    public PrijavaProblemaRepository(RestoranDbContext kontekst) : base(kontekst) { }

    private IQueryable<PrijavaProblema> SaImenima() =>
        _dbSet
            .Include(p => p.PrijavioZaposleni).ThenInclude(z => z.Korisnik)
            .Include(p => p.ResioZaposleni).ThenInclude(z => z!.Korisnik);

    public async Task<List<PrijavaProblema>> ListirajZaZaposlenogAsync(int zaposleniId) =>
        await SaImenima()
            .Where(p => p.PrijavioZaposleniId == zaposleniId)
            .OrderByDescending(p => p.DatumPrijave)
            .ToListAsync();

    public async Task<PrijavaProblema?> DobaviSaImenimaAsync(int id) =>
        await SaImenima().FirstOrDefaultAsync(p => p.Id == id);

    public async Task<(List<PrijavaProblema> Podaci, int Ukupno)> PretraziAsync(
        StatusPrijaveProblema? status, KategorijaProblema? kategorija, PrioritetProblema? prioritet,
        int strana, int velicinaStrane)
    {
        var upit = SaImenima();

        if (status.HasValue) upit = upit.Where(p => p.Status == status.Value);
        if (kategorija.HasValue) upit = upit.Where(p => p.Kategorija == kategorija.Value);
        if (prioritet.HasValue) upit = upit.Where(p => p.Prioritet == prioritet.Value);

        var ukupno = await upit.CountAsync();

        var podaci = await upit
            .OrderBy(p => p.Status == StatusPrijaveProblema.Resena || p.Status == StatusPrijaveProblema.Odbijena)
            .ThenByDescending(p => p.Prioritet)
            .ThenBy(p => p.DatumPrijave)
            .Skip((strana - 1) * velicinaStrane)
            .Take(velicinaStrane)
            .ToListAsync();

        return (podaci, ukupno);
    }
}
