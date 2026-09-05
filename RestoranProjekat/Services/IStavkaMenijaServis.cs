using Services.DTO;

namespace Services;

public interface IStavkaMenijaServis
{
    Task<PaginiranaListaDto<StavkaMenijaDto>> PretraziAsync(StavkeMenijaPretragaDto filter);
    Task<StavkaMenijaDto?> DobaviDetaljAsync(int id);
    Task<(bool Uspesno, string? Greska, int? Id)> KreirajAsync(StavkaMenijaUlazDto dto);
    Task<(bool Uspesno, string? Greska)> IzmeniAsync(int id, StavkaMenijaUlazDto dto);
    Task<(bool Uspesno, string? Greska)> PromeniDostupnostAsync(int id, PromenaDostupnostiDto dto);
    Task<(bool Uspesno, string? Greska)> ObrisiAsync(int id);

    Task<(bool Uspesno, string? Greska, string? SlikaUrl)> PostaviGlavnuSlikuAsync(
        int id, DatotekaZaUploadDto datoteka);

    Task<(bool Uspesno, string? Greska, SlikaStavkeMenijaDto? Slika)> DodajDodatnuSlikuAsync(
        int id, DatotekaZaUploadDto datoteka, string? opis);

    Task<List<SlikaStavkeMenijaDto>> ListirajDodatneSlikeAsync(int id);

    Task<(bool Uspesno, string? Greska)> ObrisiDodatnuSlikuAsync(int id, int slikaId);

    Task<(bool Uspesno, string? Greska)> PoredjajDodatneSlikeAsync(int id, RedosledSlikaDto dto);
}
