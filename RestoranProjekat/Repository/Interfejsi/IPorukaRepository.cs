using Domain.Entiteti;
using Domain.Enumi;

namespace Repository.Interfejsi;

public interface IPorukaRepository : IRepository<Poruka>
{
    Task<(List<Poruka> Podaci, int Ukupno)> PretraziAsync(
        StatusPoruke? status, KategorijaPoruke? kategorija, int strana, int velicinaStrane);

    Task<List<Poruka>> ListirajZaKorisnikaAsync(string korisnikId);

    Task<Poruka?> DobaviDetaljAsync(int id);
    Task<int> BrojPoStatusuAsync(Domain.Enumi.StatusPoruke status);

    Task<int> BrojPrioritetnihNovihAsync();
}
