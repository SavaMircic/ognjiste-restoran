using Domain.Entiteti;

namespace Repository.Interfejsi;

public interface ISmenaRepository : IRepository<Smena>
{
    Task<List<Smena>> ListirajUOpseguAsync(DateOnly od, DateOnly do_);

    Task<List<Smena>> ListirajZaZaposlenogAsync(int zaposleniId, DateOnly? od = null, DateOnly? do_ = null);

    Task<List<Smena>> ListirajZaProveruPreklapanjaAsync(int zaposleniId, DateOnly datum, int? iskljuciSmenuId = null);
}
