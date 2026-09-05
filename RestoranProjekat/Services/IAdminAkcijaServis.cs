using Services.DTO;

namespace Services;

public interface IAdminAkcijaServis
{
    Task ZabeleziAsync(string adminKorisnikId, string tipAkcije, string? ciljniKorisnikId, string opis);

    Task<PaginiranaListaDto<AdminAkcijaDto>> PretraziAsync(AdminAkcijePretragaDto filter);
}
