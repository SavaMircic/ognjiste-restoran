using Domain.Entiteti;
using Domain.Enumi;
using Domain.Konstante;
using Microsoft.EntityFrameworkCore;
using Repository.Interfejsi;
using Repository.Rezultati;

namespace Repository;

public class RezervacijaRepository : BazniRepository<Rezervacija>, IRezervacijaRepository
{
    private const int TrajanjeSati = PoslovnaPravila.TrajanjeRezervacijeSati;

    public RezervacijaRepository(RestoranDbContext kontekst) : base(kontekst) { }

    public async Task<List<Sto>> DobaviSlobodneStoloveAsync(DateTime datumVreme)
    {
        var pocetak = datumVreme.AddHours(-TrajanjeSati);
        var kraj = datumVreme.AddHours(TrajanjeSati);

        var zauzetiStoIds = await _kontekst.Rezervacije
            .Where(r => (r.Status == StatusRezervacije.Aktivna || r.Status == StatusRezervacije.Realizovana) &&
                        r.DatumVreme > pocetak && r.DatumVreme < kraj)
            .Select(r => r.StoId)
            .ToListAsync();

        return await _kontekst.Stolovi
            .Where(s => !zauzetiStoIds.Contains(s.Id))
            .OrderBy(s => s.Kapacitet)
            .ToListAsync();
    }

    public async Task<bool> PostojiKonfliktAsync(int stoId, DateTime datumVreme)
    {
        var pocetak = datumVreme.AddHours(-TrajanjeSati);
        var kraj = datumVreme.AddHours(TrajanjeSati);

        return await _dbSet.AnyAsync(r =>
            r.StoId == stoId &&
            (r.Status == StatusRezervacije.Aktivna || r.Status == StatusRezervacije.Realizovana) &&
            r.DatumVreme > pocetak && r.DatumVreme < kraj);
    }

    public async Task<PouzdanostGostaRezultat> PouzdanostGostaAsync(string korisnikId)
    {
        var poStatusu = await _dbSet
            .Where(r => r.KorisnikId == korisnikId)
            .GroupBy(r => r.Status)
            .Select(g => new { Status = g.Key, Broj = g.Count() })
            .ToListAsync();

        int Broj(StatusRezervacije s) => poStatusu.FirstOrDefault(x => x.Status == s)?.Broj ?? 0;

        return new PouzdanostGostaRezultat
        {
            Realizovane = Broj(StatusRezervacije.Realizovana),
            Istekle = Broj(StatusRezervacije.Istekla),
            Otkazane = Broj(StatusRezervacije.Otkazana),
            Aktivne = Broj(StatusRezervacije.Aktivna)
        };
    }

    public async Task<List<Rezervacija>> ListirajDospeleZaIstekAsync(DateTime granica) =>
        await _dbSet
            .Where(r => r.Status == StatusRezervacije.Aktivna && r.DatumVreme < granica)
            .ToListAsync();

    public async Task<Rezervacija?> DobaviPoKoduAsync(string kod) =>
        await _dbSet.FirstOrDefaultAsync(r => r.KodRezervacije == kod);

    public async Task<Rezervacija?> DobaviDetaljAsync(int id) =>
        await _dbSet.Include(r => r.Sto).Include(r => r.Korisnik).FirstOrDefaultAsync(r => r.Id == id);

    public async Task<List<Rezervacija>> ListirajZaKorisnikaAsync(string korisnikId) =>
        await _dbSet.Include(r => r.Sto)
            .Where(r => r.KorisnikId == korisnikId)
            .OrderByDescending(r => r.DatumVreme)
            .ToListAsync();

    public async Task<List<Rezervacija>> ListirajPoKoduAsync(string kod) =>
        await _dbSet.Include(r => r.Sto)
            .Where(r => r.KodRezervacije == kod)
            .OrderBy(r => r.Sto.BrojStola)
            .ToListAsync();

    public async Task<(List<Rezervacija> Podaci, int Ukupno)> PretraziAsync(
        DateTime? datum, StatusRezervacije? status, string? pretraga, int strana, int velicinaStrane)
    {
        var upit = _dbSet.Include(r => r.Sto).Include(r => r.Korisnik).AsQueryable();

        if (datum.HasValue)
            upit = upit.Where(r => r.DatumVreme.Date == datum.Value.Date);

        if (status.HasValue)
            upit = upit.Where(r => r.Status == status.Value);

        if (!string.IsNullOrWhiteSpace(pretraga))
            upit = upit.Where(r =>
                r.KodRezervacije.Contains(pretraga) ||
                (r.GostIme != null && r.GostIme.Contains(pretraga)) ||
                (r.GostEmail != null && r.GostEmail.Contains(pretraga)) ||
                (r.Korisnik != null && (
                    r.Korisnik.Ime.Contains(pretraga) ||
                    r.Korisnik.Prezime.Contains(pretraga) ||
                    (r.Korisnik.Email != null && r.Korisnik.Email.Contains(pretraga)))));

        var ukupno = await upit.CountAsync();

        var podaci = await upit
            .OrderByDescending(r => r.DatumVreme)
            .Skip((strana - 1) * velicinaStrane)
            .Take(velicinaStrane)
            .ToListAsync();

        return (podaci, ukupno);
    }
    public async Task<int> BrojAktivnihUOpseguAsync(DateTime od, DateTime doIsklj) =>
        await _dbSet.CountAsync(r => r.Status == StatusRezervacije.Aktivna &&
                                     r.DatumVreme >= od && r.DatumVreme < doIsklj);
}
