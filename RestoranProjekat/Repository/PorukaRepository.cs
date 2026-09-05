using Domain.Entiteti;
using Domain.Enumi;
using Microsoft.EntityFrameworkCore;
using Repository.Interfejsi;

namespace Repository;

public class PorukaRepository : BazniRepository<Poruka>, IPorukaRepository
{
    public PorukaRepository(RestoranDbContext kontekst) : base(kontekst) { }

    public async Task<(List<Poruka> Podaci, int Ukupno)> PretraziAsync(
        StatusPoruke? status, KategorijaPoruke? kategorija, int strana, int velicinaStrane)
    {
        var upit = _dbSet.Include(p => p.Korisnik).AsQueryable();

        if (status.HasValue)
            upit = upit.Where(p => p.Status == status.Value);

        if (kategorija.HasValue)
            upit = upit.Where(p => p.Kategorija == kategorija.Value);

        var ukupno = await upit.CountAsync();

        var podaci = await upit
            .OrderByDescending(p => p.Kategorija == KategorijaPoruke.Rezervacija)
            .ThenByDescending(p => p.DatumSlanja)
            .Skip((strana - 1) * velicinaStrane)
            .Take(velicinaStrane)
            .ToListAsync();

        return (podaci, ukupno);
    }

    public async Task<List<Poruka>> ListirajZaKorisnikaAsync(string korisnikId) =>
        await _dbSet
            .Where(p => p.KorisnikId == korisnikId)
            .OrderByDescending(p => p.DatumSlanja)
            .ToListAsync();

    public async Task<Poruka?> DobaviDetaljAsync(int id) =>
        await _dbSet.Include(p => p.Korisnik).FirstOrDefaultAsync(p => p.Id == id);
    public async Task<int> BrojPoStatusuAsync(Domain.Enumi.StatusPoruke status) =>
        await _dbSet.CountAsync(p => p.Status == status);

    public async Task<int> BrojPrioritetnihNovihAsync() =>
        await _dbSet.CountAsync(p => p.Status == Domain.Enumi.StatusPoruke.Novo &&
                                     p.Kategorija == Domain.Enumi.KategorijaPoruke.Rezervacija);
}