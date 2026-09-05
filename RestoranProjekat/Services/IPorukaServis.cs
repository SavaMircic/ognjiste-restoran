using Services.DTO;

namespace Services;

public interface IPorukaServis
{
    Task<(bool Uspesno, string? Greska, PorukaDto? Poruka)> PosaljiAsync(string? korisnikId, PosaljiPorukuDto dto);

    Task<List<PorukaDto>> MojePorukeAsync(string korisnikId);
    Task<PaginiranaListaDto<PorukaDto>> PretraziAsync(PorukePretragaDto filter);
    Task<(bool Uspesno, string? Greska)> OdgovoriAsync(int id, string adminKorisnikId, OdgovorNaPorukuDto dto);
    Task<(bool Uspesno, string? Greska)> OznaciProcitanomAsync(int id);
}
