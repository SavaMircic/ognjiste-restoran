using Services.DTO;

namespace Services;

public interface INamirnicaServis
{
    Task<List<NamirnicaDto>> ListirajAsync();
    Task<(bool Uspesno, string? Greska, NamirnicaDto? Namirnica)> KreirajAsync(KreirajNamirnicuDto dto);
    Task<(bool Uspesno, string? Greska)> IzmeniAsync(int id, IzmenaNamirniceDto dto);
    Task<(bool Uspesno, string? Greska, NamirnicaDto? Namirnica)> KorigujKolicinuAsync(int id, string trenutniKorisnikId, KorekcijaKolicineDto dto);
    Task<List<NabavkaStavkaDto>> ListaZaNabavkuAsync();
}
