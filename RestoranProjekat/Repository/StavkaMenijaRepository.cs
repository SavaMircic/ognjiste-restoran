using Domain.Entiteti;
using Microsoft.EntityFrameworkCore;
using Repository.Interfejsi;

namespace Repository;

public class StavkaMenijaRepository : BazniRepository<StavkaMenija>, IStavkaMenijaRepository
{
    public StavkaMenijaRepository(RestoranDbContext kontekst) : base(kontekst) { }

    public async Task<(List<StavkaMenija> Podaci, int Ukupno)> PretraziAsync(
        int? kategorijaId, string? pretraga, string? sortiranje, int strana, int velicinaStrane)
    {
        var upit = _dbSet.Include(s => s.Kategorija).AsQueryable();

        if (kategorijaId.HasValue)
            upit = upit.Where(s => s.KategorijaId == kategorijaId.Value);

        if (!string.IsNullOrWhiteSpace(pretraga))
            upit = upit.Where(s => s.Naziv.Contains(pretraga) || s.Opis.Contains(pretraga));

        upit = sortiranje switch
        {
            "cena" => upit.OrderBy(s => s.Popust.HasValue ? s.Cena * (1 - s.Popust.Value / 100m) : s.Cena),
            "popularnost" => upit.OrderByDescending(s => _kontekst.LajkoviJela.Count(l => l.StavkaMenijaId == s.Id)),
            "ocena" => upit.OrderByDescending(s => _kontekst.Recenzije.Where(r => r.StavkaMenijaId == s.Id).Average(r => (double?)r.Ocena) ?? 0),
            _ => upit.OrderBy(s => s.Naziv)
        };

        var ukupno = await upit.CountAsync();

        var podaci = await upit
            .Skip((strana - 1) * velicinaStrane)
            .Take(velicinaStrane)
            .ToListAsync();

        return (podaci, ukupno);
    }

    public async Task<StavkaMenija?> DobaviDetaljAsync(int id) =>
        await _dbSet
            .Include(s => s.Kategorija)
            .Include(s => s.DodatneSlike.OrderBy(sl => sl.Redosled))
            .FirstOrDefaultAsync(s => s.Id == id);

    public async Task<List<SlikaStavkeMenija>> ListirajDodatneSlikeAsync(int stavkaMenijaId) =>
        await _kontekst.SlikeStavkiMenija
            .Where(s => s.StavkaMenijaId == stavkaMenijaId)
            .OrderBy(s => s.Redosled)
            .ToListAsync();

    public async Task<SlikaStavkeMenija?> DobaviDodatnuSlikuAsync(int stavkaMenijaId, int slikaId) =>
        await _kontekst.SlikeStavkiMenija
            .FirstOrDefaultAsync(s => s.Id == slikaId && s.StavkaMenijaId == stavkaMenijaId);

    public async Task<int> NajveciRedosledSlikeAsync(int stavkaMenijaId) =>
        await _kontekst.SlikeStavkiMenija
            .Where(s => s.StavkaMenijaId == stavkaMenijaId)
            .MaxAsync(s => (int?)s.Redosled) ?? 0;
}
