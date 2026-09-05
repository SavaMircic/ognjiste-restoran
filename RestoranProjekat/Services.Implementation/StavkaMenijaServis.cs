using Domain.Entiteti;
using Repository.Interfejsi;
using Services.DTO;

namespace Services.Implementation;

public class StavkaMenijaServis : IStavkaMenijaServis
{
    private const int MaksDodatnihSlika = 6;

    private readonly IStavkaMenijaRepository _stavkaRepository;
    private readonly IKategorijaMenijaRepository _kategorijaRepository;
    private readonly IStavkaPorudzbineRepository _stavkaPorudzbineRepository;
    private readonly IRecenzijaRepository _recenzijaRepository;
    private readonly ILajkJelaRepository _lajkJelaRepository;
    private readonly ISkladisteSlika _skladisteSlika;
    private readonly IRepository<SlikaStavkeMenija> _slikeRepository;

    public StavkaMenijaServis(
        IStavkaMenijaRepository stavkaRepository, IKategorijaMenijaRepository kategorijaRepository,
        IStavkaPorudzbineRepository stavkaPorudzbineRepository, IRecenzijaRepository recenzijaRepository,
        ILajkJelaRepository lajkJelaRepository, ISkladisteSlika skladisteSlika,
        IRepository<SlikaStavkeMenija> slikeRepository)
    {
        _stavkaRepository = stavkaRepository;
        _kategorijaRepository = kategorijaRepository;
        _stavkaPorudzbineRepository = stavkaPorudzbineRepository;
        _recenzijaRepository = recenzijaRepository;
        _lajkJelaRepository = lajkJelaRepository;
        _skladisteSlika = skladisteSlika;
        _slikeRepository = slikeRepository;
    }

    public async Task<(bool Uspesno, string? Greska, string? SlikaUrl)> PostaviGlavnuSlikuAsync(
        int id, DatotekaZaUploadDto datoteka)
    {
        var stavka = await _stavkaRepository.DobaviPoIdAsync(id);
        if (stavka == null) return (false, "Stavka menija ne postoji.", null);

        var (uspesno, greska, url) = await _skladisteSlika.SacuvajAsync(datoteka, ISkladisteSlika.Podfolderi.Jela);
        if (!uspesno) return (false, greska, null);

        var stara = stavka.SlikaUrl;
        stavka.SlikaUrl = url!;
        _stavkaRepository.Azuriraj(stavka);
        await _stavkaRepository.SacuvajPromeneAsync();

        await _skladisteSlika.ObrisiAsync(stara);

        return (true, null, url);
    }

    public async Task<(bool Uspesno, string? Greska, SlikaStavkeMenijaDto? Slika)> DodajDodatnuSlikuAsync(
        int id, DatotekaZaUploadDto datoteka, string? opis)
    {
        var stavka = await _stavkaRepository.DobaviPoIdAsync(id);
        if (stavka == null) return (false, "Stavka menija ne postoji.", null);

        var postojece = await _stavkaRepository.ListirajDodatneSlikeAsync(id);
        if (postojece.Count >= MaksDodatnihSlika)
            return (false, $"Jelo može imati najviše {MaksDodatnihSlika} dodatnih slika.", null);

        var (uspesno, greska, url) = await _skladisteSlika.SacuvajAsync(datoteka, ISkladisteSlika.Podfolderi.Jela);
        if (!uspesno) return (false, greska, null);

        var slika = new SlikaStavkeMenija
        {
            StavkaMenijaId = id,
            SlikaUrl = url!,
            Opis = opis,
            Redosled = await _stavkaRepository.NajveciRedosledSlikeAsync(id) + 1,
            DatumDodavanja = DateTime.UtcNow
        };

        await _slikeRepository.DodajAsync(slika);
        await _slikeRepository.SacuvajPromeneAsync();

        return (true, null, MapirajSliku(slika));
    }

    public async Task<List<SlikaStavkeMenijaDto>> ListirajDodatneSlikeAsync(int id) =>
        (await _stavkaRepository.ListirajDodatneSlikeAsync(id)).Select(MapirajSliku).ToList();

    public async Task<(bool Uspesno, string? Greska)> ObrisiDodatnuSlikuAsync(int id, int slikaId)
    {
        var slika = await _stavkaRepository.DobaviDodatnuSlikuAsync(id, slikaId);
        if (slika == null) return (false, "Slika ne postoji za ovo jelo.");

        var url = slika.SlikaUrl;
        _slikeRepository.Obrisi(slika);
        await _slikeRepository.SacuvajPromeneAsync();

        await _skladisteSlika.ObrisiAsync(url);
        return (true, null);
    }

    public async Task<(bool Uspesno, string? Greska)> PoredjajDodatneSlikeAsync(int id, RedosledSlikaDto dto)
    {
        var slike = await _stavkaRepository.ListirajDodatneSlikeAsync(id);
        if (slike.Count == 0) return (false, "Jelo nema dodatnih slika.");

        var postojeciIds = slike.Select(s => s.Id).OrderBy(x => x).ToList();
        if (!dto.IdRedom.OrderBy(x => x).SequenceEqual(postojeciIds))
            return (false, "Lista mora da sadrži tačno sve id-jeve slika ovog jela, bez izostavljanja.");

        for (var i = 0; i < dto.IdRedom.Count; i++)
        {
            var slika = slike.First(s => s.Id == dto.IdRedom[i]);
            slika.Redosled = i + 1;
            _slikeRepository.Azuriraj(slika);
        }

        await _slikeRepository.SacuvajPromeneAsync();
        return (true, null);
    }

    private static SlikaStavkeMenijaDto MapirajSliku(SlikaStavkeMenija s) => new()
    {
        Id = s.Id,
        SlikaUrl = s.SlikaUrl,
        Opis = s.Opis,
        Redosled = s.Redosled
    };

    public async Task<PaginiranaListaDto<StavkaMenijaDto>> PretraziAsync(StavkeMenijaPretragaDto filter)
    {
        var (podaci, ukupno) = await _stavkaRepository.PretraziAsync(
            filter.KategorijaId, filter.Pretraga, filter.Sortiranje, filter.Strana, filter.VelicinaStrane);

        var ids = podaci.Select(s => s.Id).ToList();
        var statistika = await _recenzijaRepository.StatistikaZaStavkeAsync(ids);
        var lajkovi = await _lajkJelaRepository.BrojLajkovaZaStavkeAsync(ids);

        return new PaginiranaListaDto<StavkaMenijaDto>
        {
            Podaci = podaci.Select(s => Mapiraj(s, statistika, lajkovi)).ToList(),
            UkupnoZapisa = ukupno,
            TrenutnaStrana = filter.Strana,
            UkupnoStrana = (int)Math.Ceiling(ukupno / (double)filter.VelicinaStrane)
        };
    }

    public async Task<StavkaMenijaDto?> DobaviDetaljAsync(int id)
    {
        var stavka = await _stavkaRepository.DobaviDetaljAsync(id);
        if (stavka == null) return null;

        var statistika = await _recenzijaRepository.StatistikaZaStavkeAsync(new[] { id });
        var lajkovi = await _lajkJelaRepository.BrojLajkovaZaStavkeAsync(new[] { id });
        return Mapiraj(stavka, statistika, lajkovi, detaljno: true);
    }

    public async Task<(bool Uspesno, string? Greska, int? Id)> KreirajAsync(StavkaMenijaUlazDto dto)
    {
        if (await _kategorijaRepository.DobaviPoIdAsync(dto.KategorijaId) == null)
            return (false, "Kategorija ne postoji.", null);

        var stavka = new StavkaMenija
        {
            Naziv = dto.Naziv,
            Opis = dto.Opis,
            DetaljanOpis = string.IsNullOrWhiteSpace(dto.DetaljanOpis) ? null : dto.DetaljanOpis.Trim(),
            Cena = dto.Cena,
            SlikaUrl = ISkladisteSlika.RezervnaSlikaJela,
            KategorijaId = dto.KategorijaId,
            Popust = dto.Popust,
            Dostupno = true,
            DatumKreiranja = DateTime.UtcNow
        };

        await _stavkaRepository.DodajAsync(stavka);
        await _stavkaRepository.SacuvajPromeneAsync();

        return (true, null, stavka.Id);
    }

    public async Task<(bool Uspesno, string? Greska)> IzmeniAsync(int id, StavkaMenijaUlazDto dto)
    {
        var stavka = await _stavkaRepository.DobaviPoIdAsync(id);
        if (stavka == null) return (false, "Stavka ne postoji.");

        if (await _kategorijaRepository.DobaviPoIdAsync(dto.KategorijaId) == null)
            return (false, "Kategorija ne postoji.");

        stavka.Naziv = dto.Naziv;
        stavka.Opis = dto.Opis;
        stavka.DetaljanOpis = string.IsNullOrWhiteSpace(dto.DetaljanOpis) ? null : dto.DetaljanOpis.Trim();
        stavka.Cena = dto.Cena;
        stavka.KategorijaId = dto.KategorijaId;
        stavka.Popust = dto.Popust;

        _stavkaRepository.Azuriraj(stavka);
        await _stavkaRepository.SacuvajPromeneAsync();
        return (true, null);
    }

    public async Task<(bool Uspesno, string? Greska)> PromeniDostupnostAsync(int id, PromenaDostupnostiDto dto)
    {
        var stavka = await _stavkaRepository.DobaviPoIdAsync(id);
        if (stavka == null) return (false, "Stavka ne postoji.");

        stavka.Dostupno = dto.Dostupno;

        _stavkaRepository.Azuriraj(stavka);
        await _stavkaRepository.SacuvajPromeneAsync();
        return (true, null);
    }

    public async Task<(bool Uspesno, string? Greska)> ObrisiAsync(int id)
    {
        var stavka = await _stavkaRepository.DobaviPoIdAsync(id);
        if (stavka == null) return (false, "Stavka ne postoji.");

        if (await _stavkaPorudzbineRepository.IkadaNarucenaAsync(id))
            return (false, "Stavka je već naručivana i ne može se trajno obrisati — koristite izmenu dostupnosti.");

        var putanjeZaBrisanje = (await _stavkaRepository.ListirajDodatneSlikeAsync(id))
            .Select(s => s.SlikaUrl)
            .Append(stavka.SlikaUrl)
            .ToList();

        _stavkaRepository.Obrisi(stavka);
        await _stavkaRepository.SacuvajPromeneAsync();

        foreach (var putanja in putanjeZaBrisanje)
            await _skladisteSlika.ObrisiAsync(putanja);

        return (true, null);
    }

    internal static StavkaMenijaDto Mapiraj(
        StavkaMenija s, Dictionary<int, (double ProsecnaOcena, int BrojRecenzija)> statistika,
        Dictionary<int, int> lajkovi, bool detaljno = false)
    {
        var imaStatistiku = statistika.TryGetValue(s.Id, out var stat);
        return new StavkaMenijaDto
        {
            Id = s.Id,
            Naziv = s.Naziv,
            Opis = s.Opis,
            DetaljanOpis = detaljno ? s.DetaljanOpis : null,
            Cena = s.Cena,
            SlikaUrl = s.SlikaUrl,
            KategorijaId = s.KategorijaId,
            KategorijaNaziv = s.Kategorija.Naziv,
            Dostupno = s.Dostupno,
            Popust = s.Popust,
            CenaSaPopustom = s.CenaSaPopustom,
            DodatneSlike = s.DodatneSlike.OrderBy(sl => sl.Redosled).Select(MapirajSliku).ToList(),
            DatumKreiranja = s.DatumKreiranja,
            ProsecnaOcena = imaStatistiku ? Math.Round(stat.ProsecnaOcena, 1) : null,
            BrojRecenzija = imaStatistiku ? stat.BrojRecenzija : 0,
            BrojLajkova = lajkovi.TryGetValue(s.Id, out var broj) ? broj : 0
        };
    }
}
