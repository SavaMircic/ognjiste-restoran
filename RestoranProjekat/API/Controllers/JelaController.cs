using System.Security.Claims;
using API.Autorizacija;
using Domain.Konstante;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services;

namespace API.Controllers;

[ApiController]
[Route("api/jela")]
[Authorize]
public class JelaController : ControllerBase
{
    private readonly ILajkJelaServis _lajkJelaServis;

    public JelaController(ILajkJelaServis lajkJelaServis)
    {
        _lajkJelaServis = lajkJelaServis;
    }

    private string TrenutniKorisnikId => User.FindFirstValue(ClaimTypes.NameIdentifier)!;

    [HttpPost("{id:int}/lajk")]
    [SlucajKoriscenja("LajkujJelo", Uloge.Korisnik)]
    public async Task<IActionResult> Lajkuj(int id)
    {
        var (uspesno, greska, statusKod) = await _lajkJelaServis.LajkujAsync(id, TrenutniKorisnikId);
        if (!uspesno) return StatusCode(statusKod, new { Poruka = greska });
        return StatusCode(StatusCodes.Status201Created, new { Poruka = "Jelo je lajkovano." });
    }

    [HttpDelete("{id:int}/lajk")]
    [SlucajKoriscenja("UkloniLajk", Uloge.Korisnik)]
    public async Task<IActionResult> UkloniLajk(int id)
    {
        var (uspesno, greska) = await _lajkJelaServis.UkloniLajkAsync(id, TrenutniKorisnikId);
        if (!uspesno) return BadRequest(new { Poruka = greska });
        return NoContent();
    }
}
