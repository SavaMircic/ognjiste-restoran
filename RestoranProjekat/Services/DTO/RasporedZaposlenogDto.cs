namespace Services.DTO;

public class RasporedZaposlenogDto
{
    public int ZaposleniId { get; set; }
    public string ImeZaposlenog { get; set; } = string.Empty;
    public List<SmenaDto> Smene { get; set; } = new();

    public decimal UkupnoSati { get; set; }
}
