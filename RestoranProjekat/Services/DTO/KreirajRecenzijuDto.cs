using Domain.Enumi;

namespace Services.DTO;

public class KreirajRecenzijuDto
{
    public TipRecenzije TipRecenzije { get; set; }

    public int? StavkaMenijaId { get; set; }

    public int Ocena { get; set; }
    public string? Naslov { get; set; }
    public string Tekst { get; set; } = string.Empty;
}
