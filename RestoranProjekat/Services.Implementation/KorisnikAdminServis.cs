using Domain.Entiteti;
using Domain.Konstante;
using Microsoft.AspNetCore.Identity;
using Repository.Interfejsi;
using Services.DTO;

namespace Services.Implementation;

public class KorisnikAdminServis : IKorisnikAdminServis
{
    private readonly UserManager<Korisnik> _userManager;
    private readonly IKorisnikAdminRepository _korisnikAdminRepository;
    private readonly IAdminAkcijaServis _adminAkcijaServis;
    private readonly IRecenzijaRepository _recenzijaRepository;
    private readonly IRezervacijaRepository _rezervacijaRepository;
    private readonly IPorukaRepository _porukaRepository;
    private readonly IPorudzbinaRepository _porudzbinaRepository;

    public KorisnikAdminServis(
        UserManager<Korisnik> userManager, IKorisnikAdminRepository korisnikAdminRepository,
        IAdminAkcijaServis adminAkcijaServis, IRecenzijaRepository recenzijaRepository,
        IRezervacijaRepository rezervacijaRepository, IPorukaRepository porukaRepository,
        IPorudzbinaRepository porudzbinaRepository)
    {
        _userManager = userManager;
        _korisnikAdminRepository = korisnikAdminRepository;
        _adminAkcijaServis = adminAkcijaServis;
        _recenzijaRepository = recenzijaRepository;
        _rezervacijaRepository = rezervacijaRepository;
        _porukaRepository = porukaRepository;
        _porudzbinaRepository = porudzbinaRepository;
    }

    public async Task<PaginiranaListaDto<KorisnikAdminDto>> PretraziAsync(KorisniciPretragaDto filter)
    {
        var (podaci, ukupno) = await _korisnikAdminRepository.PretraziAsync(
            filter.Pretraga, filter.Blokiran, filter.Strana, filter.VelicinaStrane);

        var stavke = new List<KorisnikAdminDto>();
        foreach (var korisnik in podaci)
            stavke.Add(await MapirajAsync(korisnik));

        return new PaginiranaListaDto<KorisnikAdminDto>
        {
            Podaci = stavke,
            UkupnoZapisa = ukupno,
            TrenutnaStrana = filter.Strana,
            UkupnoStrana = (int)Math.Ceiling(ukupno / (double)filter.VelicinaStrane)
        };
    }

    private const int MaksZapisaPoKategoriji = 50;

    public async Task<KorisnikDetaljDto?> DobaviDetaljAsync(string korisnikId)
    {
        var korisnik = await _userManager.FindByIdAsync(korisnikId);
        if (korisnik == null) return null;

        var recenzije = await _recenzijaRepository.ListirajZaKorisnikaAsync(korisnikId, MaksZapisaPoKategoriji);
        var rezervacije = await _rezervacijaRepository.ListirajZaKorisnikaAsync(korisnikId);
        var poruke = await _porukaRepository.ListirajZaKorisnikaAsync(korisnikId);
        var racuni = await _porudzbinaRepository.RacuniGostaAsync(korisnikId, MaksZapisaPoKategoriji);
        var pouzdanost = await _rezervacijaRepository.PouzdanostGostaAsync(korisnikId);

        var odrzane = pouzdanost.Realizovane + pouzdanost.Istekle;

        return new KorisnikDetaljDto
        {
            Profil = await MapirajAsync(korisnik),
            Recenzije = recenzije.Select(r => new RecenzijaPregledDto
            {
                Id = r.Id,
                TipRecenzije = r.TipRecenzije,
                NazivStavke = r.StavkaMenija?.Naziv,
                Ocena = r.Ocena,
                Naslov = r.Naslov,
                DatumKreiranja = r.DatumKreiranja,
                Aktivan = r.Aktivan
            }).ToList(),
            Rezervacije = rezervacije.Take(MaksZapisaPoKategoriji).Select(r => new RezervacijaPregledDto
            {
                Id = r.Id,
                KodRezervacije = r.KodRezervacije,
                DatumVreme = r.DatumVreme,
                BrojGostiju = r.BrojGostiju,
                Status = r.Status
            }).ToList(),
            Poruke = poruke.Take(MaksZapisaPoKategoriji).Select(p => new PorukaPregledDto
            {
                Id = p.Id,
                Kategorija = p.Kategorija,
                Status = p.Status,
                DatumSlanja = p.DatumSlanja
            }).ToList(),
            Racuni = racuni.Select(r => new RacunGostaDto
            {
                PorudzbinaId = r.PorudzbinaId,
                KodRezervacije = r.KodRezervacije,
                VremeZatvaranja = r.VremeZatvaranja,
                BrojStola = r.BrojStola,
                Ukupno = r.Ukupno,
                Napojnica = r.Napojnica
            }).ToList(),
            Pouzdanost = new PouzdanostGostaDto
            {
                Realizovane = pouzdanost.Realizovane,
                Istekle = pouzdanost.Istekle,
                Otkazane = pouzdanost.Otkazane,
                Aktivne = pouzdanost.Aktivne,
                ProcenatPojavljivanja = odrzane == 0
                    ? null
                    : Math.Round(pouzdanost.Realizovane * 100.0 / odrzane, 1)
            },
            UkupnoPotroseno = racuni.Sum(r => r.Ukupno),
            UkupnaNapojnica = racuni.Sum(r => r.Napojnica)
        };
    }

    public async Task<(bool Uspesno, string? Greska)> BlokirajAsync(
        string korisnikId, string adminKorisnikId, BlokirajKorisnikaDto dto)
    {
        var korisnik = await _userManager.FindByIdAsync(korisnikId);
        if (korisnik == null) return (false, "Korisnik ne postoji.");

        if (dto.DatumDo.HasValue)
        {
            korisnik.BlokiranDo = dto.DatumDo;
        }
        else
        {
            korisnik.Aktivan = false;
            korisnik.BlokiranDo = null;
        }

        korisnik.RefreshToken = null;
        korisnik.RefreshTokenIstice = null;

        var rezultat = await _userManager.UpdateAsync(korisnik);
        if (!rezultat.Succeeded)
            return (false, string.Join(" ", rezultat.Errors.Select(e => e.Description)));

        var trajanje = dto.DatumDo.HasValue ? $"do {dto.DatumDo:dd.MM.yyyy HH:mm}" : "trajno";
        await _adminAkcijaServis.ZabeleziAsync(
            adminKorisnikId, TipoviAdminAkcija.Blokiranje, korisnikId,
            $"Blokiran ({trajanje}). Razlog: {dto.Razlog}");

        return (true, null);
    }

    public async Task<(bool Uspesno, string? Greska)> OdblokirajAsync(string korisnikId, string adminKorisnikId)
    {
        var korisnik = await _userManager.FindByIdAsync(korisnikId);
        if (korisnik == null) return (false, "Korisnik ne postoji.");

        korisnik.Aktivan = true;
        korisnik.BlokiranDo = null;

        var rezultat = await _userManager.UpdateAsync(korisnik);
        if (!rezultat.Succeeded)
            return (false, string.Join(" ", rezultat.Errors.Select(e => e.Description)));

        await _adminAkcijaServis.ZabeleziAsync(
            adminKorisnikId, TipoviAdminAkcija.Odblokiranje, korisnikId, "Blokada ukinuta.");

        return (true, null);
    }

    public async Task<(bool Uspesno, string? Greska)> ZabraniKomentarisanjeAsync(
        string korisnikId, string adminKorisnikId, ZabraniKomentarisanjeDto dto)
    {
        var korisnik = await _userManager.FindByIdAsync(korisnikId);
        if (korisnik == null) return (false, "Korisnik ne postoji.");

        korisnik.ZabranaKomentarisanjaDo = DateTime.UtcNow.AddDays(dto.BrojDana);

        var rezultat = await _userManager.UpdateAsync(korisnik);
        if (!rezultat.Succeeded)
            return (false, string.Join(" ", rezultat.Errors.Select(e => e.Description)));

        await _adminAkcijaServis.ZabeleziAsync(
            adminKorisnikId, TipoviAdminAkcija.ZabranaKomentarisanja, korisnikId,
            $"Zabrana komentarisanja na {dto.BrojDana} dana. Razlog: {dto.Razlog}");

        return (true, null);
    }

    public async Task<(bool Uspesno, string? Greska)> UkiniZabranuKomentarisanjaAsync(
        string korisnikId, string adminKorisnikId)
    {
        var korisnik = await _userManager.FindByIdAsync(korisnikId);
        if (korisnik == null) return (false, "Korisnik ne postoji.");

        if (korisnik.ZabranaKomentarisanjaDo == null)
            return (false, "Korisnik nema zabranu komentarisanja.");

        var trajala = korisnik.ZabranaKomentarisanjaDo.Value;
        korisnik.ZabranaKomentarisanjaDo = null;

        var rezultat = await _userManager.UpdateAsync(korisnik);
        if (!rezultat.Succeeded)
            return (false, string.Join(" ", rezultat.Errors.Select(e => e.Description)));

        await _adminAkcijaServis.ZabeleziAsync(
            adminKorisnikId, TipoviAdminAkcija.UkidanjeZabraneKomentarisanja, korisnikId,
            $"Zabrana komentarisanja ukinuta pre roka (važila je do {trajala:dd.MM.yyyy. HH:mm} UTC).");

        return (true, null);
    }

    private async Task<KorisnikAdminDto> MapirajAsync(Korisnik korisnik)
    {
        var uloge = await _userManager.GetRolesAsync(korisnik);
        var sada = DateTime.UtcNow;

        return new KorisnikAdminDto
        {
            Id = korisnik.Id,
            Ime = korisnik.Ime,
            Prezime = korisnik.Prezime,
            Email = korisnik.Email ?? string.Empty,
            BrojTelefona = korisnik.PhoneNumber,
            DatumRegistracije = korisnik.DatumRegistracije,
            Aktivan = korisnik.Aktivan,
            BlokiranDo = korisnik.BlokiranDo,
            ZabranaKomentarisanjaDo = korisnik.ZabranaKomentarisanjaDo,
            Uloge = uloge.ToList(),
            Blokiran = !korisnik.Aktivan || (korisnik.BlokiranDo.HasValue && korisnik.BlokiranDo > sada)
        };
    }
}
