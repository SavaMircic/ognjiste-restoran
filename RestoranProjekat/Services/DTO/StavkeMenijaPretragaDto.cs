namespace Services.DTO;

public class StavkeMenijaPretragaDto : PaginacijaParametriDto
{
    public int? KategorijaId { get; set; }

    public string? Sortiranje { get; set; }
}
