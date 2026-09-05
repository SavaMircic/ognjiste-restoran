using System.Security.Claims;
using API.Autorizacija;
using Domain.Enumi;
using Domain.Konstante;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services;

namespace API.Controllers;

[ApiController]
[Route("api/kuhinja")]
[Authorize]
public class KuhinjaController : ControllerBase
{
    private readonly IStavkaPorudzbineServis _stavkaPorudzbineServis;

    public KuhinjaController(IStavkaPorudzbineServis stavkaPorudzbineServis)
    {
        _stavkaPorudzbineServis = stavkaPorudzbineServis;
    }

    private string TrenutniKorisnikId => User.FindFirstValue(ClaimTypes.NameIdentifier)!;

    [HttpGet("red-cekanja")]
    [SlucajKoriscenja("KuhinjaRedCekanja", Uloge.Kuvar)]
    public async Task<IActionResult> RedCekanja()
    {
        var red = await _stavkaPorudzbineServis.RedCekanjaAsync(Odrediste.Kuhinja, TrenutniKorisnikId);
        return Ok(red);
    }
}
