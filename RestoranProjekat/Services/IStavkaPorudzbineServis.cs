using Domain.Enumi;
using Services.DTO;

namespace Services;

public interface IStavkaPorudzbineServis
{
    Task<List<RedCekanjaStavkaDto>> RedCekanjaAsync(Odrediste odrediste, string trenutniKorisnikId);
    Task<(bool Uspesno, string? Greska)> PreuzmiAsync(int stavkaId, string trenutniKorisnikId);
    Task<(bool Uspesno, string? Greska)> ZavrsiAsync(int stavkaId, string trenutniKorisnikId);
    Task<(bool Uspesno, string? Greska)> IsporuciAsync(int stavkaId);
}
