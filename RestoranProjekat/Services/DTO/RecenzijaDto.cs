using Domain.Enumi;

namespace Services.DTO;

public class RecenzijaDto
{
    public int Id { get; set; }
    public string KorisnikId { get; set; } = string.Empty;
    public string AutorIme { get; set; } = string.Empty;

    public string? AutorSlikaUrl { get; set; }
    public TipRecenzije TipRecenzije { get; set; }
    public int? StavkaMenijaId { get; set; }
    public string? NazivStavke { get; set; }
    public int Ocena { get; set; }
    public string? Naslov { get; set; }
    public string Tekst { get; set; } = string.Empty;
    public DateTime DatumKreiranja { get; set; }
    public DateTime? DatumIzmene { get; set; }
    public string? OdgovorRestorana { get; set; }
    public DateTime? DatumOdgovora { get; set; }
}
