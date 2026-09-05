using Domain.Entiteti;
using Domain.Enumi;
using Repository.Rezultati;

namespace Repository.Interfejsi;

public interface IRezervacijaRepository : IRepository<Rezervacija>
{
    Task<List<Sto>> DobaviSlobodneStoloveAsync(DateTime datumVreme);
    Task<List<Rezervacija>> ListirajPoKoduAsync(string kod);

    Task<bool> PostojiKonfliktAsync(int stoId, DateTime datumVreme);

    Task<Rezervacija?> DobaviPoKoduAsync(string kod);

    Task<Rezervacija?> DobaviDetaljAsync(int id);

    Task<List<Rezervacija>> ListirajZaKorisnikaAsync(string korisnikId);

    Task<PouzdanostGostaRezultat> PouzdanostGostaAsync(string korisnikId);

    Task<List<Rezervacija>> ListirajDospeleZaIstekAsync(DateTime granica);

    Task<(List<Rezervacija> Podaci, int Ukupno)> PretraziAsync(
        DateTime? datum, StatusRezervacije? status, string? pretraga, int strana, int velicinaStrane);
    Task<int> BrojAktivnihUOpseguAsync(DateTime od, DateTime doIsklj);
}
