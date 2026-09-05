using Domain.Entiteti;

namespace Repository.Interfejsi;

public interface INamirnicaRepository : IRepository<Namirnica>
{
    Task<List<Namirnica>> ListirajAsync();

    Task<List<Namirnica>> ListirajIspodPragaAsync();

    Task<bool> PostojiNazivAsync(string naziv, int? iskljuciId = null);

    Task<bool> KoristiSeURecepturiAsync(int namirnicaId);
}
