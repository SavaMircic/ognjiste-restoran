namespace Services.DTO;

public class StavkaSpremnaNotifikacijaDto
{
    public int StavkaPorudzbineId { get; set; }
    public int BrojStola { get; set; }
    public string NazivStavke { get; set; } = string.Empty;
}
