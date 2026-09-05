using System.Text;
using Domain.Entiteti;
using Domain.Enumi;
using Repository.Interfejsi;
using Services.DTO;

namespace Services.Implementation;

public class PorudzbinaServis : IPorudzbinaServis
{
    private readonly IPorudzbinaRepository _porudzbinaRepository;
    private readonly IStoRepository _stoRepository;
    private readonly IZaposleniRepository _zaposleniRepository;
    private readonly IStavkaMenijaRepository _stavkaMenijaRepository;
    private readonly IStavkaPorudzbineRepository _stavkaPorudzbineRepository;
    private readonly INotifikacijaServis _notifikacijaServis;

    public PorudzbinaServis(
        IPorudzbinaRepository porudzbinaRepository, IStoRepository stoRepository, IZaposleniRepository zaposleniRepository,
        IStavkaMenijaRepository stavkaMenijaRepository, IStavkaPorudzbineRepository stavkaPorudzbineRepository,
        INotifikacijaServis notifikacijaServis)
    {
        _porudzbinaRepository = porudzbinaRepository;
        _stoRepository = stoRepository;
        _zaposleniRepository = zaposleniRepository;
        _stavkaMenijaRepository = stavkaMenijaRepository;
        _stavkaPorudzbineRepository = stavkaPorudzbineRepository;
        _notifikacijaServis = notifikacijaServis;
    }

    public async Task<(bool Uspesno, string? Greska, int? Id)> OtvoriAsync(string trenutniKorisnikId, OtvoriPorudzbinuDto dto)
    {
        var konobar = await _zaposleniRepository.DobaviPoKorisnikIdAsync(trenutniKorisnikId);
        if (konobar == null) return (false, "Nalog nije povezan sa zaposlenim.", null);

        var sto = await _stoRepository.DobaviPoIdAsync(dto.StoId);
        if (sto == null) return (false, "Sto ne postoji.", null);

        if (await _porudzbinaRepository.DobaviOtvorenuZaStoAsync(dto.StoId) != null)
            return (false, "Sto već ima otvorenu porudžbinu.", null);

        var porudzbina = new Porudzbina
        {
            StoId = dto.StoId,
            KonobarId = konobar.Id,
            RezervacijaId = dto.RezervacijaId,
            VremeOtvaranja = DateTime.UtcNow,
            Status = StatusPorudzbine.Otvorena
        };
        await _porudzbinaRepository.DodajAsync(porudzbina);

        sto.TrenutniStatus = StatusStola.Zauzet;
        _stoRepository.Azuriraj(sto);

        await _porudzbinaRepository.SacuvajPromeneAsync();

        return (true, null, porudzbina.Id);
    }

    public async Task<List<PorudzbinaAktivnaDto>> ListirajAktivneAsync()
    {
        var aktivne = await _porudzbinaRepository.ListirajAktivneAsync();
        return aktivne.Select(p => new PorudzbinaAktivnaDto
        {
            Id = p.Id,
            BrojStola = p.Sto.BrojStola,
            RezervacijaId = p.RezervacijaId,
            VremeOtvaranja = p.VremeOtvaranja,
            BrojStavkiPoslato = p.Stavke.Count(s => s.Status == StatusStavkePorudzbine.Poslato),
            BrojStavkiUPripremi = p.Stavke.Count(s => s.Status == StatusStavkePorudzbine.UPripremi),
            BrojStavkiSpremno = p.Stavke.Count(s => s.Status == StatusStavkePorudzbine.Spremno)
        }).ToList();
    }

    public async Task<PorudzbinaDetaljDto?> DobaviDetaljAsync(int id)
    {
        var p = await _porudzbinaRepository.DobaviDetaljAsync(id);
        return p == null ? null : Mapiraj(p);
    }

    public async Task<(bool Uspesno, string? Greska)> DodajStavkeAsync(int porudzbinaId, DodajStavkeDto dto)
    {
        var porudzbina = await _porudzbinaRepository.DobaviDetaljAsync(porudzbinaId);
        if (porudzbina == null) return (false, "Porudžbina ne postoji.");
        if (porudzbina.Status != StatusPorudzbine.Otvorena) return (false, "Porudžbina nije otvorena.");

        var noveStavke = new List<(StavkaPorudzbine Stavka, StavkaMenija Menija)>();
        foreach (var stavkaDto in dto.Stavke)
        {
            var stavkaMenija = await _stavkaMenijaRepository.DobaviDetaljAsync(stavkaDto.StavkaMenijaId);
            if (stavkaMenija == null) return (false, $"Stavka menija sa id {stavkaDto.StavkaMenijaId} ne postoji.");
            if (!stavkaMenija.Dostupno) return (false, $"Stavka '{stavkaMenija.Naziv}' trenutno nije dostupna.");

            var stavka = new StavkaPorudzbine
            {
                PorudzbinaId = porudzbinaId,
                StavkaMenijaId = stavkaDto.StavkaMenijaId,
                Kolicina = stavkaDto.Kolicina,
                Napomena = stavkaDto.Napomena,
                CenaUTrenutkuNarudzbine = stavkaMenija.CenaSaPopustom,
                Status = StatusStavkePorudzbine.Poslato,
                VremeSlanja = DateTime.UtcNow
            };
            noveStavke.Add((stavka, stavkaMenija));
            await _stavkaPorudzbineRepository.DodajAsync(stavka);
        }

        await _stavkaPorudzbineRepository.SacuvajPromeneAsync();

        foreach (var (stavka, menija) in noveStavke)
        {
            await _notifikacijaServis.PosaljiNovaStavkaAsync(menija.Kategorija.Odrediste, new NovaStavkaNotifikacijaDto
            {
                StavkaPorudzbineId = stavka.Id,
                BrojStola = porudzbina.Sto.BrojStola,
                NazivStavke = menija.Naziv,
                Kolicina = stavka.Kolicina,
                Napomena = stavka.Napomena
            });
        }

        return (true, null);
    }

    public async Task<(bool Uspesno, string? Greska)> OtkaziStavkuAsync(int porudzbinaId, int stavkaId)
    {
        var stavka = await _stavkaPorudzbineRepository.DobaviPoIdAsync(stavkaId);
        if (stavka == null || stavka.PorudzbinaId != porudzbinaId) return (false, "Stavka ne postoji.");
        if (stavka.Status != StatusStavkePorudzbine.Poslato)
            return (false, "Stavka se više ne može otkazati — već je preuzeta na pripremu.");

        _stavkaPorudzbineRepository.Obrisi(stavka);
        await _stavkaPorudzbineRepository.SacuvajPromeneAsync();
        return (true, null);
    }

    public async Task<(bool Uspesno, string? Greska)> ZatvoriAsync(int porudzbinaId, ZatvoriPorudzbinuDto dto)
    {
        var porudzbina = await _porudzbinaRepository.DobaviDetaljAsync(porudzbinaId);
        if (porudzbina == null) return (false, "Porudžbina ne postoji.");
        if (porudzbina.Status != StatusPorudzbine.Otvorena) return (false, "Porudžbina je već zatvorena.");

        if (porudzbina.Stavke.Any(s => s.Status != StatusStavkePorudzbine.Spremno))
            return (false, "Sve stavke moraju biti spremne pre zatvaranja porudžbine.");

        porudzbina.Status = StatusPorudzbine.Zatvorena;
        porudzbina.VremeZatvaranja = DateTime.UtcNow;
        porudzbina.NacinPlacanja = dto.NacinPlacanja;
        porudzbina.IznosNapojnice = dto.IznosNapojnice;
        _porudzbinaRepository.Azuriraj(porudzbina);

        porudzbina.Sto.TrenutniStatus = StatusStola.Slobodan;
        _stoRepository.Azuriraj(porudzbina.Sto);

        await _porudzbinaRepository.SacuvajPromeneAsync();
        return (true, null);
    }

    public async Task<(bool Uspesno, string? Greska, string? SadrzajRacuna)> DobaviRacunAsync(int porudzbinaId)
    {
        var porudzbina = await _porudzbinaRepository.DobaviDetaljAsync(porudzbinaId);
        if (porudzbina == null) return (false, "Porudžbina ne postoji.", null);
        if (porudzbina.Status != StatusPorudzbine.Zatvorena)
            return (false, "Račun se generiše tek kad se porudžbina zatvori.", null);

        return (true, null, GenerisiRacun(porudzbina));
    }

    private static PorudzbinaDetaljDto Mapiraj(Porudzbina p) => new()
    {
        Id = p.Id,
        StoId = p.StoId,
        BrojStola = p.Sto.BrojStola,
        KonobarId = p.KonobarId,
        KonobarIme = $"{p.Konobar.Korisnik.Ime} {p.Konobar.Korisnik.Prezime}",
        VremeOtvaranja = p.VremeOtvaranja,
        VremeZatvaranja = p.VremeZatvaranja,
        Status = p.Status,
        NacinPlacanja = p.NacinPlacanja,
        IznosNapojnice = p.IznosNapojnice,
        Stavke = p.Stavke.Select(s => new StavkaPorudzbineDto
        {
            Id = s.Id,
            StavkaMenijaId = s.StavkaMenijaId,
            NazivStavke = s.StavkaMenija.Naziv,
            Kolicina = s.Kolicina,
            Napomena = s.Napomena,
            CenaUTrenutkuNarudzbine = s.CenaUTrenutkuNarudzbine,
            Status = s.Status,
            PripremioZaposleniId = s.PripremioZaposleniId,
            VremeSlanja = s.VremeSlanja,
            VremePreuzimanja = s.VremePreuzimanja,
            VremeZavrsetka = s.VremeZavrsetka
        }).ToList()
    };

    private static string GenerisiRacun(Porudzbina porudzbina)
    {
        var sb = new StringBuilder();
        sb.AppendLine("========================================");
        sb.AppendLine("                RAČUN");
        sb.AppendLine("========================================");
        sb.AppendLine($"Sto: {porudzbina.Sto.BrojStola}");
        sb.AppendLine($"Otvoreno:  {porudzbina.VremeOtvaranja:dd.MM.yyyy HH:mm}");
        sb.AppendLine($"Zatvoreno: {porudzbina.VremeZatvaranja:dd.MM.yyyy HH:mm}");
        sb.AppendLine("----------------------------------------");

        decimal ukupno = 0;
        foreach (var stavka in porudzbina.Stavke)
        {
            var iznos = stavka.CenaUTrenutkuNarudzbine * stavka.Kolicina;
            ukupno += iznos;
            sb.AppendLine($"{stavka.StavkaMenija.Naziv,-22} {stavka.Kolicina,3} x {stavka.CenaUTrenutkuNarudzbine,8:0.00} = {iznos,10:0.00}");
        }

        sb.AppendLine("----------------------------------------");
        sb.AppendLine($"UKUPNO:{ukupno,33:0.00} RSD");
        sb.AppendLine($"Način plaćanja: {porudzbina.NacinPlacanja}");
        if (porudzbina.IznosNapojnice.HasValue)
            sb.AppendLine($"Napojnica: {porudzbina.IznosNapojnice:0.00} RSD");
        sb.AppendLine("========================================");
        sb.AppendLine("           Hvala na poseti!");
        sb.AppendLine("========================================");

        return sb.ToString();
    }
}
