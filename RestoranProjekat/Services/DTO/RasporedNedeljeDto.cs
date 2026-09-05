namespace Services.DTO;

public class RasporedNedeljeDto
{
    public DateOnly PocetakNedelje { get; set; }
    public DateOnly KrajNedelje { get; set; }
    public List<RasporedZaposlenogDto> Zaposleni { get; set; } = new();
}
