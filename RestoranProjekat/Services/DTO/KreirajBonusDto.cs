namespace Services.DTO;

public class KreirajBonusDto
{
    public List<int> ZaposleniIds { get; set; } = new();
    public decimal Iznos { get; set; }
    public DateOnly DatumPocetka { get; set; }
    public DateOnly DatumKraja { get; set; }
    public string Razlog { get; set; } = string.Empty;
}
