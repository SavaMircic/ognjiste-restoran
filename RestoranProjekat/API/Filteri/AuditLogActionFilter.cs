using System.Security.Claims;
using System.Text.Json;
using API.Autorizacija;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using Services;

namespace API.Filteri;

public class AuditLogActionFilter : IAsyncActionFilter
{
    private const int MaksDuzinaPoruke = 1000;

    private readonly IAuditLogServis _auditLogServis;

    public AuditLogActionFilter(IAuditLogServis auditLogServis)
    {
        _auditLogServis = auditLogServis;
    }

    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var izvrsenaAkcija = await next();

        if (context.ActionDescriptor is not ControllerActionDescriptor opis) return;

        var oznaka = opis.MethodInfo
            .GetCustomAttributes(typeof(SlucajKoriscenjaAttribute), false)
            .FirstOrDefault() as SlucajKoriscenjaAttribute;

        if (oznaka == null) return;

        var korisnikId = context.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);

        var uspesno = izvrsenaAkcija.Exception == null && izvrsenaAkcija.Result switch
        {
            ObjectResult objekat => objekat.StatusCode is null or < 400,
            StatusCodeResult status => status.StatusCode < 400,
            _ => true
        };

        var poruka = uspesno ? null : OpisNeuspeha(izvrsenaAkcija);

        await _auditLogServis.ZabeleziAsync(korisnikId, oznaka.Naziv, uspesno, poruka);
    }

    private static string? OpisNeuspeha(ActionExecutedContext izvrsenaAkcija)
    {
        if (izvrsenaAkcija.Exception != null)
            return $"Neuhvaćen izuzetak: {izvrsenaAkcija.Exception.GetType().Name}";

        if (izvrsenaAkcija.Result is not ObjectResult { Value: not null } rezultat)
            return null;

        try
        {
            return Skrati(JsonSerializer.Serialize(rezultat.Value));
        }
        catch (NotSupportedException)
        {
            return Skrati(rezultat.Value.ToString());
        }
    }

    private static string? Skrati(string? tekst) =>
        tekst != null && tekst.Length > MaksDuzinaPoruke ? tekst[..MaksDuzinaPoruke] : tekst;
}
