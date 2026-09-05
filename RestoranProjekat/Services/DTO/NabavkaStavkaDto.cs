using Domain.Enumi;

namespace Services.DTO;

public class NabavkaStavkaDto
{
    public int NamirnicaId { get; set; }
    public string Naziv { get; set; } = string.Empty;
    public JedinicaMere JedinicaMere { get; set; }
    public decimal TrenutnaKolicina { get; set; }
    public decimal MinimalniPrag { get; set; }

    public decimal PredlozenaKolicina { get; set; }
}
