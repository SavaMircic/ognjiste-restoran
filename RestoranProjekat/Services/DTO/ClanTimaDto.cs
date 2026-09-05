namespace Services.DTO;

public class ClanTimaDto
{
    public string Ime { get; set; } = string.Empty;
    public string Prezime { get; set; } = string.Empty;

    public string Uloga { get; set; } = string.Empty;

    public string? SlikaUrl { get; set; }
    public string? Biografija { get; set; }
    public string? LinkedInUrl { get; set; }
    public string? InstagramUrl { get; set; }
}
