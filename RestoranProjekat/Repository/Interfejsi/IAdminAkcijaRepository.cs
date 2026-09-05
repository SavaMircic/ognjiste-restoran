using Domain.Entiteti;

namespace Repository.Interfejsi;

public interface IAdminAkcijaRepository : IRepository<AdminAkcija>
{
    Task<(List<AdminAkcija> Podaci, int Ukupno)> PretraziAsync(
        string? tipAkcije, string? ciljniKorisnikId, string? pretraga,
        DateTime? datumOd, DateTime? datumDo,
        int strana, int velicinaStrane);
}
