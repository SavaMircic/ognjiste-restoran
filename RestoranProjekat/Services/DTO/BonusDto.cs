namespace Services.DTO;

public class BonusDto
{
    public int Id { get; set; }
    public int ZaposleniId { get; set; }
    public string ImeZaposlenog { get; set; } = string.Empty;

    public decimal Iznos { get; set; }
    public DateOnly DatumPocetka { get; set; }
    public DateOnly DatumKraja { get; set; }
    public string Razlog { get; set; } = string.Empty;

    public int DodelioZaposleniId { get; set; }
    public string DodelioIme { get; set; } = string.Empty;
    public DateTime DatumDodele { get; set; }

    public bool Vazi { get; set; }
}
