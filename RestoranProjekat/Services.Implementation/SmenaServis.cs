using Domain.Entiteti;
using Repository.Interfejsi;
using Services.DTO;

namespace Services.Implementation;

public class SmenaServis : ISmenaServis
{
    private readonly ISmenaRepository _smenaRepository;
    private readonly IZaposleniRepository _zaposleniRepository;

    public SmenaServis(ISmenaRepository smenaRepository, IZaposleniRepository zaposleniRepository)
    {
        _smenaRepository = smenaRepository;
        _zaposleniRepository = zaposleniRepository;
    }

    public async Task<(bool Uspesno, string? Greska, List<SmenaDto>? Smene)> MojRasporedAsync(
        string trenutniKorisnikId, DateOnly? nedelja)
    {
        var zaposleni = await _zaposleniRepository.DobaviPoKorisnikIdAsync(trenutniKorisnikId);
        if (zaposleni == null) return (false, "Nalog nije povezan sa zaposlenim.", null);

        DateOnly? od = null, do_ = null;
        if (nedelja.HasValue)
        {
            od = PocetakNedelje(nedelja.Value);
            do_ = od.Value.AddDays(6);
        }

        var smene = await _smenaRepository.ListirajZaZaposlenogAsync(zaposleni.Id, od, do_);
        return (true, null, smene.Select(Mapiraj).ToList());
    }

    public async Task<RasporedNedeljeDto> RasporedNedeljeAsync(DateOnly? nedelja)
    {
        var pocetak = PocetakNedelje(nedelja ?? DanasnjiDan);
        var kraj = pocetak.AddDays(6);

        var smene = await _smenaRepository.ListirajUOpseguAsync(pocetak, kraj);

        return new RasporedNedeljeDto
        {
            PocetakNedelje = pocetak,
            KrajNedelje = kraj,
            Zaposleni = smene
                .GroupBy(s => s.ZaposleniId)
                .Select(grupa => new RasporedZaposlenogDto
                {
                    ZaposleniId = grupa.Key,
                    ImeZaposlenog = ImeZaposlenog(grupa.First()),
                    Smene = grupa.Select(Mapiraj).ToList(),
                    UkupnoSati = grupa.Sum(TrajanjeSati)
                })
                .OrderBy(z => z.ImeZaposlenog)
                .ToList()
        };
    }

    public async Task<(bool Uspesno, string? Greska, SmenaDto? Smena)> KreirajAsync(KreirajSmenuDto dto)
    {
        var zaposleni = await _zaposleniRepository.DobaviSaKorisnikomAsync(dto.ZaposleniId);
        if (zaposleni == null) return (false, "Zaposleni ne postoji.", null);
        if (!zaposleni.Korisnik.Aktivan) return (false, "Zaposleni je deaktiviran i ne može se rasporediti.", null);

        var greska = await ProveriPreklapanjeAsync(dto.ZaposleniId, dto.Datum, dto.VremePocetka, dto.VremeKraja);
        if (greska != null) return (false, greska, null);

        var smena = new Smena
        {
            ZaposleniId = dto.ZaposleniId,
            Datum = dto.Datum,
            VremePocetka = dto.VremePocetka,
            VremeKraja = dto.VremeKraja
        };

        await _smenaRepository.DodajAsync(smena);
        await _smenaRepository.SacuvajPromeneAsync();

        smena.Zaposleni = zaposleni;
        return (true, null, Mapiraj(smena));
    }

    public async Task<(bool Uspesno, string? Greska)> IzmeniAsync(int id, IzmenaSmeneDto dto)
    {
        var smena = await _smenaRepository.DobaviPoIdAsync(id);
        if (smena == null) return (false, "Smena ne postoji.");

        var greska = await ProveriPreklapanjeAsync(smena.ZaposleniId, dto.Datum, dto.VremePocetka, dto.VremeKraja, id);
        if (greska != null) return (false, greska);

        smena.Datum = dto.Datum;
        smena.VremePocetka = dto.VremePocetka;
        smena.VremeKraja = dto.VremeKraja;

        _smenaRepository.Azuriraj(smena);
        await _smenaRepository.SacuvajPromeneAsync();
        return (true, null);
    }

    public async Task<(bool Uspesno, string? Greska)> ObrisiAsync(int id)
    {
        var smena = await _smenaRepository.DobaviPoIdAsync(id);
        if (smena == null) return (false, "Smena ne postoji.");

        if (Interval(smena).Pocetak <= DateTime.Now)
            return (false, "Smena koja je već počela ne može se ukloniti iz rasporeda.");

        _smenaRepository.Obrisi(smena);
        await _smenaRepository.SacuvajPromeneAsync();
        return (true, null);
    }

    private async Task<string?> ProveriPreklapanjeAsync(
        int zaposleniId, DateOnly datum, TimeOnly pocetak, TimeOnly kraj, int? iskljuciSmenuId = null)
    {
        var (noviPocetak, noviKraj) = Interval(datum, pocetak, kraj);

        var kandidati = await _smenaRepository.ListirajZaProveruPreklapanjaAsync(zaposleniId, datum, iskljuciSmenuId);

        foreach (var postojeca in kandidati)
        {
            var (p, k) = Interval(postojeca);
            if (noviPocetak < k && p < noviKraj)
                return $"Zaposleni već ima smenu {postojeca.Datum:dd.MM.yyyy} od {postojeca.VremePocetka:HH\\:mm} do {postojeca.VremeKraja:HH\\:mm} koja se preklapa sa ovom.";
        }

        return null;
    }

    private static DateOnly PocetakNedelje(DateOnly datum) =>
        datum.AddDays(-(((int)datum.DayOfWeek + 6) % 7));

    private static DateOnly DanasnjiDan => DateOnly.FromDateTime(DateTime.Now);

    private static (DateTime Pocetak, DateTime Kraj) Interval(Smena smena) =>
        Interval(smena.Datum, smena.VremePocetka, smena.VremeKraja);

    private static (DateTime Pocetak, DateTime Kraj) Interval(DateOnly datum, TimeOnly pocetak, TimeOnly kraj)
    {
        var od = datum.ToDateTime(pocetak);
        var do_ = datum.ToDateTime(kraj);
        if (kraj <= pocetak) do_ = do_.AddDays(1);
        return (od, do_);
    }

    private static decimal TrajanjeSati(Smena smena)
    {
        var (od, do_) = Interval(smena);
        return Math.Round((decimal)(do_ - od).TotalHours, 2);
    }

    private static string ImeZaposlenog(Smena smena) =>
        smena.Zaposleni?.Korisnik == null
            ? string.Empty
            : $"{smena.Zaposleni.Korisnik.Ime} {smena.Zaposleni.Korisnik.Prezime}";

    private static SmenaDto Mapiraj(Smena s) => new()
    {
        Id = s.Id,
        ZaposleniId = s.ZaposleniId,
        ImeZaposlenog = ImeZaposlenog(s),
        Datum = s.Datum,
        VremePocetka = s.VremePocetka,
        VremeKraja = s.VremeKraja,
        TrajanjeSati = TrajanjeSati(s),
        PrelaziPonoc = s.VremeKraja <= s.VremePocetka,
        Prosla = Interval(s).Kraj < DateTime.Now
    };
}
