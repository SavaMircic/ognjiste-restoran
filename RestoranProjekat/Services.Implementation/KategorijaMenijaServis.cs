using Domain.Entiteti;
using Repository.Interfejsi;
using Services.DTO;

namespace Services.Implementation;

public class KategorijaMenijaServis : IKategorijaMenijaServis
{
    private readonly IKategorijaMenijaRepository _repository;

    public KategorijaMenijaServis(IKategorijaMenijaRepository repository)
    {
        _repository = repository;
    }

    public async Task<List<KategorijaMenijaDto>> ListirajAsync()
    {
        var kategorije = await _repository.ListirajPoRedosleduAsync();
        return kategorije.Select(Mapiraj).ToList();
    }

    public async Task<KategorijaMenijaDto> KreirajAsync(KategorijaMenijaUlazDto dto)
    {
        var kategorija = new KategorijaMenija
        {
            Naziv = dto.Naziv,
            Opis = dto.Opis,
            Redosled = dto.Redosled,
            Odrediste = dto.Odrediste
        };

        await _repository.DodajAsync(kategorija);
        await _repository.SacuvajPromeneAsync();

        return Mapiraj(kategorija);
    }

    public async Task<(bool Uspesno, string? Greska)> IzmeniAsync(int id, KategorijaMenijaUlazDto dto)
    {
        var kategorija = await _repository.DobaviPoIdAsync(id);
        if (kategorija == null) return (false, "Kategorija ne postoji.");

        kategorija.Naziv = dto.Naziv;
        kategorija.Opis = dto.Opis;
        kategorija.Redosled = dto.Redosled;
        kategorija.Odrediste = dto.Odrediste;

        _repository.Azuriraj(kategorija);
        await _repository.SacuvajPromeneAsync();
        return (true, null);
    }

    public async Task<(bool Uspesno, string? Greska)> ObrisiAsync(int id)
    {
        var kategorija = await _repository.DobaviPoIdAsync(id);
        if (kategorija == null) return (false, "Kategorija ne postoji.");

        if (await _repository.ImaStavkiAsync(id))
            return (false, "Kategorija sadrži stavke menija — prvo ih premestite u drugu kategoriju ili obrišite.");

        _repository.Obrisi(kategorija);
        await _repository.SacuvajPromeneAsync();
        return (true, null);
    }

    private static KategorijaMenijaDto Mapiraj(KategorijaMenija k) => new()
    {
        Id = k.Id,
        Naziv = k.Naziv,
        Opis = k.Opis,
        Redosled = k.Redosled,
        Odrediste = k.Odrediste
    };
}
