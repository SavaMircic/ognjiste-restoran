namespace Services.DTO;

public class ZaposleniPretragaDto : PaginacijaParametriDto
{
    public string? Uloga { get; set; }
    public bool? Aktivan { get; set; }
}
