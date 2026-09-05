using Services.DTO;

namespace Services;

public interface IReceptureServis
{
    Task<List<ReceptStavkaDto>> ListirajAsync(int stavkaMenijaId);
    Task<(bool Uspesno, string? Greska)> DodajAsync(int stavkaMenijaId, DodajURecepturuDto dto);
    Task<(bool Uspesno, string? Greska)> IzmeniKolicinuAsync(int stavkaMenijaId, int namirnicaId, IzmenaKolicineDto dto);
    Task<(bool Uspesno, string? Greska)> UkloniAsync(int stavkaMenijaId, int namirnicaId);
}
