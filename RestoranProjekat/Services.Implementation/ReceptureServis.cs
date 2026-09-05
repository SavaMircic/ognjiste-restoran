using Domain.Entiteti;
using Repository.Interfejsi;
using Services.DTO;

namespace Services.Implementation;

public class ReceptureServis : IReceptureServis
{
    private readonly IReceptureRepository _receptureRepository;
    private readonly IStavkaMenijaRepository _stavkaRepository;
    private readonly IRepository<Namirnica> _namirnicaRepository;

    public ReceptureServis(
        IReceptureRepository receptureRepository, IStavkaMenijaRepository stavkaRepository, IRepository<Namirnica> namirnicaRepository)
    {
        _receptureRepository = receptureRepository;
        _stavkaRepository = stavkaRepository;
        _namirnicaRepository = namirnicaRepository;
    }

    public async Task<List<ReceptStavkaDto>> ListirajAsync(int stavkaMenijaId)
    {
        var redovi = await _receptureRepository.ListirajZaStavkuAsync(stavkaMenijaId);
        return redovi.Select(r => new ReceptStavkaDto
        {
            NamirnicaId = r.NamirnicaId,
            NamirnicaNaziv = r.Namirnica.Naziv,
            JedinicaMere = r.Namirnica.JedinicaMere,
            Kolicina = r.Kolicina
        }).ToList();
    }

    public async Task<(bool Uspesno, string? Greska)> DodajAsync(int stavkaMenijaId, DodajURecepturuDto dto)
    {
        if (await _stavkaRepository.DobaviPoIdAsync(stavkaMenijaId) == null)
            return (false, "Stavka menija ne postoji.");

        if (await _namirnicaRepository.DobaviPoIdAsync(dto.NamirnicaId) == null)
            return (false, "Namirnica ne postoji.");

        if (await _receptureRepository.DobaviAsync(stavkaMenijaId, dto.NamirnicaId) != null)
            return (false, "Namirnica je već u recepturi ove stavke — koristite izmenu količine.");

        var red = new Receptura { StavkaMenijaId = stavkaMenijaId, NamirnicaId = dto.NamirnicaId, Kolicina = dto.Kolicina };
        await _receptureRepository.DodajAsync(red);
        await _receptureRepository.SacuvajPromeneAsync();

        return (true, null);
    }

    public async Task<(bool Uspesno, string? Greska)> IzmeniKolicinuAsync(int stavkaMenijaId, int namirnicaId, IzmenaKolicineDto dto)
    {
        var red = await _receptureRepository.DobaviAsync(stavkaMenijaId, namirnicaId);
        if (red == null) return (false, "Namirnica nije u recepturi ove stavke.");

        red.Kolicina = dto.Kolicina;
        _receptureRepository.Azuriraj(red);
        await _receptureRepository.SacuvajPromeneAsync();
        return (true, null);
    }

    public async Task<(bool Uspesno, string? Greska)> UkloniAsync(int stavkaMenijaId, int namirnicaId)
    {
        var red = await _receptureRepository.DobaviAsync(stavkaMenijaId, namirnicaId);
        if (red == null) return (false, "Namirnica nije u recepturi ove stavke.");

        _receptureRepository.Obrisi(red);
        await _receptureRepository.SacuvajPromeneAsync();
        return (true, null);
    }
}
