using Services.DTO;

namespace Services;

public interface IDashboardServis
{
    Task<MenadzerskiDashboardDto> ZaMenadzeraAsync();
    Task<AdministratorskiDashboardDto> ZaAdministratoraAsync();
}
