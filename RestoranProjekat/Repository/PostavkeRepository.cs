using Domain.Entiteti;
using Microsoft.EntityFrameworkCore;
using Repository.Interfejsi;

namespace Repository;

public class PostavkeRepository : BazniRepository<PostavkeRestorana>, IPostavkeRepository
{
    public PostavkeRepository(RestoranDbContext kontekst) : base(kontekst) { }

    public async Task<PostavkeRestorana?> DobaviAsync() => await _dbSet.FirstOrDefaultAsync();
}
