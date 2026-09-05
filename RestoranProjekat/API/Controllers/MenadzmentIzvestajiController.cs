using API.Autorizacija;
using Domain.Konstante;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services;
using Services.DTO;

namespace API.Controllers;

[ApiController]
[Route("api/menadzment/izvestaji")]
[Authorize(Roles = Uloge.Menadzer)]
public class MenadzmentIzvestajiController : ControllerBase
{
    private readonly IIzvestajServis _izvestajServis;

    public MenadzmentIzvestajiController(IIzvestajServis izvestajServis)
    {
        _izvestajServis = izvestajServis;
    }

    [HttpGet("prihod")]
    [SlucajKoriscenja("IzvestajPrihod", Uloge.Menadzer)]
    public async Task<IActionResult> Prihod([FromQuery] PrihodUpitDto upit)
    {
        var izvestaj = await _izvestajServis.PrihodAsync(upit);
        return Ok(izvestaj);
    }

    [HttpGet("ucinak-zaposlenih")]
    [SlucajKoriscenja("IzvestajUcinakZaposlenih", Uloge.Menadzer)]
    public async Task<IActionResult> UcinakZaposlenih([FromQuery] UcinakUpitDto upit)
    {
        var izvestaj = await _izvestajServis.UcinakZaposlenihAsync(upit);
        return Ok(izvestaj);
    }

    [HttpGet("top-jela")]
    [SlucajKoriscenja("IzvestajTopJela", Uloge.Menadzer)]
    public async Task<IActionResult> TopJela([FromQuery] TopJelaUpitDto upit)
    {
        var izvestaj = await _izvestajServis.TopJelaAsync(upit);
        return Ok(izvestaj);
    }
}
