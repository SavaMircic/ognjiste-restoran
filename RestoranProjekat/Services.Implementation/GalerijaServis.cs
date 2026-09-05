using Domain.Entiteti;
using Repository.Interfejsi;
using Services.DTO;

namespace Services.Implementation;

public class GalerijaServis : IGalerijaServis
{
    private readonly IGalerijaRepository _galerijaRepository;
    private readonly ISkladisteSlika _skladisteSlika;

    public GalerijaServis(IGalerijaRepository galerijaRepository, ISkladisteSlika skladisteSlika)
    {
        _galerijaRepository = galerijaRepository;
        _skladisteSlika = skladisteSlika;
    }

    public async Task<List<GalerijaSlikaDto>> ListirajAktivneAsync() =>
        (await _galerijaRepository.ListirajAsync(samoAktivne: true)).Select(Mapiraj).ToList();

    public async Task<List<GalerijaSlikaDto>> ListirajSveAsync() =>
        (await _galerijaRepository.ListirajAsync(samoAktivne: false)).Select(Mapiraj).ToList();

    public async Task<(bool Uspesno, string? Greska, GalerijaSlikaDto? Slika)> DodajAsync(
        DatotekaZaUploadDto datoteka, string naslov, string? opis, string grupa)
    {
        if (string.IsNullOrWhiteSpace(naslov))
            return (false, "Naslov je obavezan.", null);

        if (string.IsNullOrWhiteSpace(grupa))
            return (false, "Oblast je obavezna.", null);

        var (uspesno, greska, url) = await _skladisteSlika.SacuvajAsync(
            datoteka, ISkladisteSlika.Podfolderi.Galerija);
        if (!uspesno) return (false, greska, null);

        var slika = new GalerijaSlika
        {
            SlikaUrl = url!,
            Naslov = naslov.Trim(),
            Opis = opis,
            Grupa = grupa.Trim(),
            Redosled = await _galerijaRepository.NajveciRedosledAsync() + 1,
            Aktivan = true,
            DatumDodavanja = DateTime.UtcNow
        };

        await _galerijaRepository.DodajAsync(slika);
        await _galerijaRepository.SacuvajPromeneAsync();

        return (true, null, Mapiraj(slika));
    }

    public async Task<(bool Uspesno, string? Greska)> IzmeniAsync(int id, IzmenaGalerijeSlikeDto dto)
    {
        var slika = await _galerijaRepository.DobaviPoIdAsync(id);
        if (slika == null) return (false, "Slika ne postoji.");

        if (string.IsNullOrWhiteSpace(dto.Grupa)) return (false, "Oblast je obavezna.");

        slika.Naslov = dto.Naslov;
        slika.Opis = dto.Opis;
        slika.Grupa = dto.Grupa.Trim();
        slika.Redosled = dto.Redosled;
        slika.Aktivan = dto.Aktivan;

        _galerijaRepository.Azuriraj(slika);
        await _galerijaRepository.SacuvajPromeneAsync();
        return (true, null);
    }

    public async Task<(bool Uspesno, string? Greska)> ObrisiAsync(int id)
    {
        var slika = await _galerijaRepository.DobaviPoIdAsync(id);
        if (slika == null) return (false, "Slika ne postoji.");
        if (!slika.Aktivan) return (false, "Slika je već uklonjena sa sajta.");

        slika.Aktivan = false;

        _galerijaRepository.Azuriraj(slika);
        await _galerijaRepository.SacuvajPromeneAsync();
        return (true, null);
    }

    private static GalerijaSlikaDto Mapiraj(GalerijaSlika g) => new()
    {
        Id = g.Id,
        SlikaUrl = g.SlikaUrl,
        Naslov = g.Naslov,
        Opis = g.Opis,
        Grupa = g.Grupa,
        Redosled = g.Redosled,
        Aktivan = g.Aktivan,
        DatumDodavanja = g.DatumDodavanja
    };
}
