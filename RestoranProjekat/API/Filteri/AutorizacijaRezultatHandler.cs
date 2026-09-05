using System.Security.Claims;
using API.Autorizacija;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Policy;
using Services;

namespace API.Filteri;

public class AutorizacijaRezultatHandler : IAuthorizationMiddlewareResultHandler
{
    private readonly AuthorizationMiddlewareResultHandler _podrazumevani = new();

    public async Task HandleAsync(
        RequestDelegate next, HttpContext context,
        AuthorizationPolicy policy, PolicyAuthorizationResult rezultat)
    {
        if (rezultat.Forbidden || rezultat.Challenged)
            await ZabeleziOdbijanjeAsync(context, rezultat);

        await _podrazumevani.HandleAsync(next, context, policy, rezultat);
    }

    private static async Task ZabeleziOdbijanjeAsync(HttpContext context, PolicyAuthorizationResult rezultat)
    {
        var oznaka = context.GetEndpoint()?.Metadata.GetMetadata<SlucajKoriscenjaAttribute>();
        if (oznaka == null) return;

        var auditLogServis = context.RequestServices.GetService<IAuditLogServis>();
        if (auditLogServis == null) return;

        var korisnikId = context.User.FindFirstValue(ClaimTypes.NameIdentifier);
        var poruka = rezultat.Challenged
            ? "Odbijen pristup: zahtev nije autentifikovan."
            : $"Odbijen pristup: korisnik nema potrebnu ulogu za ovu radnju.";

        await auditLogServis.ZabeleziAsync(korisnikId, oznaka.Naziv, uspesno: false, poruka);
    }
}
