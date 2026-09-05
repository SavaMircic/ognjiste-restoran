using System.Security.Claims;
using API.Autorizacija;
using Domain.Konstante;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services;
using Services.DTO;

namespace API.Controllers;

[ApiController]
[Route("api/administracija/recenzije")]
[Authorize(Roles = Uloge.Administrator)]
public class AdministracijaRecenzijeController : ControllerBase
{
    private readonly IRecenzijaServis _recenzijaServis;

    public AdministracijaRecenzijeController(IRecenzijaServis recenzijaServis)
    {
        _recenzijaServis = recenzijaServis;
    }

    private string TrenutniKorisnikId => User.FindFirstValue(ClaimTypes.NameIdentifier)!;

    [HttpPut("{id:int}/odgovor")]
    [SlucajKoriscenja("OdgovoriNaRecenziju", Uloge.Administrator)]
    public async Task<IActionResult> Odgovori(int id, OdgovorRecenzijeDto dto)
    {
        var (uspesno, greska) = await _recenzijaServis.OdgovoriAsync(id, dto);
        if (!uspesno) return BadRequest(new { Poruka = greska });
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    [SlucajKoriscenja("ObrisiRecenzijuAdmin", Uloge.Administrator)]
    public async Task<IActionResult> Obrisi(int id, [FromQuery] string? razlog)
    {
        if (string.IsNullOrWhiteSpace(razlog))
            return BadRequest(new { Poruka = "Razlog uklanjanja je obavezan.", Greske = new Dictionary<string, string[]>() });

        var (uspesno, greska) = await _recenzijaServis.ObrisiAdminAsync(id, TrenutniKorisnikId, razlog);
        if (!uspesno) return BadRequest(new { Poruka = greska });
        return NoContent();
    }
}
