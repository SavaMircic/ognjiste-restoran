using Microsoft.AspNetCore.Mvc;
using Services;

namespace API.Controllers;

[ApiController]
[Route("api/nas-tim")]
public class NasTimController : ControllerBase
{
    private readonly IZaposleniServis _zaposleniServis;

    public NasTimController(IZaposleniServis zaposleniServis)
    {
        _zaposleniServis = zaposleniServis;
    }

    [HttpGet]
    public async Task<IActionResult> Tim() => Ok(await _zaposleniServis.ListirajTimAsync());
}
