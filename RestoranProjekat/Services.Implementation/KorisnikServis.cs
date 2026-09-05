using Domain.Entiteti;
using Microsoft.AspNetCore.Identity;
using Services.DTO;

namespace Services.Implementation;

public class KorisnikServis : IKorisnikServis
{
    private readonly UserManager<Korisnik> _userManager;
    private readonly ISkladisteSlika _skladisteSlika;

    public KorisnikServis(UserManager<Korisnik> userManager, ISkladisteSlika skladisteSlika)
    {
        _userManager = userManager;
        _skladisteSlika = skladisteSlika;
    }

    public async Task<(bool Uspesno, string? Greska, string? SlikaUrl)> PostaviProfilnuSlikuAsync(
        string korisnikId, DatotekaZaUploadDto datoteka)
    {
        var korisnik = await _userManager.FindByIdAsync(korisnikId);
        if (korisnik == null) return (false, "Korisnik ne postoji.", null);

        var (uspesno, greska, url) = await _skladisteSlika.SacuvajAsync(
            datoteka, ISkladisteSlika.Podfolderi.Profilne);
        if (!uspesno) return (false, greska, null);

        var stara = korisnik.SlikaUrl;
        korisnik.SlikaUrl = url;

        var rezultat = await _userManager.UpdateAsync(korisnik);
        if (!rezultat.Succeeded)
        {
            await _skladisteSlika.ObrisiAsync(url);
            return (false, string.Join(" ", rezultat.Errors.Select(e => e.Description)), null);
        }

        await _skladisteSlika.ObrisiAsync(stara);
        return (true, null, url);
    }

    public async Task<KorisnikProfilDto?> DobaviProfilAsync(string korisnikId)
    {
        var korisnik = await _userManager.FindByIdAsync(korisnikId);
        if (korisnik == null) return null;

        return new KorisnikProfilDto
        {
            Id = korisnik.Id,
            Ime = korisnik.Ime,
            Prezime = korisnik.Prezime,
            Email = korisnik.Email ?? string.Empty,
            BrojTelefona = korisnik.PhoneNumber,
            SlikaUrl = korisnik.SlikaUrl,
            DatumRegistracije = korisnik.DatumRegistracije
        };
    }

    public async Task<(bool Uspesno, string? Greska)> IzmeniProfilAsync(string korisnikId, IzmenaProfilaDto dto)
    {
        var korisnik = await _userManager.FindByIdAsync(korisnikId);
        if (korisnik == null) return (false, "Korisnik ne postoji.");

        korisnik.Ime = dto.Ime;
        korisnik.Prezime = dto.Prezime;
        korisnik.PhoneNumber = dto.BrojTelefona;

        var rezultat = await _userManager.UpdateAsync(korisnik);
        return rezultat.Succeeded ? (true, null) : (false, string.Join(" ", rezultat.Errors.Select(e => e.Description)));
    }

    public async Task<(bool Uspesno, string? Greska)> PromeniLozinkuAsync(string korisnikId, PromenaLozinkeDto dto)
    {
        var korisnik = await _userManager.FindByIdAsync(korisnikId);
        if (korisnik == null) return (false, "Korisnik ne postoji.");

        var rezultat = await _userManager.ChangePasswordAsync(korisnik, dto.StaraLozinka, dto.NovaLozinka);
        return rezultat.Succeeded ? (true, null) : (false, string.Join(" ", rezultat.Errors.Select(e => e.Description)));
    }
}
