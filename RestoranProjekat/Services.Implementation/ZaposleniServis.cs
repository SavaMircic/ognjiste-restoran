using Domain.Entiteti;
using Domain.Konstante;
using Microsoft.AspNetCore.Identity;
using Repository.Interfejsi;
using Services.DTO;

namespace Services.Implementation;

public class ZaposleniServis : IZaposleniServis
{
    private readonly UserManager<Korisnik> _userManager;
    private readonly IZaposleniRepository _zaposleniRepository;
    private readonly ISkladisteSlika _skladisteSlika;

    public ZaposleniServis(
        UserManager<Korisnik> userManager, IZaposleniRepository zaposleniRepository,
        ISkladisteSlika skladisteSlika)
    {
        _userManager = userManager;
        _zaposleniRepository = zaposleniRepository;
        _skladisteSlika = skladisteSlika;
    }

    public async Task<List<ClanTimaDto>> ListirajTimAsync()
    {
        var osoblje = await _zaposleniRepository.ListirajZaSajtAsync();

        return osoblje.Select(z => new ClanTimaDto
        {
            Ime = z.Ime,
            Prezime = z.Prezime,
            Uloga = z.Uloga,
            SlikaUrl = z.SlikaUrl,
            Biografija = z.Biografija,
            LinkedInUrl = z.LinkedInUrl,
            InstagramUrl = z.InstagramUrl
        }).ToList();
    }

    public async Task<(bool Uspesno, string? Greska)> IzmeniProfilNaSajtuAsync(
        int zaposleniId, IzmenaProfilaNaSajtuDto dto)
    {
        var zaposleni = await _zaposleniRepository.DobaviPoIdAsync(zaposleniId);
        if (zaposleni == null) return (false, "Zaposleni ne postoji.");

        zaposleni.Biografija = dto.Biografija;
        zaposleni.LinkedInUrl = dto.LinkedInUrl;
        zaposleni.InstagramUrl = dto.InstagramUrl;
        zaposleni.Redosled = dto.Redosled;
        zaposleni.PrikaziNaSajtu = dto.PrikaziNaSajtu;

        _zaposleniRepository.Azuriraj(zaposleni);
        await _zaposleniRepository.SacuvajPromeneAsync();
        return (true, null);
    }

    public async Task<(bool Uspesno, string? Greska, string? SlikaUrl)> PostaviSlikuAsync(
        int zaposleniId, DatotekaZaUploadDto datoteka)
    {
        var zaposleni = await _zaposleniRepository.DobaviPoIdAsync(zaposleniId);
        if (zaposleni == null) return (false, "Zaposleni ne postoji.", null);

        var (uspesno, greska, url) = await _skladisteSlika.SacuvajAsync(
            datoteka, ISkladisteSlika.Podfolderi.Zaposleni);
        if (!uspesno) return (false, greska, null);

        var stara = zaposleni.SlikaUrl;
        zaposleni.SlikaUrl = url;
        _zaposleniRepository.Azuriraj(zaposleni);
        await _zaposleniRepository.SacuvajPromeneAsync();

        await _skladisteSlika.ObrisiAsync(stara);
        return (true, null, url);
    }

    public async Task<PaginiranaListaDto<ZaposleniDto>> PretraziAsync(ZaposleniPretragaDto filter)
    {
        var (podaci, ukupno) = await _zaposleniRepository.PretraziAsync(
            filter.Pretraga, filter.Uloga, filter.Aktivan, filter.Strana, filter.VelicinaStrane);

        return new PaginiranaListaDto<ZaposleniDto>
        {
            Podaci = podaci.Select(z => new ZaposleniDto
            {
                Id = z.Id,
                KorisnikId = z.KorisnikId,
                Ime = z.Ime,
                Prezime = z.Prezime,
                Email = z.Email,
                Uloga = z.Uloga,
                DatumZaposlenja = z.DatumZaposlenja,
                Aktivan = z.Aktivan,
                SlikaUrl = z.SlikaUrl,
                Biografija = z.Biografija,
                LinkedInUrl = z.LinkedInUrl,
                InstagramUrl = z.InstagramUrl,
                Redosled = z.Redosled,
                PrikaziNaSajtu = z.PrikaziNaSajtu
            }).ToList(),
            UkupnoZapisa = ukupno,
            TrenutnaStrana = filter.Strana,
            UkupnoStrana = (int)Math.Ceiling(ukupno / (double)filter.VelicinaStrane)
        };
    }

    public async Task<(bool Uspesno, string? Greska, int? ZaposleniId)> KreirajAsync(KreirajZaposlenogDto dto)
    {
        var postojeci = await _userManager.FindByEmailAsync(dto.Email);
        if (postojeci != null)
            return (false, "Nalog sa ovom email adresom već postoji.", null);

        var korisnik = new Korisnik
        {
            UserName = dto.Email,
            Email = dto.Email,
            Ime = dto.Ime,
            Prezime = dto.Prezime,
            DatumRegistracije = DateTime.UtcNow,
            Aktivan = true,
            EmailConfirmed = true
        };

        var rezultat = await _userManager.CreateAsync(korisnik, dto.PrivremenaLozinka);
        if (!rezultat.Succeeded)
            return (false, string.Join(" ", rezultat.Errors.Select(e => e.Description)), null);

        await _userManager.AddToRoleAsync(korisnik, dto.Uloga);

        var zaposleni = new Zaposleni { KorisnikId = korisnik.Id, DatumZaposlenja = dto.DatumZaposlenja };
        await _zaposleniRepository.DodajAsync(zaposleni);
        await _zaposleniRepository.SacuvajPromeneAsync();

        return (true, null, zaposleni.Id);
    }

    public async Task<(bool Uspesno, string? Greska)> IzmeniAsync(int zaposleniId, IzmenaZaposlenogDto dto)
    {
        var zaposleni = await _zaposleniRepository.DobaviPoIdAsync(zaposleniId);
        if (zaposleni == null) return (false, "Zaposleni ne postoji.");

        var korisnik = await _userManager.FindByIdAsync(zaposleni.KorisnikId);
        if (korisnik == null) return (false, "Korisnik ne postoji.");

        korisnik.Ime = dto.Ime;
        korisnik.Prezime = dto.Prezime;

        var trenutneUloge = await _userManager.GetRolesAsync(korisnik);
        var trenutnaPozicija = trenutneUloge.Where(u => Uloge.Osoblje.Contains(u)).ToList();
        if (!trenutnaPozicija.Contains(dto.Uloga))
        {
            await _userManager.RemoveFromRolesAsync(korisnik, trenutnaPozicija);
            await _userManager.AddToRoleAsync(korisnik, dto.Uloga);
        }

        var rezultat = await _userManager.UpdateAsync(korisnik);
        return rezultat.Succeeded ? (true, null) : (false, string.Join(" ", rezultat.Errors.Select(e => e.Description)));
    }

    public async Task<(bool Uspesno, string? Greska)> DeaktivirajAsync(int zaposleniId)
    {
        var zaposleni = await _zaposleniRepository.DobaviPoIdAsync(zaposleniId);
        if (zaposleni == null) return (false, "Zaposleni ne postoji.");

        var korisnik = await _userManager.FindByIdAsync(zaposleni.KorisnikId);
        if (korisnik == null) return (false, "Korisnik ne postoji.");

        korisnik.Aktivan = false;

        var rezultat = await _userManager.UpdateAsync(korisnik);
        return rezultat.Succeeded ? (true, null) : (false, string.Join(" ", rezultat.Errors.Select(e => e.Description)));
    }
}
