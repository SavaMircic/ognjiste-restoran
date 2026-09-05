namespace Services.DTO;

public class KorisnikProfilDto
{
    public string Id { get; set; } = string.Empty;
    public string Ime { get; set; } = string.Empty;
    public string Prezime { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? BrojTelefona { get; set; }
    public string? SlikaUrl { get; set; }
    public DateTime DatumRegistracije { get; set; }
}
