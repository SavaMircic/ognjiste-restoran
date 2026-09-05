using Domain.Enumi;

namespace Services.DTO;

public class PrijavaProblemaDto
{
    public int Id { get; set; }
    public int PrijavioZaposleniId { get; set; }
    public string PrijavioIme { get; set; } = string.Empty;
    public string Naslov { get; set; } = string.Empty;
    public string Opis { get; set; } = string.Empty;
    public KategorijaProblema Kategorija { get; set; }
    public PrioritetProblema Prioritet { get; set; }
    public StatusPrijaveProblema Status { get; set; }
    public DateTime DatumPrijave { get; set; }
    public string? ResioIme { get; set; }
    public string? Odgovor { get; set; }
    public DateTime? DatumResavanja { get; set; }
}

public class PrijaviProblemDto
{
    public string Naslov { get; set; } = string.Empty;
    public string Opis { get; set; } = string.Empty;
    public KategorijaProblema Kategorija { get; set; }
    public PrioritetProblema Prioritet { get; set; } = PrioritetProblema.Srednji;
}

public class ResiPrijavuProblemaDto
{
    public StatusPrijaveProblema Status { get; set; }
    public string? Odgovor { get; set; }
}

public class PrijaveProblemaPretragaDto : PaginacijaParametriDto
{
    public StatusPrijaveProblema? Status { get; set; }
    public KategorijaProblema? Kategorija { get; set; }
    public PrioritetProblema? Prioritet { get; set; }
}
