using Microsoft.AspNetCore.Mvc;
using Services;

namespace API.Controllers;

[ApiController]
[Route("api/galerija")]
public class GalerijaController : ControllerBase
{
    private readonly IGalerijaServis _galerijaServis;

    public GalerijaController(IGalerijaServis galerijaServis)
    {
        _galerijaServis = galerijaServis;
    }

    [HttpGet]
    public async Task<IActionResult> Galerija() => Ok(await _galerijaServis.ListirajAktivneAsync());
}
