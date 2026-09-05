using Domain.Enumi;

namespace Services.DTO;

public class ReceptStavkaDto
{
    public int NamirnicaId { get; set; }
    public string NamirnicaNaziv { get; set; } = string.Empty;
    public JedinicaMere JedinicaMere { get; set; }
    public decimal Kolicina { get; set; }
}
