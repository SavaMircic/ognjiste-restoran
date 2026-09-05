using Domain.Entiteti;
using Repository.Interfejsi;
using Services.DTO;

namespace Services.Implementation;

public class AuditLogServis : IAuditLogServis
{
    private readonly IAuditLogRepository _auditLogRepository;

    public AuditLogServis(IAuditLogRepository auditLogRepository)
    {
        _auditLogRepository = auditLogRepository;
    }

    public async Task ZabeleziAsync(string? korisnikId, string nazivSlucajaKoriscenja, bool uspesno, string? poruka = null)
    {
        var zapis = new AuditLog
        {
            KorisnikId = korisnikId,
            NazivSlucajaKoriscenja = nazivSlucajaKoriscenja,
            Datum = DateTime.UtcNow,
            UspesnoIzvrseno = uspesno,
            Poruka = poruka
        };

        await _auditLogRepository.DodajAsync(zapis);
        await _auditLogRepository.SacuvajPromeneAsync();
    }

    public async Task<PaginiranaListaDto<AuditLogDto>> PretraziAsync(AuditLogPretragaDto filter)
    {
        var (podaci, ukupno) = await _auditLogRepository.PretraziAsync(
            filter.KorisnikId, filter.NazivSlucajaKoriscenja, filter.Pretraga,
            filter.DatumOd, filter.DatumDo,
            filter.Strana, filter.VelicinaStrane);

        return new PaginiranaListaDto<AuditLogDto>
        {
            Podaci = podaci.Select(a => new AuditLogDto
            {
                Id = a.Id,
                KorisnikId = a.KorisnikId,
                KorisnikEmail = a.Korisnik?.Email,
                NazivSlucajaKoriscenja = a.NazivSlucajaKoriscenja,
                Datum = a.Datum,
                UspesnoIzvrseno = a.UspesnoIzvrseno,
                Poruka = a.Poruka
            }).ToList(),
            UkupnoZapisa = ukupno,
            TrenutnaStrana = filter.Strana,
            UkupnoStrana = (int)Math.Ceiling(ukupno / (double)filter.VelicinaStrane)
        };
    }
}
