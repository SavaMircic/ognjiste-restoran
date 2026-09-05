using System.Security.Claims;
using API.Autorizacija;
using Domain.Konstante;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services;
using Services.DTO;

namespace API.Controllers;

[ApiController]
[Route("api/menadzment/prijave-problema")]
[Authorize(Roles = Uloge.Menadzer)]
public class MenadzmentPrijaveProblemaController : ControllerBase
{
    private readonly IPrijavaProblemaServis _prijavaServis;

    public MenadzmentPrijaveProblemaController(IPrijavaProblemaServis prijavaServis)
    {
        _prijavaServis = prijavaServis;
    }

    private string TrenutniKorisnikId => User.FindFirstValue(ClaimTypes.NameIdentifier)!;

    [HttpGet]
    [SlucajKoriscenja("PretraziPrijaveProblema", Uloge.Menadzer)]
    public async Task<IActionResult> Pretrazi([FromQuery] PrijaveProblemaPretragaDto filter) =>
        Ok(await _prijavaServis.PretraziAsync(filter));

    [HttpPatch("{id:int}/resi")]
    [SlucajKoriscenja("ResiPrijavuProblema", Uloge.Menadzer)]
    public async Task<IActionResult> Resi(int id, ResiPrijavuProblemaDto dto)
    {
        var (uspesno, greska) = await _prijavaServis.ResiAsync(id, TrenutniKorisnikId, dto);
        if (!uspesno) return BadRequest(new { Poruka = greska });
        return NoContent();
    }
}
