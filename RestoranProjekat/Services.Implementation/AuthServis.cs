using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Domain.Entiteti;
using Domain.Konstante;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Services.DTO;

namespace Services.Implementation;

public class AuthServis : IAuthServis
{
    private readonly UserManager<Korisnik> _userManager;
    private readonly IConfiguration _konfiguracija;
    private readonly IEmailServis _emailServis;

    public AuthServis(UserManager<Korisnik> userManager, IConfiguration konfiguracija, IEmailServis emailServis)
    {
        _userManager = userManager;
        _konfiguracija = konfiguracija;
        _emailServis = emailServis;
    }

    public async Task<(bool Uspesno, string? Greska, string? KorisnikId)> RegistrujAsync(RegistracijaDto dto)
    {
        var postojeci = await _userManager.FindByEmailAsync(dto.Email);
        if (postojeci != null)
            return (false, "Nalog sa ovom email adresom već postoji.", null);

        var korisnik = new Korisnik
        {
            UserName = dto.Email,
            Email = dto.Email,
            Ime = dto.Ime,
            Prezime = dto.Prezime,
            PhoneNumber = dto.BrojTelefona,
            DatumRegistracije = DateTime.UtcNow,
            Aktivan = true
        };

        var rezultat = await _userManager.CreateAsync(korisnik, dto.Lozinka);
        if (!rezultat.Succeeded)
            return (false, string.Join(" ", rezultat.Errors.Select(e => e.Description)), null);

        await _userManager.AddToRoleAsync(korisnik, Uloge.Korisnik);

        await PosaljiLinkZaPotvrduAsync(korisnik);

        return (true, null, korisnik.Id);
    }

    public async Task<(bool Uspesno, string? Greska)> PotvrdiEmailAsync(PotvrdaEmailDto dto)
    {
        var korisnik = await _userManager.FindByIdAsync(dto.KorisnikId);
        if (korisnik == null)
            return (false, "Korisnik ne postoji.");

        var rezultat = await _userManager.ConfirmEmailAsync(korisnik, dto.Token);
        return rezultat.Succeeded ? (true, null) : (false, "Token nije validan ili je istekao.");
    }

    public async Task PonoviPotvrduEmailaAsync(PonovnoSlanjePotvrdeDto dto)
    {
        var korisnik = await _userManager.FindByEmailAsync(dto.Email);
        if (korisnik == null) return;
        if (await _userManager.IsEmailConfirmedAsync(korisnik)) return;

        await PosaljiLinkZaPotvrduAsync(korisnik);
    }

    public async Task<(bool Uspesno, string? Greska, TokenOdgovorDto? Token)> PrijaviAsync(PrijavaDto dto)
    {
        var korisnik = await _userManager.FindByEmailAsync(dto.Email);
        var blokiran = korisnik != null &&
            (!korisnik.Aktivan || (korisnik.BlokiranDo.HasValue && korisnik.BlokiranDo > DateTime.UtcNow));
        if (korisnik == null || blokiran)
            return (false, "Pogrešan email ili lozinka.", null);

        var lozinkaTacna = await _userManager.CheckPasswordAsync(korisnik, dto.Lozinka);
        if (!lozinkaTacna)
            return (false, "Pogrešan email ili lozinka.", null);

        if (!await _userManager.IsEmailConfirmedAsync(korisnik))
            return (false, "Email adresa nije potvrđena. Proverite Vaš inbox.", null);

        var tokenOdgovor = await GenerisiTokeneAsync(korisnik);
        return (true, null, tokenOdgovor);
    }

    public async Task<(bool Uspesno, string? Greska, TokenOdgovorDto? Token)> OsveziTokenAsync(OsveziTokenDto dto)
    {
        var principal = DobaviPrincipalIzIsteklogTokena(dto.AccessToken);
        if (principal == null)
            return (false, "Nevalidan token.", null);

        var korisnikId = principal.FindFirstValue(ClaimTypes.NameIdentifier);
        var korisnik = korisnikId == null ? null : await _userManager.FindByIdAsync(korisnikId);

        if (korisnik == null || korisnik.RefreshToken != dto.RefreshToken || korisnik.RefreshTokenIstice < DateTime.UtcNow)
            return (false, "Refresh token nije validan ili je istekao. Potrebna je ponovna prijava.", null);

        var tokenOdgovor = await GenerisiTokeneAsync(korisnik);
        return (true, null, tokenOdgovor);
    }

    public async Task ZapocniResetLozinkeAsync(ZaboravljenaLozinkaDto dto)
    {
        var korisnik = await _userManager.FindByEmailAsync(dto.Email);
        if (korisnik == null) return;

        var token = await _userManager.GeneratePasswordResetTokenAsync(korisnik);
        var enkodiraniToken = Uri.EscapeDataString(token);
        var link = $"{_konfiguracija["Frontend:BazniUrl"]}/reset-lozinke?email={Uri.EscapeDataString(dto.Email)}&token={enkodiraniToken}";

        await _emailServis.PosaljiBezPrekidaAsync(
            korisnik.Email!,
            "Resetovanje lozinke",
            $"<p>Kliknite na link da postavite novu lozinku:</p><p><a href='{ZaHtmlAtribut(link)}'>Resetuj lozinku</a></p><p>Ako niste Vi zatražili ovo, slobodno ignorišite ovaj mejl.</p>");
    }

    public async Task<(bool Uspesno, string? Greska)> ResetujLozinkuAsync(ResetLozinkeDto dto)
    {
        var korisnik = await _userManager.FindByEmailAsync(dto.Email);
        if (korisnik == null)
            return (false, "Token nije validan.");

        var rezultat = await _userManager.ResetPasswordAsync(korisnik, dto.Token, dto.NovaLozinka);
        return rezultat.Succeeded ? (true, null) : (false, string.Join(" ", rezultat.Errors.Select(e => e.Description)));
    }

    private async Task<TokenOdgovorDto> GenerisiTokeneAsync(Korisnik korisnik)
    {
        var uloge = await _userManager.GetRolesAsync(korisnik);

        var claimovi = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, korisnik.Id),
            new(ClaimTypes.Email, korisnik.Email ?? string.Empty),
            new("ime", korisnik.Ime),
            new("prezime", korisnik.Prezime)
        };
        claimovi.AddRange(uloge.Select(u => new Claim(ClaimTypes.Role, u)));

        var kljuc = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_konfiguracija["Jwt:Kljuc"]!));
        var kredencijali = new SigningCredentials(kljuc, SecurityAlgorithms.HmacSha256);
        var istice = DateTime.UtcNow.AddMinutes(double.Parse(_konfiguracija["Jwt:MinutaVazenja"]!));

        var jwtToken = new JwtSecurityToken(
            issuer: _konfiguracija["Jwt:Izdavac"],
            audience: _konfiguracija["Jwt:Publika"],
            claims: claimovi,
            expires: istice,
            signingCredentials: kredencijali);

        var refreshToken = GenerisiRefreshToken();
        korisnik.RefreshToken = refreshToken;
        korisnik.RefreshTokenIstice = DateTime.UtcNow.AddDays(7);
        await _userManager.UpdateAsync(korisnik);

        return new TokenOdgovorDto
        {
            AccessToken = new JwtSecurityTokenHandler().WriteToken(jwtToken),
            RefreshToken = refreshToken,
            IsticeAccessToken = istice,
            KorisnikId = korisnik.Id,
            Ime = korisnik.Ime,
            Prezime = korisnik.Prezime,
            Email = korisnik.Email ?? string.Empty,
            Uloge = uloge.ToList()
        };
    }

    private async Task PosaljiLinkZaPotvrduAsync(Korisnik korisnik)
    {
        var token = await _userManager.GenerateEmailConfirmationTokenAsync(korisnik);
        var enkodiraniToken = Uri.EscapeDataString(token);
        var link = $"{_konfiguracija["Frontend:BazniUrl"]}/potvrda-email?korisnikId={korisnik.Id}&token={enkodiraniToken}";

        await _emailServis.PosaljiBezPrekidaAsync(
            korisnik.Email!,
            "Potvrdite Vaš nalog",
            $"<p>Dobrodošli u naš restoran! Kliknite na link da potvrdite email adresu:</p><p><a href='{ZaHtmlAtribut(link)}'>Potvrdi email</a></p>");
    }

    private static string ZaHtmlAtribut(string url) =>
        url.Replace("&", "&amp;").Replace("'", "&#39;");

    private static string GenerisiRefreshToken()
    {
        var slucajniBajtovi = new byte[64];
        using var generator = RandomNumberGenerator.Create();
        generator.GetBytes(slucajniBajtovi);
        return Convert.ToBase64String(slucajniBajtovi);
    }

    private ClaimsPrincipal? DobaviPrincipalIzIsteklogTokena(string token)
    {
        var parametri = new TokenValidationParameters
        {
            ValidateAudience = true,
            ValidateIssuer = true,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_konfiguracija["Jwt:Kljuc"]!)),
            ValidIssuer = _konfiguracija["Jwt:Izdavac"],
            ValidAudience = _konfiguracija["Jwt:Publika"],
            ValidateLifetime = false
        };

        var handler = new JwtSecurityTokenHandler();
        try
        {
            var principal = handler.ValidateToken(token, parametri, out var validovaniToken);
            if (validovaniToken is not JwtSecurityToken jwt ||
                !jwt.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
                return null;
            return principal;
        }
        catch
        {
            return null;
        }
    }
}
