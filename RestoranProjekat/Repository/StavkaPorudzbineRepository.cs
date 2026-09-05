using Domain.Entiteti;
using Domain.Enumi;
using Microsoft.EntityFrameworkCore;
using Repository.Interfejsi;

namespace Repository;

public class StavkaPorudzbineRepository : BazniRepository<StavkaPorudzbine>, IStavkaPorudzbineRepository
{
    public StavkaPorudzbineRepository(RestoranDbContext kontekst) : base(kontekst) { }

    public async Task<List<StavkaPorudzbine>> RedCekanjaAsync(Odrediste odrediste) =>
        await _dbSet
            .Include(s => s.StavkaMenija).ThenInclude(m => m.Kategorija)
            .Include(s => s.Porudzbina).ThenInclude(p => p.Sto)
            .Include(s => s.PripremioZaposleni).ThenInclude(z => z!.Korisnik)
            .Where(s => s.StavkaMenija.Kategorija.Odrediste == odrediste &&
                        (s.Status == StatusStavkePorudzbine.Poslato || s.Status == StatusStavkePorudzbine.UPripremi))
            .OrderBy(s => s.VremeSlanja)
            .ToListAsync();

    public async Task<StavkaPorudzbine?> DobaviSaDetaljimaAsync(int id) =>
        await _dbSet
            .Include(s => s.StavkaMenija).ThenInclude(m => m.Kategorija)
            .Include(s => s.Porudzbina).ThenInclude(p => p.Sto)
            .Include(s => s.Porudzbina).ThenInclude(p => p.Konobar)
            .FirstOrDefaultAsync(s => s.Id == id);

    public async Task<bool> IkadaNarucenaAsync(int stavkaMenijaId) =>
        await _dbSet.AnyAsync(s => s.StavkaMenijaId == stavkaMenijaId);
}
