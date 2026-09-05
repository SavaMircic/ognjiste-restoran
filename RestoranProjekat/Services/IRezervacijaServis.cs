using Services.DTO;

namespace Services;

public interface IRezervacijaServis
{
    Task<DostupnostDto> ProveriDostupnostAsync(DostupnostUpitDto upit);
    Task<(bool Uspesno, string? Greska, RezervacijaDto? Rezervacija)> KreirajAsync(string korisnikId, KreirajRezervacijuDto dto);
    Task<List<RezervacijaDto>> MojeAsync(string korisnikId);
    Task<(bool Uspesno, string? Greska)> OtkaziAsync(int id, string korisnikId);
    Task<(bool Uspesno, string? Greska, RezervacijaDto? Rezervacija)> KreirajAdminAsync(string adminKorisnikId, KreirajAdminRezervacijuDto dto);
    Task<PaginiranaListaDto<RezervacijaDto>> PretraziAsync(RezervacijePretragaDto filter);

    Task<List<RezervacijaDto>> ZaDanasAsync();

    Task<(bool Uspesno, string? Greska)> PrijaviDolazakAsync(int id, string trenutniKorisnikId);
    Task<(bool Uspesno, string? Greska)> OznaciIsteklomAsync(int id);

    Task<List<string>> IsteciDospeleAsync();
}
