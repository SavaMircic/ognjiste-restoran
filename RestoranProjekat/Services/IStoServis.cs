using Services.DTO;

namespace Services;

public interface IStoServis
{
    Task<List<StoDto>> ListirajAsync();
    Task<(bool Uspesno, string? Greska, StoDto? Sto)> KreirajAsync(StoUlazDto dto);
    Task<(bool Uspesno, string? Greska)> IzmeniAsync(int id, StoUlazDto dto);
}
