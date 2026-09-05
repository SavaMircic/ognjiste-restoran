namespace Services.DTO;

public class NovaStavkaNotifikacijaDto
{
    public int StavkaPorudzbineId { get; set; }
    public int BrojStola { get; set; }
    public string NazivStavke { get; set; } = string.Empty;
    public int Kolicina { get; set; }
    public string? Napomena { get; set; }
}
