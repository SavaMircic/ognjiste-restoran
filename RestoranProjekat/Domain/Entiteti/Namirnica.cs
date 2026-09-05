using Domain.Enumi;

namespace Domain.Entiteti;

public class Namirnica
{
    public int Id { get; set; }
    public string Naziv { get; set; } = string.Empty;
    public JedinicaMere JedinicaMere { get; set; }
    public decimal TrenutnaKolicina { get; set; }

    public decimal MinimalniPrag { get; set; }
}
