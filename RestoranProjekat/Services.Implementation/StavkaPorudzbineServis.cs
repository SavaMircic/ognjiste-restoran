using Domain.Entiteti;
using Domain.Enumi;
using Domain.Konstante;
using Microsoft.AspNetCore.Identity;
using Repository.Interfejsi;
using Services.DTO;

namespace Services.Implementation;

public class StavkaPorudzbineServis : IStavkaPorudzbineServis
{
    private readonly IStavkaPorudzbineRepository _repository;
    private readonly IZaposleniRepository _zaposleniRepository;
    private readonly UserManager<Korisnik> _userManager;
    private readonly IReceptureRepository _receptureRepository;
    private readonly IRepository<Namirnica> _namirnicaRepository;
    private readonly INotifikacijaServis _notifikacijaServis;

    public StavkaPorudzbineServis(
        IStavkaPorudzbineRepository repository, IZaposleniRepository zaposleniRepository, UserManager<Korisnik> userManager,
        IReceptureRepository receptureRepository, IRepository<Namirnica> namirnicaRepository, INotifikacijaServis notifikacijaServis)
    {
        _repository = repository;
        _zaposleniRepository = zaposleniRepository;
        _userManager = userManager;
        _receptureRepository = receptureRepository;
        _namirnicaRepository = namirnicaRepository;
        _notifikacijaServis = notifikacijaServis;
    }

    public async Task<List<RedCekanjaStavkaDto>> RedCekanjaAsync(Odrediste odrediste, string trenutniKorisnikId)
    {
        var ja = await _zaposleniRepository.DobaviPoKorisnikIdAsync(trenutniKorisnikId);

        var stavke = await _repository.RedCekanjaAsync(odrediste);
        return stavke.Select(s => new RedCekanjaStavkaDto
        {
            Id = s.Id,
            BrojStola = s.Porudzbina.Sto.BrojStola,
            NazivStavke = s.StavkaMenija.Naziv,
            Kolicina = s.Kolicina,
            Napomena = s.Napomena,
            VremeSlanja = s.VremeSlanja,
            Status = s.Status,
            PripremioIme = s.PripremioZaposleni == null
                ? null
                : $"{s.PripremioZaposleni.Korisnik.Ime} {s.PripremioZaposleni.Korisnik.Prezime}",
            MojaStavka = ja != null && s.PripremioZaposleniId == ja.Id
        }).ToList();
    }

    public async Task<(bool Uspesno, string? Greska)> PreuzmiAsync(int stavkaId, string trenutniKorisnikId)
    {
        var stavka = await _repository.DobaviSaDetaljimaAsync(stavkaId);
        if (stavka == null) return (false, "Stavka ne postoji.");
        if (stavka.Status != StatusStavkePorudzbine.Poslato) return (false, "Stavka je već preuzeta ili gotova.");

        var (uspesno, greska, zaposleni) = await ProveriUlogu(trenutniKorisnikId, stavka.StavkaMenija.Kategorija.Odrediste);
        if (!uspesno) return (false, greska);

        stavka.Status = StatusStavkePorudzbine.UPripremi;
        stavka.PripremioZaposleniId = zaposleni!.Id;
        stavka.VremePreuzimanja = DateTime.UtcNow;

        _repository.Azuriraj(stavka);
        await _repository.SacuvajPromeneAsync();
        return (true, null);
    }

    public async Task<(bool Uspesno, string? Greska)> ZavrsiAsync(int stavkaId, string trenutniKorisnikId)
    {
        var stavka = await _repository.DobaviSaDetaljimaAsync(stavkaId);
        if (stavka == null) return (false, "Stavka ne postoji.");
        if (stavka.Status != StatusStavkePorudzbine.UPripremi) return (false, "Stavka nije preuzeta na pripremu.");

        var zaposleni = await _zaposleniRepository.DobaviPoKorisnikIdAsync(trenutniKorisnikId);
        if (zaposleni == null || zaposleni.Id != stavka.PripremioZaposleniId)
            return (false, "Samo zaposleni koji je preuzeo stavku može da je označi gotovom.");

        stavka.Status = StatusStavkePorudzbine.Spremno;
        stavka.VremeZavrsetka = DateTime.UtcNow;
        _repository.Azuriraj(stavka);

        await UmanjiZaliheAsync(stavka.StavkaMenijaId, stavka.Kolicina);

        await _repository.SacuvajPromeneAsync();

        await _notifikacijaServis.PosaljiStavkaSpremnaAsync(stavka.Porudzbina.Konobar.KorisnikId, new StavkaSpremnaNotifikacijaDto
        {
            StavkaPorudzbineId = stavka.Id,
            BrojStola = stavka.Porudzbina.Sto.BrojStola,
            NazivStavke = stavka.StavkaMenija.Naziv
        });

        return (true, null);
    }

    public async Task<(bool Uspesno, string? Greska)> IsporuciAsync(int stavkaId)
    {
        var stavka = await _repository.DobaviPoIdAsync(stavkaId);
        if (stavka == null) return (false, "Stavka ne postoji.");
        if (stavka.Status != StatusStavkePorudzbine.Spremno) return (false, "Stavka još nije spremna.");

        return (true, null);
    }

    private async Task<(bool Uspesno, string? Greska, Zaposleni? Zaposleni)> ProveriUlogu(string korisnikId, Odrediste odrediste)
    {
        var zaposleni = await _zaposleniRepository.DobaviPoKorisnikIdAsync(korisnikId);
        if (zaposleni == null) return (false, "Nalog nije povezan sa zaposlenim.", null);

        var korisnik = await _userManager.FindByIdAsync(zaposleni.KorisnikId);
        var uloge = korisnik == null ? new List<string>() : (await _userManager.GetRolesAsync(korisnik)).ToList();

        var potrebnaUloga = odrediste == Odrediste.Kuhinja ? Uloge.Kuvar : Uloge.Sanker;
        if (!uloge.Contains(potrebnaUloga))
            return (false, $"Ovu stavku može preuzeti samo {potrebnaUloga}.", null);

        return (true, null, zaposleni);
    }

    private async Task UmanjiZaliheAsync(int stavkaMenijaId, int kolicinaPorcija)
    {
        var receptura = await _receptureRepository.ListirajZaStavkuAsync(stavkaMenijaId);
        foreach (var red in receptura)
        {
            var namirnica = await _namirnicaRepository.DobaviPoIdAsync(red.NamirnicaId);
            if (namirnica == null) continue;

            namirnica.TrenutnaKolicina -= red.Kolicina * kolicinaPorcija;
            _namirnicaRepository.Azuriraj(namirnica);
        }
    }
}
