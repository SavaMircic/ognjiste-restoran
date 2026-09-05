using System.Security.Claims;
using API.Autorizacija;
using Domain.Konstante;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services;
using Services.DTO;

namespace API.Controllers;

[ApiController]
[Route("api/korisnici")]
[Authorize]
public class KorisniciController : ControllerBase
{
    private readonly IKorisnikServis _korisnikServis;
    private readonly ILajkJelaServis _lajkJelaServis;
    private readonly IPorukaServis _porukaServis;
    private readonly IBonusServis _bonusServis;
    private readonly IIzvestajServis _izvestajServis;

    public KorisniciController(
        IKorisnikServis korisnikServis, ILajkJelaServis lajkJelaServis,
        IPorukaServis porukaServis, IBonusServis bonusServis, IIzvestajServis izvestajServis)
    {
        _korisnikServis = korisnikServis;
        _lajkJelaServis = lajkJelaServis;
        _porukaServis = porukaServis;
        _bonusServis = bonusServis;
        _izvestajServis = izvestajServis;
    }

    private string TrenutniKorisnikId => User.FindFirstValue(ClaimTypes.NameIdentifier)!;

    [HttpGet("profil")]
    public async Task<IActionResult> Profil()
    {
        var profil = await _korisnikServis.DobaviProfilAsync(TrenutniKorisnikId);
        return profil == null ? NotFound() : Ok(profil);
    }

    [HttpPut("profil")]
    [SlucajKoriscenja("IzmenaProfila")]
    public async Task<IActionResult> IzmeniProfil(IzmenaProfilaDto dto)
    {
        var (uspesno, greska) = await _korisnikServis.IzmeniProfilAsync(TrenutniKorisnikId, dto);
        if (!uspesno) return BadRequest(new { Poruka = greska });
        return NoContent();
    }

    [HttpPut("profil/slika")]
    [RequestSizeLimit(3 * 1024 * 1024)]
    [SlucajKoriscenja("PostaviProfilnuSliku")]
    public async Task<IActionResult> PostaviProfilnuSliku([FromForm] API.Ulaz.UploadSlikeDto ulaz)
    {
        var datoteka = new DatotekaZaUploadDto
        {
            Sadrzaj = ulaz.Slika!.OpenReadStream(),
            ImeDatoteke = ulaz.Slika.FileName,
            TipSadrzaja = ulaz.Slika.ContentType,
            Velicina = ulaz.Slika.Length
        };

        var (uspesno, greska, url) = await _korisnikServis.PostaviProfilnuSlikuAsync(TrenutniKorisnikId, datoteka);
        if (!uspesno) return BadRequest(new { Poruka = greska });
        return Ok(new { SlikaUrl = url });
    }

    [HttpPut("promeni-lozinku")]
    [SlucajKoriscenja("PromenaLozinke")]
    public async Task<IActionResult> PromeniLozinku(PromenaLozinkeDto dto)
    {
        var (uspesno, greska) = await _korisnikServis.PromeniLozinkuAsync(TrenutniKorisnikId, dto);
        if (!uspesno) return BadRequest(new { Poruka = greska });
        return NoContent();
    }

    [HttpGet("omiljena-jela")]
    [SlucajKoriscenja("OmiljenaJela", Uloge.Korisnik)]
    public async Task<IActionResult> OmiljenaJela()
    {
        var jela = await _lajkJelaServis.OmiljenaJelaAsync(TrenutniKorisnikId);
        return Ok(jela);
    }

    [HttpGet("poruke")]
    [SlucajKoriscenja("MojePoruke", Uloge.Korisnik)]
    public async Task<IActionResult> MojePoruke()
    {
        var poruke = await _porukaServis.MojePorukeAsync(TrenutniKorisnikId);
        return Ok(poruke);
    }

    [HttpGet("statistika")]
    [SlucajKoriscenja("MojaStatistika", Uloge.Konobar, Uloge.Sanker, Uloge.Kuvar, Uloge.Menadzer, Uloge.Administrator)]
    public async Task<IActionResult> MojaStatistika([FromQuery] MojaStatistikaUpitDto upit)
    {
        var (uspesno, greska, statistika) = await _izvestajServis.MojaStatistikaAsync(TrenutniKorisnikId, upit);
        if (!uspesno) return BadRequest(new { Poruka = greska });
        return Ok(statistika);
    }

    [HttpGet("istorija")]
    [SlucajKoriscenja("MojaIstorijaRada", Uloge.Konobar, Uloge.Sanker, Uloge.Kuvar)]
    public async Task<IActionResult> MojaIstorija([FromQuery] IstorijaRadaUpitDto upit)
    {
        var (uspesno, greska, dani) = await _izvestajServis.MojaIstorijaAsync(TrenutniKorisnikId, upit);
        if (!uspesno) return BadRequest(new { Poruka = greska });
        return Ok(dani);
    }

    [HttpGet("bonusi")]
    [SlucajKoriscenja("MojiBonusi", Uloge.Konobar, Uloge.Sanker, Uloge.Kuvar, Uloge.Menadzer, Uloge.Administrator)]
    public async Task<IActionResult> MojiBonusi()
    {
        var (uspesno, greska, bonusi) = await _bonusServis.MojiBonusiAsync(TrenutniKorisnikId);
        if (!uspesno) return BadRequest(new { Poruka = greska });
        return Ok(bonusi);
    }
}
