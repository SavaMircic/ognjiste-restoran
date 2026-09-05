using Domain.Entiteti;
using Repository.Interfejsi;
using Services.DTO;

namespace Services.Implementation;

public class LajkJelaServis : ILajkJelaServis
{
    private readonly ILajkJelaRepository _lajkRepository;
    private readonly IStavkaMenijaRepository _stavkaMenijaRepository;
    private readonly IRecenzijaRepository _recenzijaRepository;

    public LajkJelaServis(
        ILajkJelaRepository lajkRepository, IStavkaMenijaRepository stavkaMenijaRepository, IRecenzijaRepository recenzijaRepository)
    {
        _lajkRepository = lajkRepository;
        _stavkaMenijaRepository = stavkaMenijaRepository;
        _recenzijaRepository = recenzijaRepository;
    }

    public async Task<(bool Uspesno, string? Greska, int StatusKod)> LajkujAsync(int stavkaMenijaId, string korisnikId)
    {
        if (await _stavkaMenijaRepository.DobaviPoIdAsync(stavkaMenijaId) == null)
            return (false, "Stavka menija ne postoji.", 400);

        if (await _lajkRepository.DobaviAsync(korisnikId, stavkaMenijaId) != null)
            return (false, "Već ste lajkovali ovo jelo.", 409);

        await _lajkRepository.DodajAsync(new LajkJela
        {
            KorisnikId = korisnikId,
            StavkaMenijaId = stavkaMenijaId,
            DatumKreiranja = DateTime.UtcNow
        });
        await _lajkRepository.SacuvajPromeneAsync();

        return (true, null, 201);
    }

    public async Task<(bool Uspesno, string? Greska)> UkloniLajkAsync(int stavkaMenijaId, string korisnikId)
    {
        var lajk = await _lajkRepository.DobaviAsync(korisnikId, stavkaMenijaId);
        if (lajk == null) return (false, "Niste lajkovali ovo jelo.");

        _lajkRepository.Obrisi(lajk);
        await _lajkRepository.SacuvajPromeneAsync();
        return (true, null);
    }

    public async Task<List<StavkaMenijaDto>> OmiljenaJelaAsync(string korisnikId)
    {
        var stavke = await _lajkRepository.ListirajOmiljenaJelaAsync(korisnikId);

        var ids = stavke.Select(s => s.Id).ToList();
        var statistika = await _recenzijaRepository.StatistikaZaStavkeAsync(ids);
        var lajkovi = await _lajkRepository.BrojLajkovaZaStavkeAsync(ids);

        return stavke.Select(s => StavkaMenijaServis.Mapiraj(s, statistika, lajkovi)).ToList();
    }
}
