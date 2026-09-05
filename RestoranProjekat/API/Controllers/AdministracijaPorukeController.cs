using System.Security.Claims;
using API.Autorizacija;
using Domain.Konstante;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services;
using Services.DTO;

namespace API.Controllers;

[ApiController]
[Route("api/administracija/poruke")]
[Authorize(Roles = Uloge.Administrator)]
public class AdministracijaPorukeController : ControllerBase
{
    private readonly IPorukaServis _porukaServis;

    public AdministracijaPorukeController(IPorukaServis porukaServis)
    {
        _porukaServis = porukaServis;
    }

    private string TrenutniKorisnikId => User.FindFirstValue(ClaimTypes.NameIdentifier)!;

    [HttpGet]
    [SlucajKoriscenja("PretraziPoruke", Uloge.Administrator)]
    public async Task<IActionResult> Pretrazi([FromQuery] PorukePretragaDto filter)
    {
        var rezultat = await _porukaServis.PretraziAsync(filter);
        return Ok(rezultat);
    }

    [HttpPut("{id:int}/odgovor")]
    [SlucajKoriscenja("OdgovoriNaPoruku", Uloge.Administrator)]
    public async Task<IActionResult> Odgovori(int id, OdgovorNaPorukuDto dto)
    {
        var (uspesno, greska) = await _porukaServis.OdgovoriAsync(id, TrenutniKorisnikId, dto);
        if (!uspesno) return BadRequest(new { Poruka = greska });
        return NoContent();
    }

    [HttpPatch("{id:int}/procitano")]
    [SlucajKoriscenja("OznaciPorukuProcitanom", Uloge.Administrator)]
    public async Task<IActionResult> OznaciProcitanom(int id)
    {
        var (uspesno, greska) = await _porukaServis.OznaciProcitanomAsync(id);
        if (!uspesno) return BadRequest(new { Poruka = greska });
        return NoContent();
    }
}
