using System.Security.Claims;
using API.Autorizacija;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using Services;

namespace API.Filteri;

public class SlucajKoriscenjaAutorizacijaFilter : IAsyncAuthorizationFilter
{
    private readonly IAuditLogServis _auditLogServis;

    public SlucajKoriscenjaAutorizacijaFilter(IAuditLogServis auditLogServis)
    {
        _auditLogServis = auditLogServis;
    }

    public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
    {
        if (context.ActionDescriptor is not ControllerActionDescriptor opis) return;

        var oznaka = opis.MethodInfo
            .GetCustomAttributes(typeof(SlucajKoriscenjaAttribute), false)
            .FirstOrDefault() as SlucajKoriscenjaAttribute;

        if (oznaka == null || oznaka.DozvoljeneUloge.Length == 0)
            return;

        var korisnikoveUloge = context.HttpContext.User.FindAll(ClaimTypes.Role).Select(c => c.Value).ToHashSet();

        if (oznaka.DozvoljeneUloge.Any(korisnikoveUloge.Contains)) return;

        var korisnikId = context.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
        var imaoUloge = korisnikoveUloge.Count == 0 ? "bez uloge" : string.Join(", ", korisnikoveUloge);
        await _auditLogServis.ZabeleziAsync(
            korisnikId, oznaka.Naziv, uspesno: false,
            poruka: $"Odbijen pristup: potrebna uloga [{string.Join(", ", oznaka.DozvoljeneUloge)}], korisnik ima [{imaoUloge}].");

        context.Result = new ObjectResult(new { Poruka = "Nemate pravo da izvršite ovu radnju." })
        {
            StatusCode = StatusCodes.Status403Forbidden
        };
    }
}
