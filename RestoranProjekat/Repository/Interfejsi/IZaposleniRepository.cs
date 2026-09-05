using Domain.Entiteti;
using Repository.Rezultati;

namespace Repository.Interfejsi;

public interface IZaposleniRepository : IRepository<Zaposleni>
{
    Task<List<Zaposleni>> ListirajSaKorisnikomAsync();

    Task<(List<ZaposleniSaUlogomRezultat> Podaci, int Ukupno)> PretraziAsync(
        string? pretraga, string? uloga, bool? aktivan, int strana, int velicinaStrane);

    Task<List<ZaposleniSaUlogomRezultat>> ListirajZaSajtAsync();

    Task<Zaposleni?> DobaviPoKorisnikIdAsync(string korisnikId);

    Task<Zaposleni?> DobaviSaKorisnikomAsync(int zaposleniId);
}
