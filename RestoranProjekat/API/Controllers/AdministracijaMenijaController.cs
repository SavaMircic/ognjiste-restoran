using API.Autorizacija;
using API.Ulaz;
using Domain.Konstante;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services;
using Services.DTO;

namespace API.Controllers;

[ApiController]
[Route("api/administracija/meni")]
[Authorize(Roles = Uloge.Administrator)]
public class AdministracijaMenijaController : ControllerBase
{
    private readonly IKategorijaMenijaServis _kategorijaServis;
    private readonly IStavkaMenijaServis _stavkaServis;
    private readonly IReceptureServis _receptureServis;

    public AdministracijaMenijaController(
        IKategorijaMenijaServis kategorijaServis, IStavkaMenijaServis stavkaServis, IReceptureServis receptureServis)
    {
        _kategorijaServis = kategorijaServis;
        _stavkaServis = stavkaServis;
        _receptureServis = receptureServis;
    }

    [HttpPut("stavke/{id:int}/slika")]
    [RequestSizeLimit(3 * 1024 * 1024)]
    [SlucajKoriscenja("PostaviSlikuStavkeMenija", Uloge.Administrator)]
    public async Task<IActionResult> PostaviGlavnuSliku(int id, [FromForm] UploadSlikeDto ulaz)
    {
        var (uspesno, greska, url) = await _stavkaServis.PostaviGlavnuSlikuAsync(id, UcitajDatoteku(ulaz));
        if (!uspesno) return BadRequest(new { Poruka = greska });
        return Ok(new { SlikaUrl = url });
    }

    [HttpPost("stavke/{id:int}/slike")]
    [RequestSizeLimit(3 * 1024 * 1024)]
    [SlucajKoriscenja("DodajSlikuStavkeMenija", Uloge.Administrator)]
    public async Task<IActionResult> DodajDodatnuSliku(int id, [FromForm] UploadSlikeDto ulaz)
    {
        var (uspesno, greska, slika) = await _stavkaServis.DodajDodatnuSlikuAsync(id, UcitajDatoteku(ulaz), ulaz.Opis);
        if (!uspesno) return BadRequest(new { Poruka = greska });
        return StatusCode(StatusCodes.Status201Created, slika);
    }

    [HttpGet("stavke/{id:int}/slike")]
    [SlucajKoriscenja("ListirajSlikeStavkeMenija", Uloge.Administrator)]
    public async Task<IActionResult> ListirajDodatneSlike(int id) =>
        Ok(await _stavkaServis.ListirajDodatneSlikeAsync(id));

    [HttpDelete("stavke/{id:int}/slike/{slikaId:int}")]
    [SlucajKoriscenja("ObrisiSlikuStavkeMenija", Uloge.Administrator)]
    public async Task<IActionResult> ObrisiDodatnuSliku(int id, int slikaId)
    {
        var (uspesno, greska) = await _stavkaServis.ObrisiDodatnuSlikuAsync(id, slikaId);
        if (!uspesno) return BadRequest(new { Poruka = greska });
        return NoContent();
    }

    [HttpPut("stavke/{id:int}/slike/redosled")]
    [SlucajKoriscenja("PoredjajSlikeStavkeMenija", Uloge.Administrator)]
    public async Task<IActionResult> PoredjajDodatneSlike(int id, RedosledSlikaDto dto)
    {
        var (uspesno, greska) = await _stavkaServis.PoredjajDodatneSlikeAsync(id, dto);
        if (!uspesno) return BadRequest(new { Poruka = greska });
        return NoContent();
    }

    private static DatotekaZaUploadDto UcitajDatoteku(UploadSlikeDto ulaz) => new()
    {
        Sadrzaj = ulaz.Slika!.OpenReadStream(),
        ImeDatoteke = ulaz.Slika.FileName,
        TipSadrzaja = ulaz.Slika.ContentType,
        Velicina = ulaz.Slika.Length
    };

    [HttpPost("kategorije")]
    [SlucajKoriscenja("KreirajKategorijuMenija", Uloge.Administrator)]
    public async Task<IActionResult> KreirajKategoriju(KategorijaMenijaUlazDto dto)
    {
        var kategorija = await _kategorijaServis.KreirajAsync(dto);
        return StatusCode(StatusCodes.Status201Created, kategorija);
    }

    [HttpPut("kategorije/{id:int}")]
    [SlucajKoriscenja("IzmeniKategorijuMenija", Uloge.Administrator)]
    public async Task<IActionResult> IzmeniKategoriju(int id, KategorijaMenijaUlazDto dto)
    {
        var (uspesno, greska) = await _kategorijaServis.IzmeniAsync(id, dto);
        if (!uspesno) return BadRequest(new { Poruka = greska });
        return NoContent();
    }

    [HttpDelete("kategorije/{id:int}")]
    [SlucajKoriscenja("ObrisiKategorijuMenija", Uloge.Administrator)]
    public async Task<IActionResult> ObrisiKategoriju(int id)
    {
        var (uspesno, greska) = await _kategorijaServis.ObrisiAsync(id);
        if (!uspesno) return BadRequest(new { Poruka = greska });
        return NoContent();
    }

    [HttpPost("stavke")]
    [SlucajKoriscenja("KreirajStavkuMenija", Uloge.Administrator)]
    public async Task<IActionResult> KreirajStavku(StavkaMenijaUlazDto dto)
    {
        var (uspesno, greska, id) = await _stavkaServis.KreirajAsync(dto);
        if (!uspesno) return BadRequest(new { Poruka = greska });
        return StatusCode(StatusCodes.Status201Created, new { Id = id });
    }

    [HttpPut("stavke/{id:int}")]
    [SlucajKoriscenja("IzmeniStavkuMenija", Uloge.Administrator)]
    public async Task<IActionResult> IzmeniStavku(int id, StavkaMenijaUlazDto dto)
    {
        var (uspesno, greska) = await _stavkaServis.IzmeniAsync(id, dto);
        if (!uspesno) return BadRequest(new { Poruka = greska });
        return NoContent();
    }

    [HttpPatch("stavke/{id:int}/dostupnost")]
    [SlucajKoriscenja("PromeniDostupnostStavke", Uloge.Administrator)]
    public async Task<IActionResult> PromeniDostupnost(int id, PromenaDostupnostiDto dto)
    {
        var (uspesno, greska) = await _stavkaServis.PromeniDostupnostAsync(id, dto);
        if (!uspesno) return BadRequest(new { Poruka = greska });
        return NoContent();
    }

    [HttpDelete("stavke/{id:int}")]
    [SlucajKoriscenja("ObrisiStavkuMenija", Uloge.Administrator)]
    public async Task<IActionResult> ObrisiStavku(int id)
    {
        var (uspesno, greska) = await _stavkaServis.ObrisiAsync(id);
        if (!uspesno) return BadRequest(new { Poruka = greska });
        return NoContent();
    }

    [HttpGet("stavke/{id:int}/receptura")]
    [SlucajKoriscenja("PretraziRecepturu", Uloge.Administrator)]
    public async Task<IActionResult> Receptura(int id)
    {
        var receptura = await _receptureServis.ListirajAsync(id);
        return Ok(receptura);
    }

    [HttpPost("stavke/{id:int}/receptura")]
    [SlucajKoriscenja("DodajURecepturu", Uloge.Administrator)]
    public async Task<IActionResult> DodajURecepturu(int id, DodajURecepturuDto dto)
    {
        var (uspesno, greska) = await _receptureServis.DodajAsync(id, dto);
        if (!uspesno) return BadRequest(new { Poruka = greska });
        return StatusCode(StatusCodes.Status201Created, new { Poruka = "Namirnica je dodata u recepturu." });
    }

    [HttpPut("stavke/{id:int}/receptura/{namirnicaId:int}")]
    [SlucajKoriscenja("IzmeniKolicinuURecepturi", Uloge.Administrator)]
    public async Task<IActionResult> IzmeniKolicinu(int id, int namirnicaId, IzmenaKolicineDto dto)
    {
        var (uspesno, greska) = await _receptureServis.IzmeniKolicinuAsync(id, namirnicaId, dto);
        if (!uspesno) return BadRequest(new { Poruka = greska });
        return NoContent();
    }

    [HttpDelete("stavke/{id:int}/receptura/{namirnicaId:int}")]
    [SlucajKoriscenja("UkloniIzRecepture", Uloge.Administrator)]
    public async Task<IActionResult> UkloniIzRecepture(int id, int namirnicaId)
    {
        var (uspesno, greska) = await _receptureServis.UkloniAsync(id, namirnicaId);
        if (!uspesno) return BadRequest(new { Poruka = greska });
        return NoContent();
    }
}
