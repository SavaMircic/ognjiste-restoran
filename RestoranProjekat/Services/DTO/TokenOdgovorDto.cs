namespace Services.DTO;

public class TokenOdgovorDto
{
    public string AccessToken { get; set; } = string.Empty;
    public string RefreshToken { get; set; } = string.Empty;
    public DateTime IsticeAccessToken { get; set; }
    public string KorisnikId { get; set; } = string.Empty;
    public string Ime { get; set; } = string.Empty;
    public string Prezime { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public List<string> Uloge { get; set; } = new();
}
