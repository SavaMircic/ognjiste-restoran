using Domain.Entiteti;
using Domain.Enumi;
using Domain.Konstante;
using Microsoft.AspNetCore.Identity;
using Repository.Interfejsi;
using Services.DTO;

namespace Services.Implementation;

public class RezervacijaServis : IRezervacijaServis
{
    private const string KarakteriKoda = "ABCDEFGHJKLMNPQRSTUVWXYZ23456789";
    private const int MinimalniSatiZaOtkazivanje = PoslovnaPravila.RokOtkazivanjaSati;
    private const int DnevniLimit = 200;

    private readonly IRezervacijaRepository _rezervacijaRepository;
    private readonly IStoRepository _stoRepository;
    private readonly IPorudzbinaRepository _porudzbinaRepository;
    private readonly IZaposleniRepository _zaposleniRepository;
    private readonly UserManager<Korisnik> _userManager;
    private readonly IEmailServis _emailServis;
    private readonly IPostavkeRepository _postavkeRepository;

    public RezervacijaServis(
        IRezervacijaRepository rezervacijaRepository, IStoRepository stoRepository, IPorudzbinaRepository porudzbinaRepository,
        IZaposleniRepository zaposleniRepository, UserManager<Korisnik> userManager, IEmailServis emailServis,
        IPostavkeRepository postavkeRepository)
    {
        _rezervacijaRepository = rezervacijaRepository;
        _stoRepository = stoRepository;
        _porudzbinaRepository = porudzbinaRepository;
        _zaposleniRepository = zaposleniRepository;
        _userManager = userManager;
        _emailServis = emailServis;
        _postavkeRepository = postavkeRepository;
    }

    public async Task<DostupnostDto> ProveriDostupnostAsync(DostupnostUpitDto upit)
    {
        var postavke = await _postavkeRepository.DobaviAsync();

        var odgovor = new DostupnostDto
        {
            MaksGostijuOnline = PoslovnaPravila.MaksGostijuOnline,
            Telefon = postavke?.Telefon
        };

        if (upit.BrojGostiju > PoslovnaPravila.MaksGostijuOnline) return odgovor;

        var slobodni = await _rezervacijaRepository.DobaviSlobodneStoloveAsync(upit.DatumVreme);

        odgovor.Stolovi = slobodni
            .Where(s => s.Kapacitet >= upit.BrojGostiju)
            .Select(MapirajSto)
            .ToList();

        if (odgovor.Stolovi.Count == 0)
            odgovor.Predlozi = SastaviPredloge(slobodni, upit.BrojGostiju);

        return odgovor;
    }

    private static List<PredlogSpajanjaDto> SastaviPredloge(List<Sto> slobodni, int brojGostiju)
    {
        var predlozi = new List<PredlogSpajanjaDto>();
        var izbor = new List<Sto>();

        void Trazi(int od)
        {
            if (izbor.Count >= 2)
            {
                var ukupno = izbor.Sum(s => s.Kapacitet);
                if (ukupno >= brojGostiju)
                {
                    predlozi.Add(new PredlogSpajanjaDto
                    {
                        Stolovi = izbor.OrderBy(s => s.BrojStola).Select(MapirajSto).ToList(),
                        UkupnoMesta = ukupno,
                        VisakMesta = ukupno - brojGostiju
                    });
                }
            }

            if (izbor.Count >= PoslovnaPravila.MaksSpojenihStolova) return;

            for (var i = od; i < slobodni.Count; i++)
            {
                izbor.Add(slobodni[i]);
                Trazi(i + 1);
                izbor.RemoveAt(izbor.Count - 1);
            }
        }

        Trazi(0);

        return predlozi
            .OrderBy(p => p.Stolovi.Count)
            .ThenBy(p => p.VisakMesta)
            .Take(3)
            .ToList();
    }

    private static StoDto MapirajSto(Sto s) => new()
    {
        Id = s.Id,
        BrojStola = s.BrojStola,
        Kapacitet = s.Kapacitet,
        TrenutniStatus = s.TrenutniStatus
    };

    public async Task<(bool Uspesno, string? Greska, RezervacijaDto? Rezervacija)> KreirajAsync(string korisnikId, KreirajRezervacijuDto dto)
    {
        var (uspesno, greska, stolovi) = await ProveriStoloveAsync(dto.StoIds, dto.BrojGostiju, dto.DatumVreme);
        if (!uspesno) return (false, greska, null);

        var korisnik = await _userManager.FindByIdAsync(korisnikId);
        if (korisnik == null) return (false, "Korisnik ne postoji.", null);

        var rezervacije = await UpisiZaStoloveAsync(stolovi, dto.DatumVreme, dto.BrojGostiju, r =>
        {
            r.KorisnikId = korisnikId;
            r.NacinKreiranja = NacinKreiranjaRezervacije.Samostalno;
        });

        await _emailServis.PosaljiBezPrekidaAsync(korisnik.Email!, "Potvrda rezervacije",
            TeloPotvrde("Vaša rezervacija je potvrđena.", stolovi, dto.DatumVreme, dto.BrojGostiju, rezervacije[0].KodRezervacije));

        return (true, null, SaSvimStolovima(rezervacije[0], stolovi, $"{korisnik.Ime} {korisnik.Prezime}"));
    }

    private async Task<(bool Uspesno, string? Greska, List<Sto> Stolovi)> ProveriStoloveAsync(
        List<int> stoIds, int brojGostiju, DateTime datumVreme)
    {
        var jedinstveni = stoIds.Distinct().ToList();

        if (jedinstveni.Count == 0) return (false, "Izaberite bar jedan sto.", []);
        if (jedinstveni.Count != stoIds.Count) return (false, "Isti sto je izabran više puta.", []);
        if (jedinstveni.Count > PoslovnaPravila.MaksSpojenihStolova)
            return (false, $"Najviše {PoslovnaPravila.MaksSpojenihStolova} stola po rezervaciji. Za veće društvo nas pozovite.", []);

        if (brojGostiju > PoslovnaPravila.MaksGostijuOnline)
            return (false, $"Preko sajta se rezerviše za najviše {PoslovnaPravila.MaksGostijuOnline} gostiju. Za veće društvo nas pozovite telefonom.", []);

        var stolovi = new List<Sto>();
        foreach (var id in jedinstveni)
        {
            var sto = await _stoRepository.DobaviPoIdAsync(id);
            if (sto == null) return (false, "Sto ne postoji.", []);

            if (await _rezervacijaRepository.PostojiKonfliktAsync(id, datumVreme))
                return (false, $"Sto {sto.BrojStola} nije slobodan u traženom terminu.", []);

            stolovi.Add(sto);
        }

        var ukupno = stolovi.Sum(s => s.Kapacitet);
        if (ukupno < brojGostiju)
            return (false, $"Izabrani stolovi primaju {ukupno} gostiju, a traženo je {brojGostiju}.", []);

        return (true, null, stolovi);
    }

    private async Task<List<Rezervacija>> UpisiZaStoloveAsync(
        List<Sto> stolovi, DateTime datumVreme, int brojGostiju, Action<Rezervacija> dopuni)
    {
        var kod = await GenerisiJedinstveniKodAsync();
        var rezervacije = new List<Rezervacija>();

        foreach (var sto in stolovi)
        {
            var rezervacija = new Rezervacija
            {
                StoId = sto.Id,
                DatumVreme = datumVreme,
                BrojGostiju = brojGostiju,
                KodRezervacije = kod,
                Status = StatusRezervacije.Aktivna,
                DatumKreiranja = DateTime.UtcNow
            };
            dopuni(rezervacija);

            await _rezervacijaRepository.DodajAsync(rezervacija);
            OznaciStoAkoJeSkoro(sto, datumVreme);
            rezervacije.Add(rezervacija);
        }

        await _rezervacijaRepository.SacuvajPromeneAsync();
        return rezervacije;
    }

    private static RezervacijaDto SaSvimStolovima(Rezervacija prva, List<Sto> stolovi, string imeZaPrikaz)
    {
        var dto = Mapiraj(prva, stolovi[0], imeZaPrikaz);
        dto.BrojeviStolova = stolovi.Select(s => s.BrojStola).OrderBy(b => b).ToList();
        return dto;
    }

    private static string TeloPotvrde(string uvod, List<Sto> stolovi, DateTime datumVreme, int brojGostiju, string kod)
    {
        var oznaka = stolovi.Count == 1
            ? $"Sto {stolovi[0].BrojStola}"
            : $"Spojeni stolovi {string.Join(" i ", stolovi.Select(s => s.BrojStola))}";

        return $"<p>{uvod}</p><p>{oznaka}, {datumVreme:dd.MM.yyyy HH:mm}, {brojGostiju} gostiju.</p>" +
               $"<p>Kod rezervacije: <b>{kod}</b></p>";
    }

    public async Task<List<RezervacijaDto>> MojeAsync(string korisnikId)
    {
        var rezervacije = await _rezervacijaRepository.ListirajZaKorisnikaAsync(korisnikId);

        return rezervacije
            .GroupBy(r => r.KodRezervacije)
            .Select(grupa =>
            {
                var prvi = grupa.OrderBy(r => r.Sto.BrojStola).First();
                var dto = Mapiraj(prvi, prvi.Sto, ImeZaPrikaz(prvi));
                dto.BrojeviStolova = grupa.Select(r => r.Sto.BrojStola).OrderBy(b => b).ToList();
                return dto;
            })
            .OrderByDescending(r => r.DatumVreme)
            .ToList();
    }

    public async Task<(bool Uspesno, string? Greska)> OtkaziAsync(int id, string korisnikId)
    {
        var rezervacija = await _rezervacijaRepository.DobaviPoIdAsync(id);
        if (rezervacija == null || rezervacija.KorisnikId != korisnikId) return (false, "Rezervacija ne postoji.");
        if (rezervacija.Status != StatusRezervacije.Aktivna) return (false, "Rezervacija se ne može otkazati.");

        if (rezervacija.DatumVreme - DateTime.UtcNow < TimeSpan.FromHours(MinimalniSatiZaOtkazivanje))
            return (false, $"Rezervacija se može otkazati najkasnije {MinimalniSatiZaOtkazivanje}h pre termina.");

        var deloviGrupe = await _rezervacijaRepository.ListirajPoKoduAsync(rezervacija.KodRezervacije);

        foreach (var deo in deloviGrupe.Where(d => d.Status == StatusRezervacije.Aktivna))
        {
            deo.Status = StatusRezervacije.Otkazana;
            _rezervacijaRepository.Azuriraj(deo);
            await OslobodiStoAkoTrebaAsync(deo.StoId);
        }

        await _rezervacijaRepository.SacuvajPromeneAsync();
        return (true, null);
    }

    public async Task<(bool Uspesno, string? Greska, RezervacijaDto? Rezervacija)> KreirajAdminAsync(string adminKorisnikId, KreirajAdminRezervacijuDto dto)
    {
        var sto = await _stoRepository.DobaviPoIdAsync(dto.StoId);
        if (sto == null) return (false, "Sto ne postoji.", null);
        if (sto.Kapacitet < dto.BrojGostiju) return (false, "Sto nema dovoljan kapacitet za traženi broj gostiju.", null);

        if (await _rezervacijaRepository.PostojiKonfliktAsync(dto.StoId, dto.DatumVreme))
            return (false, "Sto nije slobodan u traženom terminu.", null);

        Korisnik? korisnik = null;
        if (!string.IsNullOrWhiteSpace(dto.KorisnikId))
        {
            korisnik = await _userManager.FindByIdAsync(dto.KorisnikId);
            if (korisnik == null) return (false, "Korisnik ne postoji.", null);
        }

        var admin = await _zaposleniRepository.DobaviPoKorisnikIdAsync(adminKorisnikId);

        var rezervacija = new Rezervacija
        {
            KorisnikId = korisnik?.Id,
            GostIme = korisnik == null ? dto.GostIme : null,
            GostEmail = korisnik == null ? dto.GostEmail : null,
            GostTelefon = korisnik == null ? dto.GostTelefon : null,
            StoId = dto.StoId,
            DatumVreme = dto.DatumVreme,
            BrojGostiju = dto.BrojGostiju,
            KodRezervacije = await GenerisiJedinstveniKodAsync(),
            Status = StatusRezervacije.Aktivna,
            NacinKreiranja = dto.NacinKreiranja,
            KreiraoZaposleniId = admin?.Id,
            DatumKreiranja = DateTime.UtcNow
        };
        await _rezervacijaRepository.DodajAsync(rezervacija);

        OznaciStoAkoJeSkoro(sto, dto.DatumVreme);

        await _rezervacijaRepository.SacuvajPromeneAsync();

        var emailZaSlanje = korisnik?.Email ?? dto.GostEmail;
        if (!string.IsNullOrWhiteSpace(emailZaSlanje))
        {
            await _emailServis.PosaljiBezPrekidaAsync(emailZaSlanje!, "Potvrda rezervacije",
                $"<p>Rezervacija je kreirana.</p><p>Sto {sto.BrojStola}, {dto.DatumVreme:dd.MM.yyyy HH:mm}, {dto.BrojGostiju} gostiju.</p><p>Kod rezervacije: <b>{rezervacija.KodRezervacije}</b></p>");
        }

        var imeZaPrikaz = korisnik != null ? $"{korisnik.Ime} {korisnik.Prezime}" : dto.GostIme ?? "Gost";
        return (true, null, Mapiraj(rezervacija, sto, imeZaPrikaz));
    }

    public async Task<PaginiranaListaDto<RezervacijaDto>> PretraziAsync(RezervacijePretragaDto filter)
    {
        var (podaci, ukupno) = await _rezervacijaRepository.PretraziAsync(
            filter.Datum, filter.Status, filter.Pretraga, filter.Strana, filter.VelicinaStrane);

        return new PaginiranaListaDto<RezervacijaDto>
        {
            Podaci = podaci.Select(r => Mapiraj(r, r.Sto, ImeZaPrikaz(r))).ToList(),
            UkupnoZapisa = ukupno,
            TrenutnaStrana = filter.Strana,
            UkupnoStrana = (int)Math.Ceiling(ukupno / (double)filter.VelicinaStrane)
        };
    }

    public async Task<List<RezervacijaDto>> ZaDanasAsync()
    {
        var (podaci, _) = await _rezervacijaRepository.PretraziAsync(
            DateTime.UtcNow, null, null, strana: 1, velicinaStrane: DnevniLimit);

        return podaci
            .OrderBy(r => r.DatumVreme)
            .Select(r => Mapiraj(r, r.Sto, ImeZaPrikaz(r)))
            .ToList();
    }

    public async Task<(bool Uspesno, string? Greska)> PrijaviDolazakAsync(int id, string trenutniKorisnikId)
    {
        var rezervacija = await _rezervacijaRepository.DobaviPoIdAsync(id);
        if (rezervacija == null) return (false, "Rezervacija ne postoji.");
        if (rezervacija.Status != StatusRezervacije.Aktivna) return (false, "Rezervacija nije aktivna.");

        var zaposleni = await _zaposleniRepository.DobaviPoKorisnikIdAsync(trenutniKorisnikId);
        if (zaposleni == null) return (false, "Nalog nije povezan sa zaposlenim.");

        var sto = await _stoRepository.DobaviPoIdAsync(rezervacija.StoId);
        if (sto == null) return (false, "Sto ne postoji.");

        var otvorena = await _porudzbinaRepository.DobaviOtvorenuZaStoAsync(rezervacija.StoId);

        if (otvorena != null && otvorena.RezervacijaId != null && otvorena.RezervacijaId != rezervacija.Id)
            return (false, $"Sto {sto.BrojStola} ima otvoren račun prethodne rezervacije. Zatvorite taj račun pa ponovite prijavu dolaska.");

        if (otvorena != null)
        {
            otvorena.RezervacijaId = rezervacija.Id;
            _porudzbinaRepository.Azuriraj(otvorena);
        }
        else
        {
            var porudzbina = new Porudzbina
            {
                StoId = rezervacija.StoId,
                KonobarId = zaposleni.Id,
                RezervacijaId = rezervacija.Id,
                VremeOtvaranja = DateTime.UtcNow,
                Status = StatusPorudzbine.Otvorena
            };
            await _porudzbinaRepository.DodajAsync(porudzbina);
        }

        sto.TrenutniStatus = StatusStola.Zauzet;
        _stoRepository.Azuriraj(sto);

        rezervacija.Status = StatusRezervacije.Realizovana;
        _rezervacijaRepository.Azuriraj(rezervacija);

        await _rezervacijaRepository.SacuvajPromeneAsync();
        return (true, null);
    }

    public async Task<(bool Uspesno, string? Greska)> OznaciIsteklomAsync(int id)
    {
        var rezervacija = await _rezervacijaRepository.DobaviPoIdAsync(id);
        if (rezervacija == null) return (false, "Rezervacija ne postoji.");
        if (rezervacija.Status != StatusRezervacije.Aktivna) return (false, "Rezervacija nije aktivna.");

        rezervacija.Status = StatusRezervacije.Istekla;
        _rezervacijaRepository.Azuriraj(rezervacija);

        await OslobodiStoAkoTrebaAsync(rezervacija.StoId);

        await _rezervacijaRepository.SacuvajPromeneAsync();
        return (true, null);
    }

    public async Task<List<string>> IsteciDospeleAsync()
    {
        var granica = DateTime.UtcNow.AddMinutes(-PoslovnaPravila.TolerancijaRezervacijeMinuta);

        var dospele = await _rezervacijaRepository.ListirajDospeleZaIstekAsync(granica);
        if (dospele.Count == 0) return new List<string>();

        foreach (var rezervacija in dospele)
        {
            rezervacija.Status = StatusRezervacije.Istekla;
            _rezervacijaRepository.Azuriraj(rezervacija);

            await OslobodiStoAkoTrebaAsync(rezervacija.StoId);
        }

        await _rezervacijaRepository.SacuvajPromeneAsync();

        return dospele.Select(r => r.KodRezervacije).ToList();
    }

    private void OznaciStoAkoJeSkoro(Sto sto, DateTime datumVreme)
    {
        if (sto.TrenutniStatus == StatusStola.Slobodan && datumVreme <= DateTime.UtcNow.AddHours(1))
        {
            sto.TrenutniStatus = StatusStola.Rezervisan;
            _stoRepository.Azuriraj(sto);
        }
    }

    private async Task OslobodiStoAkoTrebaAsync(int stoId)
    {
        var sto = await _stoRepository.DobaviPoIdAsync(stoId);
        if (sto != null && sto.TrenutniStatus == StatusStola.Rezervisan)
        {
            sto.TrenutniStatus = StatusStola.Slobodan;
            _stoRepository.Azuriraj(sto);
        }
    }

    private async Task<string> GenerisiJedinstveniKodAsync()
    {
        string kod;
        do
        {
            kod = new string(Enumerable.Range(0, 8).Select(_ => KarakteriKoda[Random.Shared.Next(KarakteriKoda.Length)]).ToArray());
        } while (await _rezervacijaRepository.DobaviPoKoduAsync(kod) != null);

        return kod;
    }

    private static string ImeZaPrikaz(Rezervacija r) =>
        r.Korisnik != null ? $"{r.Korisnik.Ime} {r.Korisnik.Prezime}" : r.GostIme ?? "Gost";

    private static RezervacijaDto Mapiraj(Rezervacija r, Sto sto, string imeZaPrikaz) => new()
    {
        Id = r.Id,
        KorisnikId = r.KorisnikId,
        ImeZaPrikaz = imeZaPrikaz,
        StoId = r.StoId,
        BrojStola = sto.BrojStola,
        BrojeviStolova = [sto.BrojStola],
        DatumVreme = r.DatumVreme,
        VaziOd = r.DatumVreme,
        VaziDo = r.DatumVreme.AddHours(PoslovnaPravila.TrajanjeRezervacijeSati),
        TolerancijaDo = r.DatumVreme.AddMinutes(PoslovnaPravila.TolerancijaRezervacijeMinuta),
        BrojGostiju = r.BrojGostiju,
        KodRezervacije = r.KodRezervacije,
        Status = r.Status,
        NacinKreiranja = r.NacinKreiranja,
        DatumKreiranja = r.DatumKreiranja
    };
}
