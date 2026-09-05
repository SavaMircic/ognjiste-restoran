using Domain.Entiteti;

namespace Repository.Interfejsi;

public interface IAuditLogRepository : IRepository<AuditLog>
{
    Task<(List<AuditLog> Podaci, int Ukupno)> PretraziAsync(
        string? korisnikId, string? nazivSlucajaKoriscenja, string? pretraga,
        DateTime? datumOd, DateTime? datumDo,
        int strana, int velicinaStrane);
}
