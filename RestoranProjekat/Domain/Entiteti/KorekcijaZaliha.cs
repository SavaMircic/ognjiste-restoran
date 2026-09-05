namespace Domain.Entiteti;

public class KorekcijaZaliha
{
    public int Id { get; set; }

    public int NamirnicaId { get; set; }
    public Namirnica Namirnica { get; set; } = null!;

    public decimal StaraKolicina { get; set; }
    public decimal NovaKolicina { get; set; }
    public string Razlog { get; set; } = string.Empty;

    public int ZaposleniId { get; set; }
    public Zaposleni Zaposleni { get; set; } = null!;

    public DateTime Datum { get; set; } = DateTime.UtcNow;
}
