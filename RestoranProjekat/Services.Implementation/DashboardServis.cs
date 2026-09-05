using Domain.Enumi;
using Repository.Interfejsi;
using Services.DTO;

namespace Services.Implementation;

public class DashboardServis : IDashboardServis
{
    private const int MaksNamirnicaUPregledu = 5;

    private readonly IIzvestajRepository _izvestajRepository;
    private readonly INamirnicaRepository _namirnicaRepository;
    private readonly IPorudzbinaRepository _porudzbinaRepository;
    private readonly IPorukaRepository _porukaRepository;
    private readonly IRezervacijaRepository _rezervacijaRepository;
    private readonly IKorisnikAdminRepository _korisnikAdminRepository;
    private readonly IRecenzijaRepository _recenzijaRepository;
    private readonly IPrijavaProblemaRepository _prijavaRepository;

    public DashboardServis(
        IIzvestajRepository izvestajRepository, INamirnicaRepository namirnicaRepository,
        IPorudzbinaRepository porudzbinaRepository, IPorukaRepository porukaRepository,
        IRezervacijaRepository rezervacijaRepository, IKorisnikAdminRepository korisnikAdminRepository,
        IRecenzijaRepository recenzijaRepository, IPrijavaProblemaRepository prijavaRepository)
    {
        _izvestajRepository = izvestajRepository;
        _namirnicaRepository = namirnicaRepository;
        _porudzbinaRepository = porudzbinaRepository;
        _porukaRepository = porukaRepository;
        _rezervacijaRepository = rezervacijaRepository;
        _korisnikAdminRepository = korisnikAdminRepository;
        _recenzijaRepository = recenzijaRepository;
        _prijavaRepository = prijavaRepository;
    }

    public async Task<MenadzerskiDashboardDto> ZaMenadzeraAsync()
    {
        var (od, doIsklj) = Danas();

        var zbir = await _izvestajRepository.ZbirPrihodaAsync(od, doIsklj);
        var ispodPraga = await _namirnicaRepository.ListirajIspodPragaAsync();

        var (_, novihPrijava) = await _prijavaRepository.PretraziAsync(
            StatusPrijaveProblema.Nova, null, null, strana: 1, velicinaStrane: 1);

        return new MenadzerskiDashboardDto
        {
            DanasnjiPrihod = zbir.UkupanPrihod,
            BrojZatvorenihRacunaDanas = zbir.BrojPorudzbina,
            ProsecanRacunDanas = zbir.BrojPorudzbina == 0
                ? 0
                : Math.Round(zbir.UkupanPrihod / zbir.BrojPorudzbina, 2),
            DanasnjaNapojnica = zbir.UkupnaNapojnica,
            BrojOtvorenihStolova = await _porudzbinaRepository.BrojOtvorenihAsync(),
            BrojNamirnicaIspodPraga = ispodPraga.Count,
            NamirniceIspodPraga = ispodPraga.Take(MaksNamirnicaUPregledu).Select(n => n.Naziv).ToList(),
            BrojNovihPrijavaProblema = novihPrijava
        };
    }

    public async Task<AdministratorskiDashboardDto> ZaAdministratoraAsync()
    {
        var (od, doIsklj) = Danas();

        return new AdministratorskiDashboardDto
        {
            BrojNovihPoruka = await _porukaRepository.BrojPoStatusuAsync(StatusPoruke.Novo),
            BrojPrioritetnihPoruka = await _porukaRepository.BrojPrioritetnihNovihAsync(),
            BrojAktivnihRezervacijaDanas = await _rezervacijaRepository.BrojAktivnihUOpseguAsync(od, doIsklj),
            BrojOtvorenihStolova = await _porudzbinaRepository.BrojOtvorenihAsync(),
            BrojBlokiranihKorisnika = await _korisnikAdminRepository.BrojBlokiranihAsync(),
            BrojRecenzijaBezOdgovora = await _recenzijaRepository.BrojBezOdgovoraAsync()
        };
    }

    private static (DateTime Od, DateTime DoIskljucivo) Danas()
    {
        var danas = DateTime.UtcNow.Date;
        return (danas, danas.AddDays(1));
    }
}
