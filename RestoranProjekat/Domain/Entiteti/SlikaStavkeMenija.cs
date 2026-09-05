namespace Domain.Entiteti;

public class SlikaStavkeMenija
{
    public int Id { get; set; }

    public int StavkaMenijaId { get; set; }
    public StavkaMenija StavkaMenija { get; set; } = null!;

    public string SlikaUrl { get; set; } = string.Empty;

    public string? Opis { get; set; }

    public int Redosled { get; set; }

    public DateTime DatumDodavanja { get; set; } = DateTime.UtcNow;
}
