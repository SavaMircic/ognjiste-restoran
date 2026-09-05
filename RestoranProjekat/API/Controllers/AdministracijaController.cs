using System.Security.Claims;
using API.Autorizacija;
using Domain.Konstante;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services;
using Services.DTO;

namespace API.Controllers;

[ApiController]
[Route("api/administracija")]
[Authorize]
public class AdministracijaController : ControllerBase
{
    private readonly IAuditLogServis _auditLogServis;
    private readonly IKorisnikAdminServis _korisnikAdminServis;
    private readonly IZaposleniServis _zaposleniServis;
    private readonly IPostavkeServis _postavkeServis;
    private readonly IAdminAkcijaServis _adminAkcijaServis;

    public AdministracijaController(
        IAuditLogServis auditLogServis, IKorisnikAdminServis korisnikAdminServis,
        IZaposleniServis zaposleniServis, IPostavkeServis postavkeServis,
        IAdminAkcijaServis adminAkcijaServis)
    {
        _auditLogServis = auditLogServis;
        _korisnikAdminServis = korisnikAdminServis;
        _zaposleniServis = zaposleniServis;
        _postavkeServis = postavkeServis;
        _adminAkcijaServis = adminAkcijaServis;
    }

    private string TrenutniKorisnikId => User.FindFirstValue(ClaimTypes.NameIdentifier)!;

    [HttpGet("audit-log")]
    [SlucajKoriscenja("PretraziAuditLog", Uloge.Administrator)]
    public async Task<IActionResult> PretraziAuditLog([FromQuery] AuditLogPretragaDto filter)
    {
        var rezultat = await _auditLogServis.PretraziAsync(filter);
        return Ok(rezultat);
    }

    [HttpGet("admin-akcije")]
    [SlucajKoriscenja("PretraziAdminAkcije", Uloge.Administrator)]
    public async Task<IActionResult> PretraziAdminAkcije([FromQuery] AdminAkcijePretragaDto filter) =>
        Ok(await _adminAkcijaServis.PretraziAsync(filter));

    [HttpGet("korisnici")]
    [SlucajKoriscenja("PretraziKorisnike", Uloge.Administrator)]
    public async Task<IActionResult> PretraziKorisnike([FromQuery] KorisniciPretragaDto filter)
    {
        var rezultat = await _korisnikAdminServis.PretraziAsync(filter);
        return Ok(rezultat);
    }

    [HttpGet("korisnici/{id}")]
    [SlucajKoriscenja("DetaljKorisnika", Uloge.Administrator)]
    public async Task<IActionResult> DetaljKorisnika(string id)
    {
        var detalj = await _korisnikAdminServis.DobaviDetaljAsync(id);
        return detalj == null ? NotFound() : Ok(detalj);
    }

    [HttpPut("korisnici/{id}/blokiraj")]
    [SlucajKoriscenja("BlokirajKorisnika", Uloge.Administrator)]
    public async Task<IActionResult> BlokirajKorisnika(string id, BlokirajKorisnikaDto dto)
    {
        var (uspesno, greska) = await _korisnikAdminServis.BlokirajAsync(id, TrenutniKorisnikId, dto);
        if (!uspesno) return BadRequest(new { Poruka = greska });
        return Ok(new { Poruka = "Korisnik je blokiran." });
    }

    [HttpPut("korisnici/{id}/odblokiraj")]
    [SlucajKoriscenja("OdblokirajKorisnika", Uloge.Administrator)]
    public async Task<IActionResult> OdblokirajKorisnika(string id)
    {
        var (uspesno, greska) = await _korisnikAdminServis.OdblokirajAsync(id, TrenutniKorisnikId);
        if (!uspesno) return BadRequest(new { Poruka = greska });
        return Ok(new { Poruka = "Blokada je ukinuta." });
    }

    [HttpPut("korisnici/{id}/zabrani-komentarisanje")]
    [SlucajKoriscenja("ZabraniKomentarisanje", Uloge.Administrator)]
    public async Task<IActionResult> ZabraniKomentarisanje(string id, ZabraniKomentarisanjeDto dto)
    {
        var (uspesno, greska) = await _korisnikAdminServis.ZabraniKomentarisanjeAsync(id, TrenutniKorisnikId, dto);
        if (!uspesno) return BadRequest(new { Poruka = greska });
        return Ok(new { Poruka = "Korisniku je privremeno zabranjeno komentarisanje." });
    }

    [HttpPut("korisnici/{id}/ukini-zabranu-komentarisanja")]
    [SlucajKoriscenja("UkiniZabranuKomentarisanja", Uloge.Administrator)]
    public async Task<IActionResult> UkiniZabranuKomentarisanja(string id)
    {
        var (uspesno, greska) = await _korisnikAdminServis.UkiniZabranuKomentarisanjaAsync(id, TrenutniKorisnikId);
        if (!uspesno) return BadRequest(new { Poruka = greska });
        return Ok(new { Poruka = "Zabrana komentarisanja je ukinuta." });
    }

    [HttpPost("zaposleni")]
    [SlucajKoriscenja("KreirajZaposlenog", Uloge.Administrator)]
    public async Task<IActionResult> KreirajZaposlenog(KreirajZaposlenogDto dto)
    {
        var (uspesno, greska, zaposleniId) = await _zaposleniServis.KreirajAsync(dto);
        if (!uspesno) return BadRequest(new { Poruka = greska });
        return StatusCode(StatusCodes.Status201Created, new { ZaposleniId = zaposleniId });
    }

    [HttpGet("zaposleni")]
    [SlucajKoriscenja("PretraziZaposlene", Uloge.Administrator, Uloge.Menadzer)]
    public async Task<IActionResult> PretraziZaposlene([FromQuery] ZaposleniPretragaDto filter)
    {
        var rezultat = await _zaposleniServis.PretraziAsync(filter);
        return Ok(rezultat);
    }

    [HttpPut("zaposleni/{id:int}")]
    [SlucajKoriscenja("IzmeniZaposlenog", Uloge.Administrator)]
    public async Task<IActionResult> IzmeniZaposlenog(int id, IzmenaZaposlenogDto dto)
    {
        var (uspesno, greska) = await _zaposleniServis.IzmeniAsync(id, dto);
        if (!uspesno) return BadRequest(new { Poruka = greska });
        return NoContent();
    }

    [HttpDelete("zaposleni/{id:int}")]
    [SlucajKoriscenja("DeaktivirajZaposlenog", Uloge.Administrator)]
    public async Task<IActionResult> DeaktivirajZaposlenog(int id)
    {
        var (uspesno, greska) = await _zaposleniServis.DeaktivirajAsync(id);
        if (!uspesno) return BadRequest(new { Poruka = greska });
        return NoContent();
    }

    [HttpPut("zaposleni/{id:int}/profil-sajta")]
    [SlucajKoriscenja("IzmeniProfilNaSajtu", Uloge.Administrator)]
    public async Task<IActionResult> IzmeniProfilNaSajtu(int id, IzmenaProfilaNaSajtuDto dto)
    {
        var (uspesno, greska) = await _zaposleniServis.IzmeniProfilNaSajtuAsync(id, dto);
        if (!uspesno) return BadRequest(new { Poruka = greska });
        return NoContent();
    }

    [HttpPut("zaposleni/{id:int}/slika")]
    [RequestSizeLimit(3 * 1024 * 1024)]
    [SlucajKoriscenja("PostaviSlikuZaposlenog", Uloge.Administrator)]
    public async Task<IActionResult> PostaviSlikuZaposlenog(int id, [FromForm] API.Ulaz.UploadSlikeDto ulaz)
    {
        var datoteka = new DatotekaZaUploadDto
        {
            Sadrzaj = ulaz.Slika!.OpenReadStream(),
            ImeDatoteke = ulaz.Slika.FileName,
            TipSadrzaja = ulaz.Slika.ContentType,
            Velicina = ulaz.Slika.Length
        };

        var (uspesno, greska, url) = await _zaposleniServis.PostaviSlikuAsync(id, datoteka);
        if (!uspesno) return BadRequest(new { Poruka = greska });
        return Ok(new { SlikaUrl = url });
    }

    [HttpPut("postavke")]
    [SlucajKoriscenja("IzmeniPostavke", Uloge.Administrator)]
    public async Task<IActionResult> IzmeniPostavke(IzmenaPostavkiDto dto)
    {
        var postavke = await _postavkeServis.IzmeniAsync(dto);
        return Ok(postavke);
    }
}
