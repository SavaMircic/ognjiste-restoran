namespace Domain.Entiteti;

public class LajkJela
{
    public int Id { get; set; }

    public string KorisnikId { get; set; } = string.Empty;
    public Korisnik Korisnik { get; set; } = null!;

    public int StavkaMenijaId { get; set; }
    public StavkaMenija StavkaMenija { get; set; } = null!;

    public DateTime DatumKreiranja { get; set; } = DateTime.UtcNow;
}
