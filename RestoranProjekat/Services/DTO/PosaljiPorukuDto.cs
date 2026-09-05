using Domain.Enumi;

namespace Services.DTO;

public class PosaljiPorukuDto
{
    public KategorijaPoruke Kategorija { get; set; }
    public string Tekst { get; set; } = string.Empty;

    public string? Ime { get; set; }
    public string? Email { get; set; }
    public string? Telefon { get; set; }

    public DateTime? ZeljeniDatumVreme { get; set; }
    public int? ZeljeniBrojGostiju { get; set; }
}
