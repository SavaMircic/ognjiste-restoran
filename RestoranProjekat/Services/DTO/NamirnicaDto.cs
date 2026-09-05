using Domain.Enumi;

namespace Services.DTO;

public class NamirnicaDto
{
    public int Id { get; set; }
    public string Naziv { get; set; } = string.Empty;
    public JedinicaMere JedinicaMere { get; set; }
    public decimal TrenutnaKolicina { get; set; }
    public decimal MinimalniPrag { get; set; }

    public bool IspodPraga { get; set; }
}
