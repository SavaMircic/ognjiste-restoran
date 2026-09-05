using Domain.Entiteti;
using Microsoft.EntityFrameworkCore;
using Repository.Interfejsi;

namespace Repository;

public class AuditLogRepository : BazniRepository<AuditLog>, IAuditLogRepository
{
    public AuditLogRepository(RestoranDbContext kontekst) : base(kontekst) { }

    public async Task<(List<AuditLog> Podaci, int Ukupno)> PretraziAsync(
        string? korisnikId, string? nazivSlucajaKoriscenja, string? pretraga,
        DateTime? datumOd, DateTime? datumDo,
        int strana, int velicinaStrane)
    {
        var upit = _dbSet.Include(a => a.Korisnik).AsQueryable();

        if (!string.IsNullOrWhiteSpace(korisnikId))
            upit = upit.Where(a => a.KorisnikId == korisnikId);

        if (!string.IsNullOrWhiteSpace(nazivSlucajaKoriscenja))
            upit = upit.Where(a => a.NazivSlucajaKoriscenja.Contains(nazivSlucajaKoriscenja));

        if (!string.IsNullOrWhiteSpace(pretraga))
            upit = upit.Where(a => a.Korisnik != null && a.Korisnik.Email != null && a.Korisnik.Email.Contains(pretraga));

        if (datumOd.HasValue)
            upit = upit.Where(a => a.Datum >= datumOd.Value);

        if (datumDo.HasValue)
            upit = upit.Where(a => a.Datum <= datumDo.Value);

        var ukupno = await upit.CountAsync();

        var podaci = await upit
            .OrderByDescending(a => a.Datum)
            .Skip((strana - 1) * velicinaStrane)
            .Take(velicinaStrane)
            .ToListAsync();

        return (podaci, ukupno);
    }
}
