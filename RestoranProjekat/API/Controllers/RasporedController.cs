using System.Security.Claims;
using API.Autorizacija;
using Domain.Konstante;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services;

namespace API.Controllers;

[ApiController]
[Route("api/raspored")]
[Authorize]
public class RasporedController : ControllerBase
{
    private readonly ISmenaServis _smenaServis;

    public RasporedController(ISmenaServis smenaServis)
    {
        _smenaServis = smenaServis;
    }

    private string TrenutniKorisnikId => User.FindFirstValue(ClaimTypes.NameIdentifier)!;

    [HttpGet("moj")]
    [SlucajKoriscenja("MojRaspored", Uloge.Konobar, Uloge.Sanker, Uloge.Kuvar, Uloge.Menadzer, Uloge.Administrator)]
    public async Task<IActionResult> MojRaspored([FromQuery] DateOnly? nedelja)
    {
        var (uspesno, greska, smene) = await _smenaServis.MojRasporedAsync(TrenutniKorisnikId, nedelja);
        if (!uspesno) return BadRequest(new { Poruka = greska });
        return Ok(smene);
    }
}
