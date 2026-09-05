namespace Services.DTO;

public class KreirajZaposlenogDto
{
    public string Ime { get; set; } = string.Empty;
    public string Prezime { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PrivremenaLozinka { get; set; } = string.Empty;
    public string Uloga { get; set; } = string.Empty;
    public DateTime DatumZaposlenja { get; set; }
}
