using System.Security.Claims;
using API.Autorizacija;
using Domain.Konstante;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services;
using Services.DTO;

namespace API.Controllers;

[ApiController]
[Route("api/prijave-problema")]
[Authorize]
public class PrijaveProblemaController : ControllerBase
{
    private readonly IPrijavaProblemaServis _prijavaServis;

    public PrijaveProblemaController(IPrijavaProblemaServis prijavaServis)
    {
        _prijavaServis = prijavaServis;
    }

    private string TrenutniKorisnikId => User.FindFirstValue(ClaimTypes.NameIdentifier)!;

    [HttpPost]
    [SlucajKoriscenja("PrijaviProblem", Uloge.Konobar, Uloge.Sanker, Uloge.Kuvar, Uloge.Menadzer, Uloge.Administrator)]
    public async Task<IActionResult> Prijavi(PrijaviProblemDto dto)
    {
        var (uspesno, greska, prijava) = await _prijavaServis.PrijaviAsync(TrenutniKorisnikId, dto);
        if (!uspesno) return BadRequest(new { Poruka = greska });
        return StatusCode(StatusCodes.Status201Created, prijava);
    }

    [HttpGet("moje")]
    [SlucajKoriscenja("MojePrijaveProblema", Uloge.Konobar, Uloge.Sanker, Uloge.Kuvar, Uloge.Menadzer, Uloge.Administrator)]
    public async Task<IActionResult> Moje()
    {
        var (uspesno, greska, prijave) = await _prijavaServis.MojePrijaveAsync(TrenutniKorisnikId);
        if (!uspesno) return BadRequest(new { Poruka = greska });
        return Ok(prijave);
    }
}
