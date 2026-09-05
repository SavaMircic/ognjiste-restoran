using API.Autorizacija;
using Domain.Konstante;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services;
using Services.DTO;

namespace API.Controllers;

[ApiController]
[Route("api/administracija/stolovi")]
[Authorize(Roles = Uloge.Administrator)]
public class AdministracijaStoloviController : ControllerBase
{
    private readonly IStoServis _stoServis;

    public AdministracijaStoloviController(IStoServis stoServis)
    {
        _stoServis = stoServis;
    }

    [HttpPost]
    [SlucajKoriscenja("KreirajSto", Uloge.Administrator)]
    public async Task<IActionResult> KreirajSto(StoUlazDto dto)
    {
        var (uspesno, greska, sto) = await _stoServis.KreirajAsync(dto);
        if (!uspesno) return BadRequest(new { Poruka = greska });
        return StatusCode(StatusCodes.Status201Created, sto);
    }

    [HttpPut("{id:int}")]
    [SlucajKoriscenja("IzmeniSto", Uloge.Administrator)]
    public async Task<IActionResult> IzmeniSto(int id, StoUlazDto dto)
    {
        var (uspesno, greska) = await _stoServis.IzmeniAsync(id, dto);
        if (!uspesno) return BadRequest(new { Poruka = greska });
        return NoContent();
    }
}
