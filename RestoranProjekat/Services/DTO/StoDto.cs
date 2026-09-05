using Domain.Enumi;

namespace Services.DTO;

public class StoDto
{
    public int Id { get; set; }
    public int BrojStola { get; set; }
    public int Kapacitet { get; set; }
    public StatusStola TrenutniStatus { get; set; }
}
