using Services;

namespace API.Poslovi;

public class IstekRezervacijaPosao : BackgroundService
{
    private static readonly TimeSpan IntervalProvere = TimeSpan.FromMinutes(5);

    private const int MaksKodovaUPoruci = 10;

    private readonly IServiceScopeFactory _fabrikaOpsega;
    private readonly ILogger<IstekRezervacijaPosao> _logger;

    public IstekRezervacijaPosao(IServiceScopeFactory fabrikaOpsega, ILogger<IstekRezervacijaPosao> logger)
    {
        _fabrikaOpsega = fabrikaOpsega;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken tokenZaustavljanja)
    {
        using var tajmer = new PeriodicTimer(IntervalProvere);

        try
        {
            while (await tajmer.WaitForNextTickAsync(tokenZaustavljanja))
                await IzvrsiTikAsync();
        }
        catch (OperationCanceledException)
        {
        }
    }

    private async Task IzvrsiTikAsync()
    {
        try
        {
            using var opseg = _fabrikaOpsega.CreateScope();
            var rezervacijaServis = opseg.ServiceProvider.GetRequiredService<IRezervacijaServis>();

            var istekleRezervacije = await rezervacijaServis.IsteciDospeleAsync();
            if (istekleRezervacije.Count == 0) return;

            var opis = OpisSerije(istekleRezervacije);
            _logger.LogInformation("Automatsko isticanje rezervacija: {Opis}", opis);

            var auditLogServis = opseg.ServiceProvider.GetRequiredService<IAuditLogServis>();
            await auditLogServis.ZabeleziAsync(
                korisnikId: null,
                nazivSlucajaKoriscenja: "AutomatskoIsticanjeRezervacija",
                uspesno: true,
                poruka: opis);
        }
        catch (Exception izuzetak)
        {
            _logger.LogError(izuzetak, "Greška pri automatskom isticanju rezervacija.");
        }
    }

    private static string OpisSerije(List<string> kodovi)
    {
        var prikazani = string.Join(", ", kodovi.Take(MaksKodovaUPoruci));
        var ostatak = kodovi.Count - MaksKodovaUPoruci;

        return ostatak > 0
            ? $"Isteklo rezervacija: {kodovi.Count} ({prikazani} i još {ostatak})."
            : $"Isteklo rezervacija: {kodovi.Count} ({prikazani}).";
    }
}
