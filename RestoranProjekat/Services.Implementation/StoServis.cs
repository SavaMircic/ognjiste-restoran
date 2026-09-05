using Domain.Entiteti;
using Domain.Enumi;
using Repository.Interfejsi;
using Services.DTO;

namespace Services.Implementation;

public class StoServis : IStoServis
{
    private readonly IStoRepository _repository;

    public StoServis(IStoRepository repository)
    {
        _repository = repository;
    }

    public async Task<List<StoDto>> ListirajAsync()
    {
        var stolovi = await _repository.ListirajAsync();
        return stolovi.Select(Mapiraj).ToList();
    }

    public async Task<(bool Uspesno, string? Greska, StoDto? Sto)> KreirajAsync(StoUlazDto dto)
    {
        if (await _repository.PostojiBrojStolaAsync(dto.BrojStola))
            return (false, $"Sto broj {dto.BrojStola} već postoji.", null);

        var sto = new Sto { BrojStola = dto.BrojStola, Kapacitet = dto.Kapacitet, TrenutniStatus = StatusStola.Slobodan };
        await _repository.DodajAsync(sto);
        await _repository.SacuvajPromeneAsync();

        return (true, null, Mapiraj(sto));
    }

    public async Task<(bool Uspesno, string? Greska)> IzmeniAsync(int id, StoUlazDto dto)
    {
        var sto = await _repository.DobaviPoIdAsync(id);
        if (sto == null) return (false, "Sto ne postoji.");

        if (await _repository.PostojiBrojStolaAsync(dto.BrojStola, id))
            return (false, $"Sto broj {dto.BrojStola} već postoji.");

        sto.BrojStola = dto.BrojStola;
        sto.Kapacitet = dto.Kapacitet;

        _repository.Azuriraj(sto);
        await _repository.SacuvajPromeneAsync();
        return (true, null);
    }

    private static StoDto Mapiraj(Sto s) => new()
    {
        Id = s.Id,
        BrojStola = s.BrojStola,
        Kapacitet = s.Kapacitet,
        TrenutniStatus = s.TrenutniStatus
    };
}
