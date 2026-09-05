using Domain.Enumi;

namespace Services.DTO;

public class RezervacijePretragaDto : PaginacijaParametriDto
{
    public DateTime? Datum { get; set; }
    public StatusRezervacije? Status { get; set; }
}
