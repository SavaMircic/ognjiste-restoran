using Domain.Enumi;

namespace Services.DTO;

public class KreirajNamirnicuDto
{
    public string Naziv { get; set; } = string.Empty;
    public JedinicaMere JedinicaMere { get; set; }
    public decimal MinimalniPrag { get; set; }
    public decimal PocetnaKolicina { get; set; }
}
