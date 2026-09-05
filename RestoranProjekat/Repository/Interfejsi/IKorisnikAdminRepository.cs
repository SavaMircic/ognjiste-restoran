using Domain.Entiteti;

namespace Repository.Interfejsi;

public interface IKorisnikAdminRepository
{
    Task<(List<Korisnik> Podaci, int Ukupno)> PretraziAsync(
        string? pretraga, bool? blokiran, int strana, int velicinaStrane);
    Task<int> BrojBlokiranihAsync();
}
