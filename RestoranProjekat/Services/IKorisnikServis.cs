using Services.DTO;

namespace Services;

public interface IKorisnikServis
{
    Task<KorisnikProfilDto?> DobaviProfilAsync(string korisnikId);
    Task<(bool Uspesno, string? Greska)> IzmeniProfilAsync(string korisnikId, IzmenaProfilaDto dto);
    Task<(bool Uspesno, string? Greska)> PromeniLozinkuAsync(string korisnikId, PromenaLozinkeDto dto);

    Task<(bool Uspesno, string? Greska, string? SlikaUrl)> PostaviProfilnuSlikuAsync(
        string korisnikId, DatotekaZaUploadDto datoteka);
}
