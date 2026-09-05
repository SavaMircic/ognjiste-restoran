using Domain.Entiteti;

namespace Repository.Interfejsi;

public interface IStavkaMenijaRepository : IRepository<StavkaMenija>
{
    Task<(List<StavkaMenija> Podaci, int Ukupno)> PretraziAsync(
        int? kategorijaId, string? pretraga, string? sortiranje, int strana, int velicinaStrane);

    Task<StavkaMenija?> DobaviDetaljAsync(int id);

    Task<List<SlikaStavkeMenija>> ListirajDodatneSlikeAsync(int stavkaMenijaId);

    Task<SlikaStavkeMenija?> DobaviDodatnuSlikuAsync(int stavkaMenijaId, int slikaId);

    Task<int> NajveciRedosledSlikeAsync(int stavkaMenijaId);
}
