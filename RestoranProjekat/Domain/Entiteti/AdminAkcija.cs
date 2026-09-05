namespace Domain.Entiteti;

public class AdminAkcija
{
    public int Id { get; set; }

    public int AdministratorId { get; set; }
    public Zaposleni Administrator { get; set; } = null!;

    public string TipAkcije { get; set; } = string.Empty;

    public string? CiljniKorisnikId { get; set; }
    public Korisnik? CiljniKorisnik { get; set; }

    public string Opis { get; set; } = string.Empty;

    public DateTime Datum { get; set; } = DateTime.UtcNow;
}
