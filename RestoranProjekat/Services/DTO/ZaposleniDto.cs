namespace Services.DTO;

public class ZaposleniDto
{
    public int Id { get; set; }
    public string KorisnikId { get; set; } = string.Empty;
    public string Ime { get; set; } = string.Empty;
    public string Prezime { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Uloga { get; set; } = string.Empty;
    public DateTime DatumZaposlenja { get; set; }
    public bool Aktivan { get; set; }

    public string? SlikaUrl { get; set; }
    public string? Biografija { get; set; }
    public string? LinkedInUrl { get; set; }
    public string? InstagramUrl { get; set; }
    public int Redosled { get; set; }
    public bool PrikaziNaSajtu { get; set; }
}
