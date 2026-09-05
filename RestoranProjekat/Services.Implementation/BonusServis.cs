using Domain.Entiteti;
using Repository.Interfejsi;
using Services.DTO;

namespace Services.Implementation;

public class BonusServis : IBonusServis
{
    private readonly IBonusRepository _bonusRepository;
    private readonly IZaposleniRepository _zaposleniRepository;

    public BonusServis(IBonusRepository bonusRepository, IZaposleniRepository zaposleniRepository)
    {
        _bonusRepository = bonusRepository;
        _zaposleniRepository = zaposleniRepository;
    }

    public async Task<(bool Uspesno, string? Greska, List<BonusDto>? Bonusi)> MojiBonusiAsync(string trenutniKorisnikId)
    {
        var zaposleni = await _zaposleniRepository.DobaviPoKorisnikIdAsync(trenutniKorisnikId);
        if (zaposleni == null) return (false, "Nalog nije povezan sa zaposlenim.", null);

        var bonusi = await _bonusRepository.ListirajZaZaposlenogAsync(zaposleni.Id);
        return (true, null, bonusi.Select(Mapiraj).ToList());
    }

    public async Task<PaginiranaListaDto<BonusDto>> PretraziAsync(BonusiPretragaDto filter)
    {
        var (podaci, ukupno) = await _bonusRepository.PretraziAsync(
            filter.ZaposleniId, filter.DatumOd, filter.DatumDo, filter.Strana, filter.VelicinaStrane);

        return new PaginiranaListaDto<BonusDto>
        {
            Podaci = podaci.Select(Mapiraj).ToList(),
            UkupnoZapisa = ukupno,
            TrenutnaStrana = filter.Strana,
            UkupnoStrana = (int)Math.Ceiling(ukupno / (double)filter.VelicinaStrane)
        };
    }

    public async Task<(bool Uspesno, string? Greska, List<BonusDto>? Bonusi)> DodeliAsync(
        string trenutniKorisnikId, KreirajBonusDto dto)
    {
        var menadzer = await _zaposleniRepository.DobaviPoKorisnikIdAsync(trenutniKorisnikId);
        if (menadzer == null) return (false, "Nalog nije povezan sa zaposlenim.", null);

        menadzer = await _zaposleniRepository.DobaviSaKorisnikomAsync(menadzer.Id) ?? menadzer;

        var primaoci = new List<Zaposleni>();
        foreach (var zaposleniId in dto.ZaposleniIds)
        {
            var primalac = await _zaposleniRepository.DobaviSaKorisnikomAsync(zaposleniId);
            if (primalac == null)
                return (false, $"Zaposleni sa id {zaposleniId} ne postoji.", null);
            if (!primalac.Korisnik.Aktivan)
                return (false, $"{primalac.Korisnik.Ime} {primalac.Korisnik.Prezime} je deaktiviran i ne može dobiti bonus.", null);

            primaoci.Add(primalac);
        }

        var bonusi = new List<Bonus>();
        foreach (var primalac in primaoci)
        {
            var bonus = new Bonus
            {
                ZaposleniId = primalac.Id,
                Iznos = dto.Iznos,
                DatumPocetka = dto.DatumPocetka,
                DatumKraja = dto.DatumKraja,
                Razlog = dto.Razlog,
                DodelioZaposleniId = menadzer.Id,
                DatumDodele = DateTime.UtcNow
            };

            await _bonusRepository.DodajAsync(bonus);
            bonus.Zaposleni = primalac;
            bonus.DodelioZaposleni = menadzer;
            bonusi.Add(bonus);
        }

        await _bonusRepository.SacuvajPromeneAsync();

        return (true, null, bonusi.Select(Mapiraj).ToList());
    }

    private static string ImeIz(Zaposleni? zaposleni) =>
        zaposleni?.Korisnik == null ? string.Empty : $"{zaposleni.Korisnik.Ime} {zaposleni.Korisnik.Prezime}";

    private static BonusDto Mapiraj(Bonus b)
    {
        var danas = DateOnly.FromDateTime(DateTime.Now);

        return new BonusDto
        {
            Id = b.Id,
            ZaposleniId = b.ZaposleniId,
            ImeZaposlenog = ImeIz(b.Zaposleni),
            Iznos = b.Iznos,
            DatumPocetka = b.DatumPocetka,
            DatumKraja = b.DatumKraja,
            Razlog = b.Razlog,
            DodelioZaposleniId = b.DodelioZaposleniId,
            DodelioIme = ImeIz(b.DodelioZaposleni),
            DatumDodele = b.DatumDodele,
            Vazi = danas >= b.DatumPocetka && danas <= b.DatumKraja
        };
    }
}
