using Domain.Entiteti;
using Repository.Rezultati;

namespace Repository.Interfejsi;

public interface IPorudzbinaRepository : IRepository<Porudzbina>
{
    Task<List<Porudzbina>> ListirajAktivneAsync();

    Task<Porudzbina?> DobaviDetaljAsync(int id);

    Task<Porudzbina?> DobaviOtvorenuZaStoAsync(int stoId);

    Task<int> BrojOtvorenihAsync();

    Task<List<RacunGostaRezultat>> RacuniGostaAsync(string korisnikId, int maks);
}
