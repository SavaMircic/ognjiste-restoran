namespace Domain.Entiteti;

public class Receptura
{
    public int Id { get; set; }

    public int StavkaMenijaId { get; set; }
    public StavkaMenija StavkaMenija { get; set; } = null!;

    public int NamirnicaId { get; set; }
    public Namirnica Namirnica { get; set; } = null!;

    public decimal Kolicina { get; set; }
}
