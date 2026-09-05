using Domain.Enumi;

namespace Services.DTO;

public class PorukaDto
{
    public int Id { get; set; }
    public string? KorisnikId { get; set; }
    public string PosiljalacIme { get; set; } = string.Empty;
    public string? PosiljalacEmail { get; set; }
    public string? PosiljalacTelefon { get; set; }
    public KategorijaPoruke Kategorija { get; set; }
    public string Tekst { get; set; } = string.Empty;
    public DateTime DatumSlanja { get; set; }
    public StatusPoruke Status { get; set; }
    public string? Odgovor { get; set; }
    public DateTime? DatumOdgovora { get; set; }

    public DateTime? ZeljeniDatumVreme { get; set; }
    public int? ZeljeniBrojGostiju { get; set; }

    public bool Prioritetna { get; set; }
}
