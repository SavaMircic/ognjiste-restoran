using Domain.Entiteti;

namespace Repository.Interfejsi;

public interface IGalerijaRepository : IRepository<GalerijaSlika>
{
    Task<List<GalerijaSlika>> ListirajAsync(bool samoAktivne);

    Task<int> NajveciRedosledAsync();
}
