using API.Autorizacija;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Services;
using Services.DTO;

namespace API.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly IAuthServis _authServis;

    public AuthController(IAuthServis authServis)
    {
        _authServis = authServis;
    }

    [HttpPost("registracija")]
    [SlucajKoriscenja("Registracija")]
    public async Task<IActionResult> Registracija(RegistracijaDto dto)
    {
        var (uspesno, greska, korisnikId) = await _authServis.RegistrujAsync(dto);
        if (!uspesno) return BadRequest(new { Poruka = greska });
        return StatusCode(StatusCodes.Status201Created,
            new { KorisnikId = korisnikId, Poruka = "Registracija uspešna. Proverite email za potvrdu naloga." });
    }

    [HttpPost("potvrda-email")]
    [SlucajKoriscenja("PotvrdaEmail")]
    public async Task<IActionResult> PotvrdaEmail(PotvrdaEmailDto dto)
    {
        var (uspesno, greska) = await _authServis.PotvrdiEmailAsync(dto);
        if (!uspesno) return BadRequest(new { Poruka = greska });
        return Ok(new { Poruka = "Email adresa je uspešno potvrđena." });
    }

    [HttpPost("posalji-ponovo-potvrdu")]
    [EnableRateLimiting(PolitikeOgranicenja.SlanjeMejla)]
    [SlucajKoriscenja("PonovoPosaljiPotvrdu")]
    public async Task<IActionResult> PosaljiPonovoPotvrdu(PonovnoSlanjePotvrdeDto dto)
    {
        await _authServis.PonoviPotvrduEmailaAsync(dto);
        return Ok(new { Poruka = "Ako nalog sa ovom adresom postoji i još nije potvrđen, poslali smo nov link za potvrdu." });
    }

    [HttpPost("prijava")]
    [SlucajKoriscenja("Prijava")]
    public async Task<IActionResult> Prijava(PrijavaDto dto)
    {
        var (uspesno, greska, token) = await _authServis.PrijaviAsync(dto);
        if (!uspesno) return Unauthorized(new { Poruka = greska });
        return Ok(token);
    }

    [HttpPost("osvezi-token")]
    [SlucajKoriscenja("OsveziToken")]
    public async Task<IActionResult> OsveziToken(OsveziTokenDto dto)
    {
        var (uspesno, greska, token) = await _authServis.OsveziTokenAsync(dto);
        if (!uspesno) return Unauthorized(new { Poruka = greska });
        return Ok(token);
    }

    [HttpPost("zaboravljena-lozinka")]
    [EnableRateLimiting(PolitikeOgranicenja.SlanjeMejla)]
    [SlucajKoriscenja("ZaboravljenaLozinka")]
    public async Task<IActionResult> ZaboravljenaLozinka(ZaboravljenaLozinkaDto dto)
    {
        await _authServis.ZapocniResetLozinkeAsync(dto);
        return Ok(new { Poruka = "Ako nalog sa ovom adresom postoji, poslato je uputstvo za reset lozinke." });
    }

    [HttpPost("resetuj-lozinku")]
    [SlucajKoriscenja("ResetujLozinku")]
    public async Task<IActionResult> ResetujLozinku(ResetLozinkeDto dto)
    {
        var (uspesno, greska) = await _authServis.ResetujLozinkuAsync(dto);
        if (!uspesno) return BadRequest(new { Poruka = greska });
        return Ok(new { Poruka = "Lozinka je uspešno promenjena." });
    }
}
