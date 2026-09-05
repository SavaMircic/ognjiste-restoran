using Services.DTO;

namespace Services;

public interface IAuthServis
{
    Task<(bool Uspesno, string? Greska, string? KorisnikId)> RegistrujAsync(RegistracijaDto dto);
    Task<(bool Uspesno, string? Greska)> PotvrdiEmailAsync(PotvrdaEmailDto dto);

    Task PonoviPotvrduEmailaAsync(PonovnoSlanjePotvrdeDto dto);
    Task<(bool Uspesno, string? Greska, TokenOdgovorDto? Token)> PrijaviAsync(PrijavaDto dto);
    Task<(bool Uspesno, string? Greska, TokenOdgovorDto? Token)> OsveziTokenAsync(OsveziTokenDto dto);
    Task ZapocniResetLozinkeAsync(ZaboravljenaLozinkaDto dto);
    Task<(bool Uspesno, string? Greska)> ResetujLozinkuAsync(ResetLozinkeDto dto);
}
