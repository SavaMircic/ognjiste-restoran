using Domain.Enumi;

namespace Domain.Entiteti;

public class Recenzija
{
    public int Id { get; set; }

    public string KorisnikId { get; set; } = string.Empty;
    public Korisnik Korisnik { get; set; } = null!;

    public TipRecenzije TipRecenzije { get; set; }

    public int? StavkaMenijaId { get; set; }
    public StavkaMenija? StavkaMenija { get; set; }

    public int Ocena { get; set; }
    public string? Naslov { get; set; }
    public string Tekst { get; set; } = string.Empty;

    public DateTime DatumKreiranja { get; set; } = DateTime.UtcNow;
    public DateTime? DatumIzmene { get; set; }

    public bool Aktivan { get; set; } = true;

    public string? OdgovorRestorana { get; set; }
    public DateTime? DatumOdgovora { get; set; }
}
