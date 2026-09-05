using Domain.Entiteti;
using Repository.Rezultati;

namespace Repository.Interfejsi;

public interface IIzvestajRepository
{
    Task<List<PrihodPoDanuRezultat>> PrihodPoDanuAsync(DateTime od, DateTime doIsklj);

    Task<List<PrihodPoGrupiRezultat>> PrihodPoKategorijiAsync(DateTime od, DateTime doIsklj);

    Task<List<PrihodPoGrupiRezultat>> PrihodPoArtikluAsync(DateTime od, DateTime doIsklj);

    Task<ZbirPrihodaRezultat> ZbirPrihodaAsync(DateTime od, DateTime doIsklj);

    Task<List<UcinakKonobaraRezultat>> UcinakKonobaraAsync(DateTime od, DateTime doIsklj, int? zaposleniId);

    Task<List<UcinakPripremeRezultat>> UcinakPripremeAsync(DateTime od, DateTime doIsklj, int? zaposleniId);

    Task<List<TopJeloRezultat>> ProdajaPoArtikluAsync(DateTime od, DateTime doIsklj);

    Task<List<Porudzbina>> MojiZatvoreniRacuniAsync(DateTime od, DateTime doIsklj, int zaposleniId);

    Task<List<StavkaPorudzbine>> MojePripremljeneStavkeAsync(DateTime od, DateTime doIsklj, int zaposleniId);
}
