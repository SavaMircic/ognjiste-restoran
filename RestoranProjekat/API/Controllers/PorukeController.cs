using System.Security.Claims;
using API.Autorizacija;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services;
using Services.DTO;

namespace API.Controllers;

[ApiController]
[Route("api/poruke")]
public class PorukeController : ControllerBase
{
    private readonly IPorukaServis _porukaServis;

    public PorukeController(IPorukaServis porukaServis)
    {
        _porukaServis = porukaServis;
    }

    [HttpPost]
    [AllowAnonymous]
    [SlucajKoriscenja("PosaljiPoruku")]
    public async Task<IActionResult> Posalji(PosaljiPorukuDto dto)
    {
        var korisnikId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        var (uspesno, greska, poruka) = await _porukaServis.PosaljiAsync(korisnikId, dto);
        if (!uspesno) return BadRequest(new { Poruka = greska });
        return StatusCode(StatusCodes.Status201Created, poruka);
    }
}
