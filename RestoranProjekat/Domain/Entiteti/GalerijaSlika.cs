namespace Domain.Entiteti;

public class GalerijaSlika
{
    public int Id { get; set; }

    public string SlikaUrl { get; set; } = string.Empty;

    public string Naslov { get; set; } = string.Empty;
    public string? Opis { get; set; }

    public string Grupa { get; set; } = string.Empty;

    public int Redosled { get; set; }

    public bool Aktivan { get; set; } = true;

    public DateTime DatumDodavanja { get; set; } = DateTime.UtcNow;
}
