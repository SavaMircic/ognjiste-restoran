using Domain.Enumi;

namespace Domain.Entiteti;

public class Sto
{
    public int Id { get; set; }
    public int BrojStola { get; set; }
    public int Kapacitet { get; set; }

    public StatusStola TrenutniStatus { get; set; } = StatusStola.Slobodan;

    public List<Porudzbina> Porudzbine { get; set; } = new();
}
