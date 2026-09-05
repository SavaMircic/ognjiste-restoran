using Services.DTO;

namespace Services;

public interface ISmenaServis
{
    Task<(bool Uspesno, string? Greska, List<SmenaDto>? Smene)> MojRasporedAsync(string trenutniKorisnikId, DateOnly? nedelja);

    Task<RasporedNedeljeDto> RasporedNedeljeAsync(DateOnly? nedelja);

    Task<(bool Uspesno, string? Greska, SmenaDto? Smena)> KreirajAsync(KreirajSmenuDto dto);
    Task<(bool Uspesno, string? Greska)> IzmeniAsync(int id, IzmenaSmeneDto dto);
    Task<(bool Uspesno, string? Greska)> ObrisiAsync(int id);
}
