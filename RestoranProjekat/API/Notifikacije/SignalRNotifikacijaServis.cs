using API.Hubs;
using Domain.Enumi;
using Microsoft.AspNetCore.SignalR;
using Services;
using Services.DTO;

namespace API.Notifikacije;

public class SignalRNotifikacijaServis : INotifikacijaServis
{
    private readonly IHubContext<KuhinjaHub> _hub;

    public SignalRNotifikacijaServis(IHubContext<KuhinjaHub> hub)
    {
        _hub = hub;
    }

    public async Task PosaljiNovaStavkaAsync(Odrediste odrediste, NovaStavkaNotifikacijaDto podaci)
    {
        var grupa = odrediste == Odrediste.Kuhinja ? "kuhinja" : "sank";
        await _hub.Clients.Group(grupa).SendAsync("NovaStavka", podaci);
    }

    public async Task PosaljiStavkaSpremnaAsync(string konobarKorisnikId, StavkaSpremnaNotifikacijaDto podaci)
    {
        await _hub.Clients.Group($"konobar-{konobarKorisnikId}").SendAsync("StavkaSpremna", podaci);
    }
}
