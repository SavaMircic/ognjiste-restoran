using Services.DTO;

namespace Services;

public interface IPorudzbinaServis
{
    Task<(bool Uspesno, string? Greska, int? Id)> OtvoriAsync(string trenutniKorisnikId, OtvoriPorudzbinuDto dto);
    Task<List<PorudzbinaAktivnaDto>> ListirajAktivneAsync();
    Task<PorudzbinaDetaljDto?> DobaviDetaljAsync(int id);
    Task<(bool Uspesno, string? Greska)> DodajStavkeAsync(int porudzbinaId, DodajStavkeDto dto);
    Task<(bool Uspesno, string? Greska)> OtkaziStavkuAsync(int porudzbinaId, int stavkaId);
    Task<(bool Uspesno, string? Greska)> ZatvoriAsync(int porudzbinaId, ZatvoriPorudzbinuDto dto);
    Task<(bool Uspesno, string? Greska, string? SadrzajRacuna)> DobaviRacunAsync(int porudzbinaId);
}
