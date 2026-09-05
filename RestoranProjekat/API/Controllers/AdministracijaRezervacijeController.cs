using System.Security.Claims;
using API.Autorizacija;
using Domain.Konstante;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services;
using Services.DTO;

namespace API.Controllers;

[ApiController]
[Route("api/administracija/rezervacije")]
[Authorize(Roles = Uloge.Administrator)]
public class AdministracijaRezervacijeController : ControllerBase
{
    private readonly IRezervacijaServis _rezervacijaServis;

    public AdministracijaRezervacijeController(IRezervacijaServis rezervacijaServis)
    {
        _rezervacijaServis = rezervacijaServis;
    }

    private string TrenutniKorisnikId => User.FindFirstValue(ClaimTypes.NameIdentifier)!;

    [HttpPost]
    [SlucajKoriscenja("KreirajAdminRezervaciju", Uloge.Administrator)]
    public async Task<IActionResult> KreirajRezervaciju(KreirajAdminRezervacijuDto dto)
    {
        var (uspesno, greska, rezervacija) = await _rezervacijaServis.KreirajAdminAsync(TrenutniKorisnikId, dto);
        if (!uspesno) return BadRequest(new { Poruka = greska });
        return StatusCode(StatusCodes.Status201Created, rezervacija);
    }

    [HttpGet]
    [SlucajKoriscenja("PretraziRezervacije", Uloge.Administrator)]
    public async Task<IActionResult> Pretrazi([FromQuery] RezervacijePretragaDto filter)
    {
        var rezultat = await _rezervacijaServis.PretraziAsync(filter);
        return Ok(rezultat);
    }
}
