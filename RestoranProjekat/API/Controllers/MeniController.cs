using Microsoft.AspNetCore.Mvc;
using Services;
using Services.DTO;

namespace API.Controllers;

[ApiController]
[Route("api/meni")]
public class MeniController : ControllerBase
{
    private readonly IKategorijaMenijaServis _kategorijaServis;
    private readonly IStavkaMenijaServis _stavkaServis;

    public MeniController(IKategorijaMenijaServis kategorijaServis, IStavkaMenijaServis stavkaServis)
    {
        _kategorijaServis = kategorijaServis;
        _stavkaServis = stavkaServis;
    }

    [HttpGet("kategorije")]
    public async Task<IActionResult> Kategorije()
    {
        var kategorije = await _kategorijaServis.ListirajAsync();
        return Ok(kategorije);
    }

    [HttpGet("stavke")]
    public async Task<IActionResult> Stavke([FromQuery] StavkeMenijaPretragaDto filter)
    {
        var rezultat = await _stavkaServis.PretraziAsync(filter);
        return Ok(rezultat);
    }

    [HttpGet("stavke/{id:int}")]
    public async Task<IActionResult> Stavka(int id)
    {
        var stavka = await _stavkaServis.DobaviDetaljAsync(id);
        return stavka == null ? NotFound() : Ok(stavka);
    }
}
