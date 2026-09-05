using Domain.Enumi;

namespace Services.DTO;

public class PorukePretragaDto : PaginacijaParametriDto
{
    public StatusPoruke? Status { get; set; }
    public KategorijaPoruke? Kategorija { get; set; }
}
