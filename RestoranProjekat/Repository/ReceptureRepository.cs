using Domain.Entiteti;
using Microsoft.EntityFrameworkCore;
using Repository.Interfejsi;

namespace Repository;

public class ReceptureRepository : BazniRepository<Receptura>, IReceptureRepository
{
    public ReceptureRepository(RestoranDbContext kontekst) : base(kontekst) { }

    public async Task<List<Receptura>> ListirajZaStavkuAsync(int stavkaMenijaId) =>
        await _dbSet.Include(r => r.Namirnica)
            .Where(r => r.StavkaMenijaId == stavkaMenijaId)
            .ToListAsync();

    public async Task<Receptura?> DobaviAsync(int stavkaMenijaId, int namirnicaId) =>
        await _dbSet.FirstOrDefaultAsync(r => r.StavkaMenijaId == stavkaMenijaId && r.NamirnicaId == namirnicaId);
}
