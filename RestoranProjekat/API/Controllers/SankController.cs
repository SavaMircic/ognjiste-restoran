using System.Security.Claims;
using API.Autorizacija;
using Domain.Enumi;
using Domain.Konstante;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services;

namespace API.Controllers;

[ApiController]
[Route("api/sank")]
[Authorize]
public class SankController : ControllerBase
{
    private readonly IStavkaPorudzbineServis _stavkaPorudzbineServis;

    public SankController(IStavkaPorudzbineServis stavkaPorudzbineServis)
    {
        _stavkaPorudzbineServis = stavkaPorudzbineServis;
    }

    private string TrenutniKorisnikId => User.FindFirstValue(ClaimTypes.NameIdentifier)!;

    [HttpGet("red-cekanja")]
    [SlucajKoriscenja("SankRedCekanja", Uloge.Sanker)]
    public async Task<IActionResult> RedCekanja()
    {
        var red = await _stavkaPorudzbineServis.RedCekanjaAsync(Odrediste.Sank, TrenutniKorisnikId);
        return Ok(red);
    }
}
