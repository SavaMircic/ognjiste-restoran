using Domain.Entiteti;
using Domain.Enumi;

namespace Repository.Interfejsi;

public interface IPrijavaProblemaRepository : IRepository<PrijavaProblema>
{
    Task<List<PrijavaProblema>> ListirajZaZaposlenogAsync(int zaposleniId);

    Task<(List<PrijavaProblema> Podaci, int Ukupno)> PretraziAsync(
        StatusPrijaveProblema? status, KategorijaProblema? kategorija, PrioritetProblema? prioritet,
        int strana, int velicinaStrane);

    Task<PrijavaProblema?> DobaviSaImenimaAsync(int id);
}
