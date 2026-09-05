using Domain.Entiteti;
using Domain.Konstante;
using Microsoft.EntityFrameworkCore;
using Repository.Interfejsi;
using Repository.Rezultati;

namespace Repository;

public class ZaposleniRepository : BazniRepository<Zaposleni>, IZaposleniRepository
{
    public ZaposleniRepository(RestoranDbContext kontekst) : base(kontekst) { }

    public async Task<List<Zaposleni>> ListirajSaKorisnikomAsync() =>
        await _dbSet.Include(z => z.Korisnik).ToListAsync();

    private IQueryable<ZaposleniSaUlogomRezultat> OsnovniUpit() =>
        from z in _dbSet
        join ur in _kontekst.UserRoles on z.KorisnikId equals ur.UserId
        join r in _kontekst.Roles on ur.RoleId equals r.Id
        where Uloge.Osoblje.Contains(r.Name!)
        select new ZaposleniSaUlogomRezultat
        {
            Id = z.Id,
            KorisnikId = z.KorisnikId,
            Ime = z.Korisnik.Ime,
            Prezime = z.Korisnik.Prezime,
            Email = z.Korisnik.Email ?? string.Empty,
            Uloga = r.Name!,
            DatumZaposlenja = z.DatumZaposlenja,
            Aktivan = z.Korisnik.Aktivan,
            SlikaUrl = z.SlikaUrl,
            Biografija = z.Biografija,
            LinkedInUrl = z.LinkedInUrl,
            InstagramUrl = z.InstagramUrl,
            Redosled = z.Redosled,
            PrikaziNaSajtu = z.PrikaziNaSajtu
        };

    public async Task<List<ZaposleniSaUlogomRezultat>> ListirajZaSajtAsync() =>
        await OsnovniUpit()
            .Where(x => x.PrikaziNaSajtu && x.Aktivan)
            .OrderBy(x => x.Redosled).ThenBy(x => x.Prezime)
            .ToListAsync();

    public async Task<(List<ZaposleniSaUlogomRezultat> Podaci, int Ukupno)> PretraziAsync(
        string? pretraga, string? uloga, bool? aktivan, int strana, int velicinaStrane)
    {
        var upit = OsnovniUpit();

        if (!string.IsNullOrWhiteSpace(pretraga))
            upit = upit.Where(x =>
                x.Ime.Contains(pretraga) ||
                x.Prezime.Contains(pretraga) ||
                x.Email.Contains(pretraga));

        if (!string.IsNullOrWhiteSpace(uloga))
            upit = upit.Where(x => x.Uloga == uloga);

        if (aktivan.HasValue)
            upit = upit.Where(x => x.Aktivan == aktivan.Value);

        var ukupno = await upit.CountAsync();

        var podaci = await upit
            .OrderBy(x => x.Prezime).ThenBy(x => x.Ime)
            .Skip((strana - 1) * velicinaStrane)
            .Take(velicinaStrane)
            .ToListAsync();

        return (podaci, ukupno);
    }

    public async Task<Zaposleni?> DobaviPoKorisnikIdAsync(string korisnikId) =>
        await _dbSet.FirstOrDefaultAsync(z => z.KorisnikId == korisnikId);

    public async Task<Zaposleni?> DobaviSaKorisnikomAsync(int zaposleniId) =>
        await _dbSet.Include(z => z.Korisnik).FirstOrDefaultAsync(z => z.Id == zaposleniId);
}
