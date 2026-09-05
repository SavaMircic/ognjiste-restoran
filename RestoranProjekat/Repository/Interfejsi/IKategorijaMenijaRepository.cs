using Domain.Entiteti;

namespace Repository.Interfejsi;

public interface IKategorijaMenijaRepository : IRepository<KategorijaMenija>
{
    Task<List<KategorijaMenija>> ListirajPoRedosleduAsync();

    Task<bool> ImaStavkiAsync(int kategorijaId);
}
