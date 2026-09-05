using Domain.Entiteti;
using Domain.Enumi;
using Microsoft.AspNetCore.Identity;
using Repository.Interfejsi;
using Services.DTO;

namespace Services.Implementation;

public class PorukaServis : IPorukaServis
{
    private readonly IPorukaRepository _porukaRepository;
    private readonly IZaposleniRepository _zaposleniRepository;
    private readonly UserManager<Korisnik> _userManager;
    private readonly IEmailServis _emailServis;

    public PorukaServis(
        IPorukaRepository porukaRepository, IZaposleniRepository zaposleniRepository,
        UserManager<Korisnik> userManager, IEmailServis emailServis)
    {
        _porukaRepository = porukaRepository;
        _zaposleniRepository = zaposleniRepository;
        _userManager = userManager;
        _emailServis = emailServis;
    }

    public async Task<(bool Uspesno, string? Greska, PorukaDto? Poruka)> PosaljiAsync(string? korisnikId, PosaljiPorukuDto dto)
    {
        Korisnik? korisnik = null;

        if (korisnikId != null)
        {
            korisnik = await _userManager.FindByIdAsync(korisnikId);
            if (korisnik == null) return (false, "Korisnik ne postoji.", null);
        }
        else if (string.IsNullOrWhiteSpace(dto.Ime) || string.IsNullOrWhiteSpace(dto.Email) || string.IsNullOrWhiteSpace(dto.Telefon))
        {
            return (false, "Ime, email i telefon su obavezni kada poruku šalje gost bez naloga.", null);
        }

        var poruka = new Poruka
        {
            KorisnikId = korisnik?.Id,
            GostIme = korisnik == null ? dto.Ime : null,
            GostEmail = korisnik == null ? dto.Email : null,
            GostTelefon = korisnik == null ? dto.Telefon : null,
            Kategorija = dto.Kategorija,
            Tekst = dto.Tekst,
            DatumSlanja = DateTime.UtcNow,
            Status = StatusPoruke.Novo,
            ZeljeniDatumVreme = dto.Kategorija == KategorijaPoruke.Rezervacija ? dto.ZeljeniDatumVreme : null,
            ZeljeniBrojGostiju = dto.Kategorija == KategorijaPoruke.Rezervacija ? dto.ZeljeniBrojGostiju : null
        };

        await _porukaRepository.DodajAsync(poruka);
        await _porukaRepository.SacuvajPromeneAsync();

        poruka.Korisnik = korisnik;
        return (true, null, Mapiraj(poruka));
    }

    public async Task<List<PorukaDto>> MojePorukeAsync(string korisnikId)
    {
        var poruke = await _porukaRepository.ListirajZaKorisnikaAsync(korisnikId);
        var korisnik = await _userManager.FindByIdAsync(korisnikId);

        return poruke.Select(p =>
        {
            p.Korisnik = korisnik;
            return Mapiraj(p);
        }).ToList();
    }

    public async Task<PaginiranaListaDto<PorukaDto>> PretraziAsync(PorukePretragaDto filter)
    {
        var (podaci, ukupno) = await _porukaRepository.PretraziAsync(
            filter.Status, filter.Kategorija, filter.Strana, filter.VelicinaStrane);

        return new PaginiranaListaDto<PorukaDto>
        {
            Podaci = podaci.Select(Mapiraj).ToList(),
            UkupnoZapisa = ukupno,
            TrenutnaStrana = filter.Strana,
            UkupnoStrana = (int)Math.Ceiling(ukupno / (double)filter.VelicinaStrane)
        };
    }

    public async Task<(bool Uspesno, string? Greska)> OdgovoriAsync(int id, string adminKorisnikId, OdgovorNaPorukuDto dto)
    {
        var poruka = await _porukaRepository.DobaviDetaljAsync(id);
        if (poruka == null) return (false, "Poruka ne postoji.");

        var admin = await _zaposleniRepository.DobaviPoKorisnikIdAsync(adminKorisnikId);

        poruka.Odgovor = dto.Odgovor;
        poruka.DatumOdgovora = DateTime.UtcNow;
        poruka.Status = StatusPoruke.Odgovoreno;
        poruka.ObradioZaposleniId = admin?.Id;

        _porukaRepository.Azuriraj(poruka);
        await _porukaRepository.SacuvajPromeneAsync();

        var adresa = poruka.Korisnik?.Email ?? poruka.GostEmail;
        if (!string.IsNullOrWhiteSpace(adresa))
        {
            await _emailServis.PosaljiBezPrekidaAsync(adresa!, "Odgovor na Vašu poruku",
                $"<p>Poštovani,</p><p>Vaša poruka:</p><blockquote>{poruka.Tekst}</blockquote><p>Naš odgovor:</p><blockquote>{dto.Odgovor}</blockquote><p>Srdačan pozdrav,<br/>Vaš restoran</p>");
        }

        return (true, null);
    }

    public async Task<(bool Uspesno, string? Greska)> OznaciProcitanomAsync(int id)
    {
        var poruka = await _porukaRepository.DobaviPoIdAsync(id);
        if (poruka == null) return (false, "Poruka ne postoji.");
        if (poruka.Status != StatusPoruke.Novo) return (false, "Poruka je već obrađena.");

        poruka.Status = StatusPoruke.Procitano;
        _porukaRepository.Azuriraj(poruka);
        await _porukaRepository.SacuvajPromeneAsync();
        return (true, null);
    }

    private static PorukaDto Mapiraj(Poruka p) => new()
    {
        Id = p.Id,
        KorisnikId = p.KorisnikId,
        PosiljalacIme = p.Korisnik != null ? $"{p.Korisnik.Ime} {p.Korisnik.Prezime}" : p.GostIme ?? "Gost",
        PosiljalacEmail = p.Korisnik?.Email ?? p.GostEmail,
        PosiljalacTelefon = p.Korisnik?.PhoneNumber ?? p.GostTelefon,
        Kategorija = p.Kategorija,
        Tekst = p.Tekst,
        DatumSlanja = p.DatumSlanja,
        Status = p.Status,
        Odgovor = p.Odgovor,
        DatumOdgovora = p.DatumOdgovora,
        ZeljeniDatumVreme = p.ZeljeniDatumVreme,
        ZeljeniBrojGostiju = p.ZeljeniBrojGostiju,
        Prioritetna = p.Kategorija == KategorijaPoruke.Rezervacija
    };
}
