using Domain.Entiteti;

namespace Repository.Interfejsi;

public interface IStoRepository : IRepository<Sto>
{
    Task<List<Sto>> ListirajAsync();
    Task<bool> PostojiBrojStolaAsync(int brojStola, int? iskljuciId = null);
}
