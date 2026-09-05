using API.Autorizacija;
using Domain.Konstante;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services;

namespace API.Controllers;

[ApiController]
[Route("api/menadzment/dashboard")]
[Authorize(Roles = Uloge.Menadzer)]
public class MenadzmentDashboardController : ControllerBase
{
    private readonly IDashboardServis _dashboardServis;

    public MenadzmentDashboardController(IDashboardServis dashboardServis)
    {
        _dashboardServis = dashboardServis;
    }

    [HttpGet]
    [SlucajKoriscenja("MenadzerskiDashboard", Uloge.Menadzer)]
    public async Task<IActionResult> Pregled() => Ok(await _dashboardServis.ZaMenadzeraAsync());
}

[ApiController]
[Route("api/administracija/dashboard")]
[Authorize(Roles = Uloge.Administrator)]
public class AdministracijaDashboardController : ControllerBase
{
    private readonly IDashboardServis _dashboardServis;

    public AdministracijaDashboardController(IDashboardServis dashboardServis)
    {
        _dashboardServis = dashboardServis;
    }

    [HttpGet]
    [SlucajKoriscenja("AdministratorskiDashboard", Uloge.Administrator)]
    public async Task<IActionResult> Pregled() => Ok(await _dashboardServis.ZaAdministratoraAsync());
}
