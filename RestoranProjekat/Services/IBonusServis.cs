using Services.DTO;

namespace Services;

public interface IBonusServis
{
    Task<(bool Uspesno, string? Greska, List<BonusDto>? Bonusi)> MojiBonusiAsync(string trenutniKorisnikId);
    Task<PaginiranaListaDto<BonusDto>> PretraziAsync(BonusiPretragaDto filter);
    Task<(bool Uspesno, string? Greska, List<BonusDto>? Bonusi)> DodeliAsync(
        string trenutniKorisnikId, KreirajBonusDto dto);
}
