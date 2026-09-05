using Domain.Enumi;

namespace Services.DTO;

public class RecenzijePretragaDto : PaginacijaParametriDto
{
    public int? StavkaMenijaId { get; set; }
    public TipRecenzije? TipRecenzije { get; set; }

    public string? Sortiranje { get; set; }
}
