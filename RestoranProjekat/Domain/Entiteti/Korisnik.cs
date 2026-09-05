using Microsoft.AspNetCore.Identity;

namespace Domain.Entiteti;

public class Korisnik : IdentityUser
{
    public string Ime { get; set; } = string.Empty;
    public string Prezime { get; set; } = string.Empty;
    public DateTime DatumRegistracije { get; set; } = DateTime.UtcNow;

    public bool Aktivan { get; set; } = true;

    public DateTime? BlokiranDo { get; set; }

    public DateTime? ZabranaKomentarisanjaDo { get; set; }

    public string? SlikaUrl { get; set; }

    public string? RefreshToken { get; set; }
    public DateTime? RefreshTokenIstice { get; set; }

    public Zaposleni? Zaposleni { get; set; }
}
