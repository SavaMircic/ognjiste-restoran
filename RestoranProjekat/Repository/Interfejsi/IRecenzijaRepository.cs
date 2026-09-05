using Domain.Entiteti;
using Domain.Enumi;

namespace Repository.Interfejsi;

public interface IRecenzijaRepository : IRepository<Recenzija>
{
    Task<(List<Recenzija> Podaci, int Ukupno)> PretraziAsync(
        int? stavkaMenijaId, TipRecenzije? tipRecenzije, string? sortiranje, int strana, int velicinaStrane);

    Task<Recenzija?> DobaviPostojecuAsync(string korisnikId, TipRecenzije tipRecenzije, int? stavkaMenijaId);

    Task<Dictionary<int, (double ProsecnaOcena, int BrojRecenzija)>> StatistikaZaStavkeAsync(IEnumerable<int> stavkaMenijaIds);

    Task<List<Recenzija>> ListirajZaKorisnikaAsync(string korisnikId, int maks);
    Task<int> BrojBezOdgovoraAsync();
}
