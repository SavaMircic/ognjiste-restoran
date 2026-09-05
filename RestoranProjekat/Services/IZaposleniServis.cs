using Services.DTO;

namespace Services;

public interface IZaposleniServis
{
    Task<PaginiranaListaDto<ZaposleniDto>> PretraziAsync(ZaposleniPretragaDto filter);
    Task<(bool Uspesno, string? Greska, int? ZaposleniId)> KreirajAsync(KreirajZaposlenogDto dto);
    Task<(bool Uspesno, string? Greska)> IzmeniAsync(int zaposleniId, IzmenaZaposlenogDto dto);
    Task<(bool Uspesno, string? Greska)> DeaktivirajAsync(int zaposleniId);

    Task<List<ClanTimaDto>> ListirajTimAsync();

    Task<(bool Uspesno, string? Greska)> IzmeniProfilNaSajtuAsync(int zaposleniId, IzmenaProfilaNaSajtuDto dto);

    Task<(bool Uspesno, string? Greska, string? SlikaUrl)> PostaviSlikuAsync(
        int zaposleniId, DatotekaZaUploadDto datoteka);
}
