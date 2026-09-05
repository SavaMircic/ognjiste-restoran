using System.Security.Claims;
using API.Autorizacija;
using Domain.Konstante;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services;
using Services.DTO;

namespace API.Controllers;

[ApiController]
[Route("api/menadzment/bonusi")]
[Authorize(Roles = Uloge.Menadzer)]
public class MenadzmentBonusiController : ControllerBase
{
    private readonly IBonusServis _bonusServis;

    public MenadzmentBonusiController(IBonusServis bonusServis)
    {
        _bonusServis = bonusServis;
    }

    private string TrenutniKorisnikId => User.FindFirstValue(ClaimTypes.NameIdentifier)!;

    [HttpGet]
    [SlucajKoriscenja("PretraziBonuse", Uloge.Menadzer)]
    public async Task<IActionResult> Pretrazi([FromQuery] BonusiPretragaDto filter)
    {
        var bonusi = await _bonusServis.PretraziAsync(filter);
        return Ok(bonusi);
    }

    [HttpPost]
    [SlucajKoriscenja("DodeliBonus", Uloge.Menadzer)]
    public async Task<IActionResult> Dodeli(KreirajBonusDto dto)
    {
        var (uspesno, greska, bonusi) = await _bonusServis.DodeliAsync(TrenutniKorisnikId, dto);
        if (!uspesno) return BadRequest(new { Poruka = greska });
        return StatusCode(StatusCodes.Status201Created, bonusi);
    }
}
