using Microsoft.EntityFrameworkCore;
using Repository.Interfejsi;

namespace Repository;

public class BazniRepository<T> : IRepository<T> where T : class
{
    protected readonly RestoranDbContext _kontekst;
    protected readonly DbSet<T> _dbSet;

    public BazniRepository(RestoranDbContext kontekst)
    {
        _kontekst = kontekst;
        _dbSet = kontekst.Set<T>();
    }

    public async Task<T?> DobaviPoIdAsync(object id) => await _dbSet.FindAsync(id);

    public IQueryable<T> Upit() => _dbSet.AsQueryable();

    public async Task<T> DodajAsync(T entitet)
    {
        await _dbSet.AddAsync(entitet);
        return entitet;
    }

    public void Azuriraj(T entitet) => _dbSet.Update(entitet);

    public void Obrisi(T entitet) => _dbSet.Remove(entitet);

    public async Task<int> SacuvajPromeneAsync() => await _kontekst.SaveChangesAsync();
}
