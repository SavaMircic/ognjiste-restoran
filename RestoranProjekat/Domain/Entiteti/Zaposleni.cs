namespace Domain.Entiteti;

public class Zaposleni
{
    public int Id { get; set; }

    public string KorisnikId { get; set; } = string.Empty;
    public Korisnik Korisnik { get; set; } = null!;

    public DateTime DatumZaposlenja { get; set; }

    public string? SlikaUrl { get; set; }

    public string? Biografija { get; set; }

    public string? LinkedInUrl { get; set; }
    public string? InstagramUrl { get; set; }

    public int Redosled { get; set; }

    public bool PrikaziNaSajtu { get; set; }
}
