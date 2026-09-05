namespace Domain.Entiteti;

public class AuditLog
{
    public int Id { get; set; }

    public string? KorisnikId { get; set; }
    public Korisnik? Korisnik { get; set; }

    public string NazivSlucajaKoriscenja { get; set; } = string.Empty;

    public DateTime Datum { get; set; } = DateTime.UtcNow;
    public bool UspesnoIzvrseno { get; set; }
    public string? Poruka { get; set; }
}
