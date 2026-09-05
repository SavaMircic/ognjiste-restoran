using Domain.Entiteti;
using Microsoft.EntityFrameworkCore;
using Repository.Interfejsi;

namespace Repository;

public class NamirnicaRepository : BazniRepository<Namirnica>, INamirnicaRepository
{
    public NamirnicaRepository(RestoranDbContext kontekst) : base(kontekst) { }

    public async Task<List<Namirnica>> ListirajAsync() =>
        await _dbSet.OrderBy(n => n.Naziv).ToListAsync();

    public async Task<List<Namirnica>> ListirajIspodPragaAsync() =>
        await _dbSet
            .Where(n => n.TrenutnaKolicina < n.MinimalniPrag)
            .OrderBy(n => n.Naziv)
            .ToListAsync();

    public async Task<bool> PostojiNazivAsync(string naziv, int? iskljuciId = null) =>
        await _dbSet.AnyAsync(n => n.Naziv == naziv && (!iskljuciId.HasValue || n.Id != iskljuciId.Value));

    public async Task<bool> KoristiSeURecepturiAsync(int namirnicaId) =>
        await _kontekst.Recepture.AnyAsync(r => r.NamirnicaId == namirnicaId);
}
