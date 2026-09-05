namespace Domain.Entiteti;

public class Bonus
{
    public int Id { get; set; }

    public int ZaposleniId { get; set; }
    public Zaposleni Zaposleni { get; set; } = null!;

    public decimal Iznos { get; set; }

    public DateOnly DatumPocetka { get; set; }
    public DateOnly DatumKraja { get; set; }

    public string Razlog { get; set; } = string.Empty;

    public int DodelioZaposleniId { get; set; }
    public Zaposleni DodelioZaposleni { get; set; } = null!;

    public DateTime DatumDodele { get; set; } = DateTime.UtcNow;
}
