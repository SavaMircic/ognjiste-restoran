namespace Services.DTO;

public class ResetLozinkeDto
{
    public string Email { get; set; } = string.Empty;
    public string Token { get; set; } = string.Empty;
    public string NovaLozinka { get; set; } = string.Empty;
}
