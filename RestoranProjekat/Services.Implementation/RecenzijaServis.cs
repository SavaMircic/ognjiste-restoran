using Domain.Entiteti;
using Domain.Enumi;
using Domain.Konstante;
using Microsoft.AspNetCore.Identity;
using Repository.Interfejsi;
using Services.DTO;

namespace Services.Implementation;

public class RecenzijaServis : IRecenzijaServis
{
    private readonly IRecenzijaRepository _recenzijaRepository;
    private readonly IStavkaMenijaRepository _stavkaMenijaRepository;
    private readonly UserManager<Korisnik> _userManager;
    private readonly IAdminAkcijaServis _adminAkcijaServis;

    public RecenzijaServis(
        IRecenzijaRepository recenzijaRepository, IStavkaMenijaRepository stavkaMenijaRepository,
        UserManager<Korisnik> userManager, IAdminAkcijaServis adminAkcijaServis)
    {
        _recenzijaRepository = recenzijaRepository;
        _stavkaMenijaRepository = stavkaMenijaRepository;
        _userManager = userManager;
        _adminAkcijaServis = adminAkcijaServis;
    }

    public async Task<PaginiranaListaDto<RecenzijaDto>> PretraziAsync(RecenzijePretragaDto filter)
    {
        var (podaci, ukupno) = await _recenzijaRepository.PretraziAsync(
            filter.StavkaMenijaId, filter.TipRecenzije, filter.Sortiranje, filter.Strana, filter.VelicinaStrane);

        return new PaginiranaListaDto<RecenzijaDto>
        {
            Podaci = podaci.Select(Mapiraj).ToList(),
            UkupnoZapisa = ukupno,
            TrenutnaStrana = filter.Strana,
            UkupnoStrana = (int)Math.Ceiling(ukupno / (double)filter.VelicinaStrane)
        };
    }

    public async Task<(bool Uspesno, string? Greska, int StatusKod, RecenzijaDto? Recenzija)> KreirajAsync(string korisnikId, KreirajRecenzijuDto dto)
    {
        var korisnik = await _userManager.FindByIdAsync(korisnikId);
        if (korisnik == null) return (false, "Korisnik ne postoji.", 400, null);

        if (korisnik.ZabranaKomentarisanjaDo.HasValue && korisnik.ZabranaKomentarisanjaDo > DateTime.UtcNow)
            return (false, "Trenutno vam je zabranjeno komentarisanje.", 403, null);

        StavkaMenija? stavka = null;
        if (dto.TipRecenzije == TipRecenzije.Jelo)
        {
            stavka = await _stavkaMenijaRepository.DobaviPoIdAsync(dto.StavkaMenijaId!.Value);
            if (stavka == null) return (false, "Stavka menija ne postoji.", 400, null);
        }

        if (await _recenzijaRepository.DobaviPostojecuAsync(korisnikId, dto.TipRecenzije, stavka?.Id) != null)
            return (false, "Već ste ostavili recenziju za ovo.", 409, null);

        var recenzija = new Recenzija
        {
            KorisnikId = korisnikId,
            TipRecenzije = dto.TipRecenzije,
            StavkaMenijaId = stavka?.Id,
            Ocena = dto.Ocena,
            Naslov = dto.Naslov,
            Tekst = dto.Tekst,
            DatumKreiranja = DateTime.UtcNow
        };
        await _recenzijaRepository.DodajAsync(recenzija);
        await _recenzijaRepository.SacuvajPromeneAsync();

        return (true, null, 201, new RecenzijaDto
        {
            Id = recenzija.Id,
            KorisnikId = korisnikId,
            AutorIme = $"{korisnik.Ime} {korisnik.Prezime}",
            AutorSlikaUrl = korisnik.SlikaUrl,
            TipRecenzije = recenzija.TipRecenzije,
            StavkaMenijaId = recenzija.StavkaMenijaId,
            NazivStavke = stavka?.Naziv,
            Ocena = recenzija.Ocena,
            Naslov = recenzija.Naslov,
            Tekst = recenzija.Tekst,
            DatumKreiranja = recenzija.DatumKreiranja
        });
    }

    public async Task<(bool Uspesno, string? Greska)> IzmeniAsync(int id, string korisnikId, IzmenaRecenzijeDto dto)
    {
        var recenzija = await _recenzijaRepository.DobaviPoIdAsync(id);
        if (recenzija == null || recenzija.KorisnikId != korisnikId) return (false, "Recenzija ne postoji.");

        recenzija.Ocena = dto.Ocena;
        recenzija.Naslov = dto.Naslov;
        recenzija.Tekst = dto.Tekst;
        recenzija.DatumIzmene = DateTime.UtcNow;

        _recenzijaRepository.Azuriraj(recenzija);
        await _recenzijaRepository.SacuvajPromeneAsync();
        return (true, null);
    }

    public async Task<(bool Uspesno, string? Greska)> ObrisiAsync(int id, string korisnikId)
    {
        var recenzija = await _recenzijaRepository.DobaviPoIdAsync(id);
        if (recenzija == null || recenzija.KorisnikId != korisnikId) return (false, "Recenzija ne postoji.");

        _recenzijaRepository.Obrisi(recenzija);
        await _recenzijaRepository.SacuvajPromeneAsync();
        return (true, null);
    }

    public async Task<(bool Uspesno, string? Greska)> OdgovoriAsync(int id, OdgovorRecenzijeDto dto)
    {
        var recenzija = await _recenzijaRepository.DobaviPoIdAsync(id);
        if (recenzija == null) return (false, "Recenzija ne postoji.");

        recenzija.OdgovorRestorana = dto.OdgovorRestorana;
        recenzija.DatumOdgovora = DateTime.UtcNow;

        _recenzijaRepository.Azuriraj(recenzija);
        await _recenzijaRepository.SacuvajPromeneAsync();
        return (true, null);
    }

    public async Task<(bool Uspesno, string? Greska)> ObrisiAdminAsync(int id, string adminKorisnikId, string razlog)
    {
        var recenzija = await _recenzijaRepository.DobaviPoIdAsync(id);
        if (recenzija == null) return (false, "Recenzija ne postoji.");
        if (!recenzija.Aktivan) return (false, "Recenzija je već uklonjena.");

        recenzija.Aktivan = false;
        _recenzijaRepository.Azuriraj(recenzija);
        await _recenzijaRepository.SacuvajPromeneAsync();

        await _adminAkcijaServis.ZabeleziAsync(
            adminKorisnikId, TipoviAdminAkcija.BrisanjeRecenzije, recenzija.KorisnikId,
            $"Uklonjena recenzija #{recenzija.Id} (ocena {recenzija.Ocena}). Razlog: {razlog}");

        return (true, null);
    }

    private static RecenzijaDto Mapiraj(Recenzija r) => new()
    {
        Id = r.Id,
        KorisnikId = r.KorisnikId,
        AutorIme = $"{r.Korisnik.Ime} {r.Korisnik.Prezime}",
        AutorSlikaUrl = r.Korisnik.SlikaUrl,
        TipRecenzije = r.TipRecenzije,
        StavkaMenijaId = r.StavkaMenijaId,
        NazivStavke = r.StavkaMenija?.Naziv,
        Ocena = r.Ocena,
        Naslov = r.Naslov,
        Tekst = r.Tekst,
        DatumKreiranja = r.DatumKreiranja,
        DatumIzmene = r.DatumIzmene,
        OdgovorRestorana = r.OdgovorRestorana,
        DatumOdgovora = r.DatumOdgovora
    };
}
