using Services.DTO;

namespace Services;

public interface IRecenzijaServis
{
    Task<PaginiranaListaDto<RecenzijaDto>> PretraziAsync(RecenzijePretragaDto filter);
    Task<(bool Uspesno, string? Greska, int StatusKod, RecenzijaDto? Recenzija)> KreirajAsync(string korisnikId, KreirajRecenzijuDto dto);
    Task<(bool Uspesno, string? Greska)> IzmeniAsync(int id, string korisnikId, IzmenaRecenzijeDto dto);
    Task<(bool Uspesno, string? Greska)> ObrisiAsync(int id, string korisnikId);
    Task<(bool Uspesno, string? Greska)> OdgovoriAsync(int id, OdgovorRecenzijeDto dto);
    Task<(bool Uspesno, string? Greska)> ObrisiAdminAsync(int id, string adminKorisnikId, string razlog);
}
