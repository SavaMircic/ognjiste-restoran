using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace API.Greske;

public class GlobalniHendlerGresaka : IExceptionHandler
{
    private readonly ILogger<GlobalniHendlerGresaka> _logger;

    public GlobalniHendlerGresaka(ILogger<GlobalniHendlerGresaka> logger)
    {
        _logger = logger;
    }

    public async ValueTask<bool> TryHandleAsync(
        HttpContext kontekst, Exception izuzetak, CancellationToken token)
    {
        _logger.LogError(izuzetak, "Neuhvaćena greška na {Metod} {Putanja}",
            kontekst.Request.Method, kontekst.Request.Path);

        var odgovor = new ObjectResult(new
        {
            Poruka = "Došlo je do neočekivane greške. Pokušajte ponovo.",
            Greske = new Dictionary<string, string[]>()
        })
        {
            StatusCode = StatusCodes.Status500InternalServerError
        };

        kontekst.Response.StatusCode = StatusCodes.Status500InternalServerError;
        await kontekst.Response.WriteAsJsonAsync(odgovor.Value, token);

        return true;
    }
}
