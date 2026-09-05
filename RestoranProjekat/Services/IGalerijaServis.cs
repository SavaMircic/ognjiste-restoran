using Services.DTO;

namespace Services;

public interface IGalerijaServis
{
    Task<List<GalerijaSlikaDto>> ListirajAktivneAsync();

    Task<List<GalerijaSlikaDto>> ListirajSveAsync();

    Task<(bool Uspesno, string? Greska, GalerijaSlikaDto? Slika)> DodajAsync(
        DatotekaZaUploadDto datoteka, string naslov, string? opis, string grupa);

    Task<(bool Uspesno, string? Greska)> IzmeniAsync(int id, IzmenaGalerijeSlikeDto dto);

    Task<(bool Uspesno, string? Greska)> ObrisiAsync(int id);
}
