namespace Services.DTO;

public class BonusiPretragaDto : PaginacijaParametriDto
{
    public int? ZaposleniId { get; set; }
    public DateOnly? DatumOd { get; set; }
    public DateOnly? DatumDo { get; set; }
}
