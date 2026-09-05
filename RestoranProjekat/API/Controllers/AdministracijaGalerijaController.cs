using API.Autorizacija;
using API.Ulaz;
using Domain.Konstante;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services;
using Services.DTO;

namespace API.Controllers;

[ApiController]
[Route("api/administracija/galerija")]
[Authorize(Roles = Uloge.Administrator)]
public class AdministracijaGalerijaController : ControllerBase
{
    private readonly IGalerijaServis _galerijaServis;

    public AdministracijaGalerijaController(IGalerijaServis galerijaServis)
    {
        _galerijaServis = galerijaServis;
    }

    [HttpGet]
    [SlucajKoriscenja("ListirajGaleriju", Uloge.Administrator)]
    public async Task<IActionResult> Sve() => Ok(await _galerijaServis.ListirajSveAsync());

    [HttpPost]
    [RequestSizeLimit(3 * 1024 * 1024)]
    [SlucajKoriscenja("DodajUGaleriju", Uloge.Administrator)]
    public async Task<IActionResult> Dodaj([FromForm] UploadGalerijeDto ulaz)
    {
        var datoteka = new DatotekaZaUploadDto
        {
            Sadrzaj = ulaz.Slika!.OpenReadStream(),
            ImeDatoteke = ulaz.Slika.FileName,
            TipSadrzaja = ulaz.Slika.ContentType,
            Velicina = ulaz.Slika.Length
        };

        var (uspesno, greska, slika) = await _galerijaServis.DodajAsync(
            datoteka, ulaz.Naslov, ulaz.Opis, ulaz.Grupa);
        if (!uspesno) return BadRequest(new { Poruka = greska });
        return StatusCode(StatusCodes.Status201Created, slika);
    }

    [HttpPut("{id:int}")]
    [SlucajKoriscenja("IzmeniSlikuGalerije", Uloge.Administrator)]
    public async Task<IActionResult> Izmeni(int id, IzmenaGalerijeSlikeDto dto)
    {
        var (uspesno, greska) = await _galerijaServis.IzmeniAsync(id, dto);
        if (!uspesno) return BadRequest(new { Poruka = greska });
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    [SlucajKoriscenja("ObrisiSlikuGalerije", Uloge.Administrator)]
    public async Task<IActionResult> Obrisi(int id)
    {
        var (uspesno, greska) = await _galerijaServis.ObrisiAsync(id);
        if (!uspesno) return BadRequest(new { Poruka = greska });
        return NoContent();
    }
}
