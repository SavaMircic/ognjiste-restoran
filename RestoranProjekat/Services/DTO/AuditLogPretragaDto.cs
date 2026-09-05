namespace Services.DTO;

public class AuditLogPretragaDto : PaginacijaParametriDto
{
    public string? KorisnikId { get; set; }

    public string? NazivSlucajaKoriscenja { get; set; }
    public DateTime? DatumOd { get; set; }
    public DateTime? DatumDo { get; set; }
}
