using Services.DTO;

namespace Services;

public interface IAuditLogServis
{
    Task ZabeleziAsync(string? korisnikId, string nazivSlucajaKoriscenja, bool uspesno, string? poruka = null);
    Task<PaginiranaListaDto<AuditLogDto>> PretraziAsync(AuditLogPretragaDto filter);
}
