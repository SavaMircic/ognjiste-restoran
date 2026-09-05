using Domain.Enumi;

namespace Domain.Entiteti;

public class PrijavaProblema
{
    public int Id { get; set; }

    public int PrijavioZaposleniId { get; set; }
    public Zaposleni PrijavioZaposleni { get; set; } = null!;

    public string Naslov { get; set; } = string.Empty;
    public string Opis { get; set; } = string.Empty;

    public KategorijaProblema Kategorija { get; set; }
    public PrioritetProblema Prioritet { get; set; } = PrioritetProblema.Srednji;
    public StatusPrijaveProblema Status { get; set; } = StatusPrijaveProblema.Nova;

    public DateTime DatumPrijave { get; set; } = DateTime.UtcNow;

    public int? ResioZaposleniId { get; set; }
    public Zaposleni? ResioZaposleni { get; set; }

    public string? Odgovor { get; set; }
    public DateTime? DatumResavanja { get; set; }
}
