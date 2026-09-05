using Domain.Entiteti;
using Domain.Enumi;
using Repository.Interfejsi;
using Services.DTO;

namespace Services.Implementation;

public class PrijavaProblemaServis : IPrijavaProblemaServis
{
    private readonly IPrijavaProblemaRepository _prijavaRepository;
    private readonly IZaposleniRepository _zaposleniRepository;

    public PrijavaProblemaServis(
        IPrijavaProblemaRepository prijavaRepository, IZaposleniRepository zaposleniRepository)
    {
        _prijavaRepository = prijavaRepository;
        _zaposleniRepository = zaposleniRepository;
    }

    public async Task<(bool Uspesno, string? Greska, PrijavaProblemaDto? Prijava)> PrijaviAsync(
        string trenutniKorisnikId, PrijaviProblemDto dto)
    {
        var zaposleni = await _zaposleniRepository.DobaviPoKorisnikIdAsync(trenutniKorisnikId);
        if (zaposleni == null) return (false, "Nalog nije povezan sa zaposlenim.", null);

        var prijava = new PrijavaProblema
        {
            PrijavioZaposleniId = zaposleni.Id,
            Naslov = dto.Naslov,
            Opis = dto.Opis,
            Kategorija = dto.Kategorija,
            Prioritet = dto.Prioritet,
            Status = StatusPrijaveProblema.Nova,
            DatumPrijave = DateTime.UtcNow
        };

        await _prijavaRepository.DodajAsync(prijava);
        await _prijavaRepository.SacuvajPromeneAsync();

        var sacuvana = await _prijavaRepository.DobaviSaImenimaAsync(prijava.Id);
        return (true, null, Mapiraj(sacuvana!));
    }

    public async Task<(bool Uspesno, string? Greska, List<PrijavaProblemaDto>? Prijave)> MojePrijaveAsync(
        string trenutniKorisnikId)
    {
        var zaposleni = await _zaposleniRepository.DobaviPoKorisnikIdAsync(trenutniKorisnikId);
        if (zaposleni == null) return (false, "Nalog nije povezan sa zaposlenim.", null);

        var prijave = await _prijavaRepository.ListirajZaZaposlenogAsync(zaposleni.Id);
        return (true, null, prijave.Select(Mapiraj).ToList());
    }

    public async Task<PaginiranaListaDto<PrijavaProblemaDto>> PretraziAsync(PrijaveProblemaPretragaDto filter)
    {
        var (podaci, ukupno) = await _prijavaRepository.PretraziAsync(
            filter.Status, filter.Kategorija, filter.Prioritet, filter.Strana, filter.VelicinaStrane);

        return new PaginiranaListaDto<PrijavaProblemaDto>
        {
            Podaci = podaci.Select(Mapiraj).ToList(),
            UkupnoZapisa = ukupno,
            TrenutnaStrana = filter.Strana,
            UkupnoStrana = (int)Math.Ceiling(ukupno / (double)filter.VelicinaStrane)
        };
    }

    public async Task<(bool Uspesno, string? Greska)> ResiAsync(
        int id, string trenutniKorisnikId, ResiPrijavuProblemaDto dto)
    {
        var prijava = await _prijavaRepository.DobaviPoIdAsync(id);
        if (prijava == null) return (false, "Prijava ne postoji.");

        var menadzer = await _zaposleniRepository.DobaviPoKorisnikIdAsync(trenutniKorisnikId);
        if (menadzer == null) return (false, "Nalog nije povezan sa zaposlenim.");

        prijava.Status = dto.Status;
        prijava.Odgovor = dto.Odgovor;

        var zatvorena = dto.Status is StatusPrijaveProblema.Resena or StatusPrijaveProblema.Odbijena;
        prijava.ResioZaposleniId = zatvorena ? menadzer.Id : null;
        prijava.DatumResavanja = zatvorena ? DateTime.UtcNow : null;

        _prijavaRepository.Azuriraj(prijava);
        await _prijavaRepository.SacuvajPromeneAsync();
        return (true, null);
    }

    private static string Ime(Zaposleni? z) =>
        z?.Korisnik == null ? string.Empty : $"{z.Korisnik.Ime} {z.Korisnik.Prezime}";

    private static PrijavaProblemaDto Mapiraj(PrijavaProblema p) => new()
    {
        Id = p.Id,
        PrijavioZaposleniId = p.PrijavioZaposleniId,
        PrijavioIme = Ime(p.PrijavioZaposleni),
        Naslov = p.Naslov,
        Opis = p.Opis,
        Kategorija = p.Kategorija,
        Prioritet = p.Prioritet,
        Status = p.Status,
        DatumPrijave = p.DatumPrijave,
        ResioIme = p.ResioZaposleni == null ? null : Ime(p.ResioZaposleni),
        Odgovor = p.Odgovor,
        DatumResavanja = p.DatumResavanja
    };
}
