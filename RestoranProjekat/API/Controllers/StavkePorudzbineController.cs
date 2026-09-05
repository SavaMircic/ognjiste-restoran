using System.Security.Claims;
using API.Autorizacija;
using Domain.Konstante;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services;

namespace API.Controllers;

[ApiController]
[Route("api/stavke-porudzbine")]
[Authorize]
public class StavkePorudzbineController : ControllerBase
{
    private readonly IStavkaPorudzbineServis _stavkaPorudzbineServis;

    public StavkePorudzbineController(IStavkaPorudzbineServis stavkaPorudzbineServis)
    {
        _stavkaPorudzbineServis = stavkaPorudzbineServis;
    }

    private string TrenutniKorisnikId => User.FindFirstValue(ClaimTypes.NameIdentifier)!;

    [HttpPatch("{id:int}/preuzmi")]
    [SlucajKoriscenja("PreuzmiStavku", Uloge.Kuvar, Uloge.Sanker)]
    public async Task<IActionResult> Preuzmi(int id)
    {
        var (uspesno, greska) = await _stavkaPorudzbineServis.PreuzmiAsync(id, TrenutniKorisnikId);
        if (!uspesno) return BadRequest(new { Poruka = greska });
        return NoContent();
    }

    [HttpPatch("{id:int}/zavrsi")]
    [SlucajKoriscenja("ZavrsiStavku", Uloge.Kuvar, Uloge.Sanker)]
    public async Task<IActionResult> Zavrsi(int id)
    {
        var (uspesno, greska) = await _stavkaPorudzbineServis.ZavrsiAsync(id, TrenutniKorisnikId);
        if (!uspesno) return BadRequest(new { Poruka = greska });
        return NoContent();
    }

    [HttpPatch("{id:int}/isporuci")]
    [SlucajKoriscenja("IsporuciStavku", Uloge.Konobar)]
    public async Task<IActionResult> Isporuci(int id)
    {
        var (uspesno, greska) = await _stavkaPorudzbineServis.IsporuciAsync(id);
        if (!uspesno) return BadRequest(new { Poruka = greska });
        return NoContent();
    }
}
