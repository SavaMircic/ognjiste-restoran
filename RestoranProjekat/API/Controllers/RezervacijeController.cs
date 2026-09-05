using System.Security.Claims;
using API.Autorizacija;
using Domain.Konstante;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services;
using Services.DTO;

namespace API.Controllers;

[ApiController]
[Route("api/rezervacije")]
[Authorize]
public class RezervacijeController : ControllerBase
{
    private readonly IRezervacijaServis _rezervacijaServis;

    public RezervacijeController(IRezervacijaServis rezervacijaServis)
    {
        _rezervacijaServis = rezervacijaServis;
    }

    private string TrenutniKorisnikId => User.FindFirstValue(ClaimTypes.NameIdentifier)!;

    [HttpGet("dostupnost")]
    [SlucajKoriscenja("ProveriDostupnost", Uloge.Korisnik, Uloge.Administrator)]
    public async Task<IActionResult> Dostupnost([FromQuery] DostupnostUpitDto upit)
    {
        var stolovi = await _rezervacijaServis.ProveriDostupnostAsync(upit);
        return Ok(stolovi);
    }

    [HttpPost]
    [SlucajKoriscenja("KreirajRezervaciju", Uloge.Korisnik)]
    public async Task<IActionResult> KreirajRezervaciju(KreirajRezervacijuDto dto)
    {
        var (uspesno, greska, rezervacija) = await _rezervacijaServis.KreirajAsync(TrenutniKorisnikId, dto);
        if (!uspesno) return BadRequest(new { Poruka = greska });
        return StatusCode(StatusCodes.Status201Created, rezervacija);
    }

    [HttpGet("moje")]
    [SlucajKoriscenja("MojeRezervacije", Uloge.Korisnik)]
    public async Task<IActionResult> Moje()
    {
        var rezervacije = await _rezervacijaServis.MojeAsync(TrenutniKorisnikId);
        return Ok(rezervacije);
    }

    [HttpDelete("{id:int}")]
    [SlucajKoriscenja("OtkaziRezervaciju", Uloge.Korisnik)]
    public async Task<IActionResult> Otkazi(int id)
    {
        var (uspesno, greska) = await _rezervacijaServis.OtkaziAsync(id, TrenutniKorisnikId);
        if (!uspesno) return BadRequest(new { Poruka = greska });
        return NoContent();
    }

    [HttpGet("danas")]
    [SlucajKoriscenja("RezervacijeZaDanas", Uloge.Konobar, Uloge.Administrator)]
    public async Task<IActionResult> Danas()
    {
        var rezervacije = await _rezervacijaServis.ZaDanasAsync();
        return Ok(rezervacije);
    }

    [HttpPatch("{id:int}/prijavi-dolazak")]
    [SlucajKoriscenja("PrijaviDolazak", Uloge.Konobar, Uloge.Administrator)]
    public async Task<IActionResult> PrijaviDolazak(int id)
    {
        var (uspesno, greska) = await _rezervacijaServis.PrijaviDolazakAsync(id, TrenutniKorisnikId);
        if (!uspesno) return BadRequest(new { Poruka = greska });
        return NoContent();
    }

    [HttpPatch("{id:int}/istekla")]
    [SlucajKoriscenja("OznaciRezervacijuIsteklom", Uloge.Konobar, Uloge.Administrator)]
    public async Task<IActionResult> OznaciIsteklom(int id)
    {
        var (uspesno, greska) = await _rezervacijaServis.OznaciIsteklomAsync(id);
        if (!uspesno) return BadRequest(new { Poruka = greska });
        return NoContent();
    }
}
