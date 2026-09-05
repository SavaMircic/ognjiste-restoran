using Microsoft.AspNetCore.Mvc;
using Services;

namespace API.Controllers;

[ApiController]
[Route("api/postavke")]
public class PostavkeController : ControllerBase
{
    private readonly IPostavkeServis _postavkeServis;

    public PostavkeController(IPostavkeServis postavkeServis)
    {
        _postavkeServis = postavkeServis;
    }

    [HttpGet]
    public async Task<IActionResult> Postavke()
    {
        var postavke = await _postavkeServis.DobaviAsync();
        return Ok(postavke);
    }
}
