using Services.DTO;

namespace Services;

public interface IIzvestajServis
{
    Task<PrihodIzvestajDto> PrihodAsync(PrihodUpitDto upit);
    Task<List<UcinakZaposlenogDto>> UcinakZaposlenihAsync(UcinakUpitDto upit);
    Task<List<TopJeloDto>> TopJelaAsync(TopJelaUpitDto upit);

    Task<(bool Uspesno, string? Greska, UcinakZaposlenogDto? Statistika)> MojaStatistikaAsync(
        string trenutniKorisnikId, MojaStatistikaUpitDto upit);

    Task<(bool Uspesno, string? Greska, List<IstorijaDanDto>? Dani)> MojaIstorijaAsync(
        string trenutniKorisnikId, IstorijaRadaUpitDto upit);
}
