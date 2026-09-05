namespace Services.DTO;

public class SlikaStavkeMenijaDto
{
    public int Id { get; set; }
    public string SlikaUrl { get; set; } = string.Empty;
    public string? Opis { get; set; }
    public int Redosled { get; set; }
}
