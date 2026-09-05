namespace Domain.Entiteti;

public class Smena
{
    public int Id { get; set; }

    public int ZaposleniId { get; set; }
    public Zaposleni Zaposleni { get; set; } = null!;

    public DateOnly Datum { get; set; }

    public TimeOnly VremePocetka { get; set; }
    public TimeOnly VremeKraja { get; set; }
}
