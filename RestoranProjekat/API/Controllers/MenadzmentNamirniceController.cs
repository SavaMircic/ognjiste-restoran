using System.Security.Claims;
using API.Autorizacija;
using Domain.Konstante;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services;
using Services.DTO;

namespace API.Controllers;

[ApiController]
[Route("api/menadzment/namirnice")]
[Authorize]
public class MenadzmentNamirniceController : ControllerBase
{
    private readonly INamirnicaServis _namirnicaServis;

    public MenadzmentNamirniceController(INamirnicaServis namirnicaServis)
    {
        _namirnicaServis = namirnicaServis;
    }

    private string TrenutniKorisnikId => User.FindFirstValue(ClaimTypes.NameIdentifier)!;

    [HttpGet]
    [SlucajKoriscenja("ListirajNamirnice", Uloge.Menadzer, Uloge.Administrator)]
    public async Task<IActionResult> Namirnice()
    {
        var namirnice = await _namirnicaServis.ListirajAsync();
        return Ok(namirnice);
    }

    [HttpGet("lista-za-nabavku")]
    [SlucajKoriscenja("ListaZaNabavku", Uloge.Menadzer)]
    public async Task<IActionResult> ListaZaNabavku()
    {
        var lista = await _namirnicaServis.ListaZaNabavkuAsync();
        return Ok(lista);
    }

    [HttpPost]
    [SlucajKoriscenja("KreirajNamirnicu", Uloge.Menadzer)]
    public async Task<IActionResult> Kreiraj(KreirajNamirnicuDto dto)
    {
        var (uspesno, greska, namirnica) = await _namirnicaServis.KreirajAsync(dto);
        if (!uspesno) return BadRequest(new { Poruka = greska });
        return StatusCode(StatusCodes.Status201Created, namirnica);
    }

    [HttpPut("{id:int}")]
    [SlucajKoriscenja("IzmeniNamirnicu", Uloge.Menadzer)]
    public async Task<IActionResult> Izmeni(int id, IzmenaNamirniceDto dto)
    {
        var (uspesno, greska) = await _namirnicaServis.IzmeniAsync(id, dto);
        if (!uspesno) return BadRequest(new { Poruka = greska });
        return NoContent();
    }

    [HttpPatch("{id:int}/kolicina")]
    [SlucajKoriscenja("KorigujKolicinuNamirnice", Uloge.Menadzer)]
    public async Task<IActionResult> KorigujKolicinu(int id, KorekcijaKolicineDto dto)
    {
        var (uspesno, greska, namirnica) = await _namirnicaServis.KorigujKolicinuAsync(id, TrenutniKorisnikId, dto);
        if (!uspesno) return BadRequest(new { Poruka = greska });
        return Ok(namirnica);
    }
}
