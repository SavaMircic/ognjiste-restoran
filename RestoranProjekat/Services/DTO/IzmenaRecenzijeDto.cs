namespace Services.DTO;

public class IzmenaRecenzijeDto
{
    public int Ocena { get; set; }
    public string? Naslov { get; set; }
    public string Tekst { get; set; } = string.Empty;
}
