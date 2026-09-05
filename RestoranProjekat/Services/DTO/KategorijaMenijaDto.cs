using Domain.Enumi;

namespace Services.DTO;

public class KategorijaMenijaDto
{
    public int Id { get; set; }
    public string Naziv { get; set; } = string.Empty;
    public string? Opis { get; set; }
    public int Redosled { get; set; }
    public Odrediste Odrediste { get; set; }
}
