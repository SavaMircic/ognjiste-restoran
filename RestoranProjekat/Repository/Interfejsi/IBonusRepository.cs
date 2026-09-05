using Domain.Entiteti;

namespace Repository.Interfejsi;

public interface IBonusRepository : IRepository<Bonus>
{
    Task<List<Bonus>> ListirajZaZaposlenogAsync(int zaposleniId);

    Task<(List<Bonus> Podaci, int Ukupno)> PretraziAsync(
        int? zaposleniId, DateOnly? datumOd, DateOnly? datumDo, int strana, int velicinaStrane);
}
