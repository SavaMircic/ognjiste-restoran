using System.Security.Claims;
using API.Autorizacija;
using Domain.Konstante;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services;
using Services.DTO;

namespace API.Controllers;

[ApiController]
[Route("api/recenzije")]
[Authorize]
public class RecenzijeController : ControllerBase
{
    private readonly IRecenzijaServis _recenzijaServis;

    public RecenzijeController(IRecenzijaServis recenzijaServis)
    {
        _recenzijaServis = recenzijaServis;
    }

    private string TrenutniKorisnikId => User.FindFirstValue(ClaimTypes.NameIdentifier)!;

    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> Pretrazi([FromQuery] RecenzijePretragaDto filter)
    {
        var rezultat = await _recenzijaServis.PretraziAsync(filter);
        return Ok(rezultat);
    }

    [HttpPost]
    [SlucajKoriscenja("KreirajRecenziju", Uloge.Korisnik)]
    public async Task<IActionResult> Kreiraj(KreirajRecenzijuDto dto)
    {
        var (uspesno, greska, statusKod, recenzija) = await _recenzijaServis.KreirajAsync(TrenutniKorisnikId, dto);
        if (!uspesno) return StatusCode(statusKod, new { Poruka = greska });
        return StatusCode(StatusCodes.Status201Created, recenzija);
    }

    [HttpPut("{id:int}")]
    [SlucajKoriscenja("IzmeniRecenziju", Uloge.Korisnik)]
    public async Task<IActionResult> Izmeni(int id, IzmenaRecenzijeDto dto)
    {
        var (uspesno, greska) = await _recenzijaServis.IzmeniAsync(id, TrenutniKorisnikId, dto);
        if (!uspesno) return BadRequest(new { Poruka = greska });
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    [SlucajKoriscenja("ObrisiRecenziju", Uloge.Korisnik)]
    public async Task<IActionResult> Obrisi(int id)
    {
        var (uspesno, greska) = await _recenzijaServis.ObrisiAsync(id, TrenutniKorisnikId);
        if (!uspesno) return BadRequest(new { Poruka = greska });
        return NoContent();
    }
}
