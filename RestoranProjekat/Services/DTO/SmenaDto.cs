namespace Services.DTO;

public class SmenaDto
{
    public int Id { get; set; }
    public int ZaposleniId { get; set; }
    public string ImeZaposlenog { get; set; } = string.Empty;

    public DateOnly Datum { get; set; }
    public TimeOnly VremePocetka { get; set; }
    public TimeOnly VremeKraja { get; set; }

    public decimal TrajanjeSati { get; set; }

    public bool PrelaziPonoc { get; set; }

    public bool Prosla { get; set; }
}
