using Domain.Enumi;

namespace Domain.Entiteti;

public class KategorijaMenija
{
    public int Id { get; set; }
    public string Naziv { get; set; } = string.Empty;
    public string? Opis { get; set; }
    public int Redosled { get; set; }
    public Odrediste Odrediste { get; set; }

    public List<StavkaMenija> Stavke { get; set; } = new();
}
