namespace Services.DTO;

public class AuditLogDto
{
    public int Id { get; set; }
    public string? KorisnikId { get; set; }
    public string? KorisnikEmail { get; set; }
    public string NazivSlucajaKoriscenja { get; set; } = string.Empty;
    public DateTime Datum { get; set; }
    public bool UspesnoIzvrseno { get; set; }
    public string? Poruka { get; set; }
}
