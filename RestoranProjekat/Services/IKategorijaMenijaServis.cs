using Services.DTO;

namespace Services;

public interface IKategorijaMenijaServis
{
    Task<List<KategorijaMenijaDto>> ListirajAsync();
    Task<KategorijaMenijaDto> KreirajAsync(KategorijaMenijaUlazDto dto);
    Task<(bool Uspesno, string? Greska)> IzmeniAsync(int id, KategorijaMenijaUlazDto dto);
    Task<(bool Uspesno, string? Greska)> ObrisiAsync(int id);
}
