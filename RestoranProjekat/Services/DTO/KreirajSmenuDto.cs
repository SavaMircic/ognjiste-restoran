namespace Services.DTO;

public class KreirajSmenuDto
{
    public int ZaposleniId { get; set; }
    public DateOnly Datum { get; set; }

    public TimeOnly VremePocetka { get; set; }
    public TimeOnly VremeKraja { get; set; }
}
