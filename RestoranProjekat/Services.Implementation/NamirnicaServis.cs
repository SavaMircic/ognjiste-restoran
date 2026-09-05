using Domain.Entiteti;
using Repository.Interfejsi;
using Services.DTO;

namespace Services.Implementation;

public class NamirnicaServis : INamirnicaServis
{
    private const int CiljniFaktorPraga = 2;

    private readonly INamirnicaRepository _namirnicaRepository;
    private readonly IZaposleniRepository _zaposleniRepository;
    private readonly IRepository<KorekcijaZaliha> _korekcijaRepository;

    public NamirnicaServis(
        INamirnicaRepository namirnicaRepository, IZaposleniRepository zaposleniRepository,
        IRepository<KorekcijaZaliha> korekcijaRepository)
    {
        _namirnicaRepository = namirnicaRepository;
        _zaposleniRepository = zaposleniRepository;
        _korekcijaRepository = korekcijaRepository;
    }

    public async Task<List<NamirnicaDto>> ListirajAsync()
    {
        var namirnice = await _namirnicaRepository.ListirajAsync();
        return namirnice.Select(Mapiraj).ToList();
    }

    public async Task<(bool Uspesno, string? Greska, NamirnicaDto? Namirnica)> KreirajAsync(KreirajNamirnicuDto dto)
    {
        if (await _namirnicaRepository.PostojiNazivAsync(dto.Naziv))
            return (false, $"Namirnica '{dto.Naziv}' već postoji.", null);

        var namirnica = new Namirnica
        {
            Naziv = dto.Naziv,
            JedinicaMere = dto.JedinicaMere,
            TrenutnaKolicina = dto.PocetnaKolicina,
            MinimalniPrag = dto.MinimalniPrag
        };

        await _namirnicaRepository.DodajAsync(namirnica);
        await _namirnicaRepository.SacuvajPromeneAsync();

        return (true, null, Mapiraj(namirnica));
    }

    public async Task<(bool Uspesno, string? Greska)> IzmeniAsync(int id, IzmenaNamirniceDto dto)
    {
        var namirnica = await _namirnicaRepository.DobaviPoIdAsync(id);
        if (namirnica == null) return (false, "Namirnica ne postoji.");

        if (await _namirnicaRepository.PostojiNazivAsync(dto.Naziv, id))
            return (false, $"Namirnica '{dto.Naziv}' već postoji.");

        namirnica.Naziv = dto.Naziv;
        namirnica.MinimalniPrag = dto.MinimalniPrag;

        _namirnicaRepository.Azuriraj(namirnica);
        await _namirnicaRepository.SacuvajPromeneAsync();
        return (true, null);
    }

    public async Task<(bool Uspesno, string? Greska, NamirnicaDto? Namirnica)> KorigujKolicinuAsync(
        int id, string trenutniKorisnikId, KorekcijaKolicineDto dto)
    {
        var namirnica = await _namirnicaRepository.DobaviPoIdAsync(id);
        if (namirnica == null) return (false, "Namirnica ne postoji.", null);

        var zaposleni = await _zaposleniRepository.DobaviPoKorisnikIdAsync(trenutniKorisnikId);
        if (zaposleni == null) return (false, "Nalog nije povezan sa zaposlenim.", null);

        var staraKolicina = namirnica.TrenutnaKolicina;
        var novaKolicina = dto.NovaKolicina ?? staraKolicina + dto.Delta!.Value;

        if (novaKolicina < 0)
            return (false, "Korekcija bi dovela do negativnog stanja zalihe.", null);

        namirnica.TrenutnaKolicina = novaKolicina;
        _namirnicaRepository.Azuriraj(namirnica);

        await _korekcijaRepository.DodajAsync(new KorekcijaZaliha
        {
            NamirnicaId = namirnica.Id,
            StaraKolicina = staraKolicina,
            NovaKolicina = novaKolicina,
            Razlog = dto.Razlog,
            ZaposleniId = zaposleni.Id,
            Datum = DateTime.UtcNow
        });

        await _namirnicaRepository.SacuvajPromeneAsync();
        return (true, null, Mapiraj(namirnica));
    }

    public async Task<List<NabavkaStavkaDto>> ListaZaNabavkuAsync()
    {
        var namirnice = await _namirnicaRepository.ListirajIspodPragaAsync();

        return namirnice.Select(n => new NabavkaStavkaDto
        {
            NamirnicaId = n.Id,
            Naziv = n.Naziv,
            JedinicaMere = n.JedinicaMere,
            TrenutnaKolicina = n.TrenutnaKolicina,
            MinimalniPrag = n.MinimalniPrag,
            PredlozenaKolicina = (n.MinimalniPrag * CiljniFaktorPraga) - n.TrenutnaKolicina
        }).ToList();
    }

    private static NamirnicaDto Mapiraj(Namirnica n) => new()
    {
        Id = n.Id,
        Naziv = n.Naziv,
        JedinicaMere = n.JedinicaMere,
        TrenutnaKolicina = n.TrenutnaKolicina,
        MinimalniPrag = n.MinimalniPrag,
        IspodPraga = n.TrenutnaKolicina < n.MinimalniPrag
    };
}
