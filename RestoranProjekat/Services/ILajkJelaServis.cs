using Services.DTO;

namespace Services;

public interface ILajkJelaServis
{
    Task<(bool Uspesno, string? Greska, int StatusKod)> LajkujAsync(int stavkaMenijaId, string korisnikId);
    Task<(bool Uspesno, string? Greska)> UkloniLajkAsync(int stavkaMenijaId, string korisnikId);
    Task<List<StavkaMenijaDto>> OmiljenaJelaAsync(string korisnikId);
}
