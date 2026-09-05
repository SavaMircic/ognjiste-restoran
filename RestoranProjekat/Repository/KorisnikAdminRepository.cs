using Domain.Entiteti;
using Microsoft.EntityFrameworkCore;
using Repository.Interfejsi;

namespace Repository;

public class KorisnikAdminRepository : IKorisnikAdminRepository
{
    private readonly RestoranDbContext _kontekst;

    public KorisnikAdminRepository(RestoranDbContext kontekst)
    {
        _kontekst = kontekst;
    }

    public async Task<(List<Korisnik> Podaci, int Ukupno)> PretraziAsync(
        string? pretraga, bool? blokiran, int strana, int velicinaStrane)
    {
        var upit = _kontekst.Users.AsQueryable();

        if (!string.IsNullOrWhiteSpace(pretraga))
            upit = upit.Where(k =>
                k.Ime.Contains(pretraga) || k.Prezime.Contains(pretraga) ||
                (k.Email != null && k.Email.Contains(pretraga)));

        if (blokiran.HasValue)
        {
            var sada = DateTime.UtcNow;
            upit = blokiran.Value
                ? upit.Where(k => !k.Aktivan || (k.BlokiranDo != null && k.BlokiranDo > sada))
                : upit.Where(k => k.Aktivan && (k.BlokiranDo == null || k.BlokiranDo <= sada));
        }

        var ukupno = await upit.CountAsync();

        var podaci = await upit
            .OrderBy(k => k.Prezime).ThenBy(k => k.Ime)
            .Skip((strana - 1) * velicinaStrane)
            .Take(velicinaStrane)
            .ToListAsync();

        return (podaci, ukupno);
    }
    public async Task<int> BrojBlokiranihAsync() =>
        await _kontekst.Users.CountAsync(k => !k.Aktivan || (k.BlokiranDo != null && k.BlokiranDo > DateTime.UtcNow));
}