using System.Security.Claims;
using Domain.Konstante;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace API.Hubs;

[Authorize]
public class KuhinjaHub : Hub
{
    public override async Task OnConnectedAsync()
    {
        var korisnikoveUloge = Context.User?.FindAll(ClaimTypes.Role).Select(c => c.Value).ToHashSet() ?? new HashSet<string>();
        var korisnikId = Context.User?.FindFirstValue(ClaimTypes.NameIdentifier);

        if (korisnikoveUloge.Contains(Uloge.Kuvar))
            await Groups.AddToGroupAsync(Context.ConnectionId, "kuhinja");

        if (korisnikoveUloge.Contains(Uloge.Sanker))
            await Groups.AddToGroupAsync(Context.ConnectionId, "sank");

        if (korisnikoveUloge.Contains(Uloge.Konobar) && korisnikId != null)
            await Groups.AddToGroupAsync(Context.ConnectionId, $"konobar-{korisnikId}");

        await base.OnConnectedAsync();
    }
}
