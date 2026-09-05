using API.Autorizacija;
using Domain.Konstante;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services;

namespace API.Controllers;

[ApiController]
[Route("api/stolovi")]
[Authorize]
public class StoloviController : ControllerBase
{
    private readonly IStoServis _stoServis;

    public StoloviController(IStoServis stoServis)
    {
        _stoServis = stoServis;
    }

    [HttpGet]
    [SlucajKoriscenja("ListirajStolove", Uloge.Konobar, Uloge.Menadzer, Uloge.Administrator)]
    public async Task<IActionResult> Stolovi()
    {
        var stolovi = await _stoServis.ListirajAsync();
        return Ok(stolovi);
    }
}
