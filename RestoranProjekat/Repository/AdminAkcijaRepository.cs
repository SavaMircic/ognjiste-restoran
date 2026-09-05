using Domain.Entiteti;
using Microsoft.EntityFrameworkCore;
using Repository.Interfejsi;

namespace Repository;

public class AdminAkcijaRepository : BazniRepository<AdminAkcija>, IAdminAkcijaRepository
{
    public AdminAkcijaRepository(RestoranDbContext kontekst) : base(kontekst) { }

    public async Task<(List<AdminAkcija> Podaci, int Ukupno)> PretraziAsync(
        string? tipAkcije, string? ciljniKorisnikId, string? pretraga,
        DateTime? datumOd, DateTime? datumDo,
        int strana, int velicinaStrane)
    {
        var upit = _dbSet
            .Include(a => a.Administrator).ThenInclude(z => z.Korisnik)
            .Include(a => a.CiljniKorisnik)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(tipAkcije))
            upit = upit.Where(a => a.TipAkcije == tipAkcije);

        if (!string.IsNullOrWhiteSpace(ciljniKorisnikId))
            upit = upit.Where(a => a.CiljniKorisnikId == ciljniKorisnikId);

        if (!string.IsNullOrWhiteSpace(pretraga))
            upit = upit.Where(a => a.CiljniKorisnik != null && a.CiljniKorisnik.Email != null && a.CiljniKorisnik.Email.Contains(pretraga));

        if (datumOd.HasValue) upit = upit.Where(a => a.Datum >= datumOd.Value);
        if (datumDo.HasValue) upit = upit.Where(a => a.Datum <= datumDo.Value);

        var ukupno = await upit.CountAsync();

        var podaci = await upit
            .OrderByDescending(a => a.Datum)
            .Skip((strana - 1) * velicinaStrane)
            .Take(velicinaStrane)
            .ToListAsync();

        return (podaci, ukupno);
    }
}
