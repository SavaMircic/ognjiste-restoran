using Domain.Entiteti;
using Domain.Enumi;
using Microsoft.EntityFrameworkCore;
using Repository.Interfejsi;
using Repository.Rezultati;

namespace Repository;

public class PorudzbinaRepository : BazniRepository<Porudzbina>, IPorudzbinaRepository
{
    public PorudzbinaRepository(RestoranDbContext kontekst) : base(kontekst) { }

    public async Task<List<Porudzbina>> ListirajAktivneAsync() =>
        await _dbSet
            .Include(p => p.Sto)
            .Include(p => p.Stavke)
            .Where(p => p.Status == StatusPorudzbine.Otvorena)
            .OrderBy(p => p.VremeOtvaranja)
            .ToListAsync();

    public async Task<Porudzbina?> DobaviDetaljAsync(int id) =>
        await _dbSet
            .Include(p => p.Sto)
            .Include(p => p.Konobar).ThenInclude(z => z.Korisnik)
            .Include(p => p.Stavke).ThenInclude(s => s.StavkaMenija)
            .Include(p => p.Stavke).ThenInclude(s => s.PripremioZaposleni)
            .FirstOrDefaultAsync(p => p.Id == id);

    public async Task<Porudzbina?> DobaviOtvorenuZaStoAsync(int stoId) =>
        await _dbSet.FirstOrDefaultAsync(p => p.StoId == stoId && p.Status == StatusPorudzbine.Otvorena);

    public async Task<int> BrojOtvorenihAsync() =>
        await _dbSet.CountAsync(p => p.Status == StatusPorudzbine.Otvorena);

    public async Task<List<RacunGostaRezultat>> RacuniGostaAsync(string korisnikId, int maks) =>
        await _dbSet
            .Where(p => p.Status == StatusPorudzbine.Zatvorena &&
                        p.Rezervacija != null && p.Rezervacija.KorisnikId == korisnikId)
            .OrderByDescending(p => p.VremeZatvaranja)
            .Take(maks)
            .Select(p => new RacunGostaRezultat
            {
                PorudzbinaId = p.Id,
                KodRezervacije = p.Rezervacija!.KodRezervacije,
                VremeZatvaranja = p.VremeZatvaranja,
                BrojStola = p.Sto.BrojStola,
                Ukupno = p.Stavke.Sum(s => (decimal?)(s.CenaUTrenutkuNarudzbine * s.Kolicina)) ?? 0,
                Napojnica = p.IznosNapojnice ?? 0
            })
            .ToListAsync();
}
