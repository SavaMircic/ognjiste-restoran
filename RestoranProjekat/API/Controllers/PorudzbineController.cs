using System.Security.Claims;
using System.Text;
using API.Autorizacija;
using Domain.Konstante;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services;
using Services.DTO;

namespace API.Controllers;

[ApiController]
[Route("api/porudzbine")]
[Authorize]
public class PorudzbineController : ControllerBase
{
    private readonly IPorudzbinaServis _porudzbinaServis;

    public PorudzbineController(IPorudzbinaServis porudzbinaServis)
    {
        _porudzbinaServis = porudzbinaServis;
    }

    private string TrenutniKorisnikId => User.FindFirstValue(ClaimTypes.NameIdentifier)!;

    [HttpPost]
    [SlucajKoriscenja("OtvoriPorudzbinu", Uloge.Konobar)]
    public async Task<IActionResult> OtvoriPorudzbinu(OtvoriPorudzbinuDto dto)
    {
        var (uspesno, greska, id) = await _porudzbinaServis.OtvoriAsync(TrenutniKorisnikId, dto);
        if (!uspesno) return BadRequest(new { Poruka = greska });
        return StatusCode(StatusCodes.Status201Created, new { PorudzbinaId = id });
    }

    [HttpGet("aktivne")]
    [SlucajKoriscenja("ListirajAktivnePorudzbine", Uloge.Konobar)]
    public async Task<IActionResult> Aktivne()
    {
        var aktivne = await _porudzbinaServis.ListirajAktivneAsync();
        return Ok(aktivne);
    }

    [HttpGet("{id:int}")]
    [SlucajKoriscenja("DetaljPorudzbine", Uloge.Konobar, Uloge.Menadzer)]
    public async Task<IActionResult> Detalj(int id)
    {
        var detalj = await _porudzbinaServis.DobaviDetaljAsync(id);
        return detalj == null ? NotFound() : Ok(detalj);
    }

    [HttpPost("{id:int}/stavke")]
    [SlucajKoriscenja("DodajStavkeUPorudzbinu", Uloge.Konobar)]
    public async Task<IActionResult> DodajStavke(int id, DodajStavkeDto dto)
    {
        var (uspesno, greska) = await _porudzbinaServis.DodajStavkeAsync(id, dto);
        if (!uspesno) return BadRequest(new { Poruka = greska });
        return StatusCode(StatusCodes.Status201Created, new { Poruka = "Stavke su poslate." });
    }

    [HttpDelete("{id:int}/stavke/{stavkaId:int}")]
    [SlucajKoriscenja("OtkaziStavkuPorudzbine", Uloge.Konobar)]
    public async Task<IActionResult> OtkaziStavku(int id, int stavkaId)
    {
        var (uspesno, greska) = await _porudzbinaServis.OtkaziStavkuAsync(id, stavkaId);
        if (!uspesno) return BadRequest(new { Poruka = greska });
        return NoContent();
    }

    [HttpPatch("{id:int}/zatvori")]
    [SlucajKoriscenja("ZatvoriPorudzbinu", Uloge.Konobar)]
    public async Task<IActionResult> Zatvori(int id, ZatvoriPorudzbinuDto dto)
    {
        var (uspesno, greska) = await _porudzbinaServis.ZatvoriAsync(id, dto);
        if (!uspesno) return BadRequest(new { Poruka = greska });
        return NoContent();
    }

    [HttpGet("{id:int}/racun")]
    [SlucajKoriscenja("PreuzmiRacun", Uloge.Konobar)]
    public async Task<IActionResult> Racun(int id)
    {
        var (uspesno, greska, sadrzaj) = await _porudzbinaServis.DobaviRacunAsync(id);
        if (!uspesno) return BadRequest(new { Poruka = greska });
        return File(Encoding.UTF8.GetBytes(sadrzaj!), "text/plain", $"racun-{id}.txt");
    }
}
