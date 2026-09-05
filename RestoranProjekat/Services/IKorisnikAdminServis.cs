using Services.DTO;

namespace Services;

public interface IKorisnikAdminServis
{
    Task<PaginiranaListaDto<KorisnikAdminDto>> PretraziAsync(KorisniciPretragaDto filter);
    Task<KorisnikDetaljDto?> DobaviDetaljAsync(string korisnikId);

    Task<(bool Uspesno, string? Greska)> BlokirajAsync(
        string korisnikId, string adminKorisnikId, BlokirajKorisnikaDto dto);

    Task<(bool Uspesno, string? Greska)> OdblokirajAsync(string korisnikId, string adminKorisnikId);

    Task<(bool Uspesno, string? Greska)> ZabraniKomentarisanjeAsync(
        string korisnikId, string adminKorisnikId, ZabraniKomentarisanjeDto dto);

    Task<(bool Uspesno, string? Greska)> UkiniZabranuKomentarisanjaAsync(
        string korisnikId, string adminKorisnikId);
}
