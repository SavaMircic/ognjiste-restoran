using API.Autorizacija;
using Domain.Konstante;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services;
using Services.DTO;

namespace API.Controllers;

[ApiController]
[Route("api/menadzment/raspored")]
[Authorize(Roles = Uloge.Menadzer)]
public class MenadzmentRasporedController : ControllerBase
{
    private readonly ISmenaServis _smenaServis;

    public MenadzmentRasporedController(ISmenaServis smenaServis)
    {
        _smenaServis = smenaServis;
    }

    [HttpGet]
    [SlucajKoriscenja("RasporedNedelje", Uloge.Menadzer)]
    public async Task<IActionResult> Raspored([FromQuery] DateOnly? nedelja)
    {
        var raspored = await _smenaServis.RasporedNedeljeAsync(nedelja);
        return Ok(raspored);
    }

    [HttpPost]
    [SlucajKoriscenja("KreirajSmenu", Uloge.Menadzer)]
    public async Task<IActionResult> Kreiraj(KreirajSmenuDto dto)
    {
        var (uspesno, greska, smena) = await _smenaServis.KreirajAsync(dto);
        if (!uspesno) return BadRequest(new { Poruka = greska });
        return StatusCode(StatusCodes.Status201Created, smena);
    }

    [HttpPut("{id:int}")]
    [SlucajKoriscenja("IzmeniSmenu", Uloge.Menadzer)]
    public async Task<IActionResult> Izmeni(int id, IzmenaSmeneDto dto)
    {
        var (uspesno, greska) = await _smenaServis.IzmeniAsync(id, dto);
        if (!uspesno) return BadRequest(new { Poruka = greska });
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    [SlucajKoriscenja("ObrisiSmenu", Uloge.Menadzer)]
    public async Task<IActionResult> Obrisi(int id)
    {
        var (uspesno, greska) = await _smenaServis.ObrisiAsync(id);
        if (!uspesno) return BadRequest(new { Poruka = greska });
        return NoContent();
    }
}
