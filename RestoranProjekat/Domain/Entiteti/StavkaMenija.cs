namespace Domain.Entiteti;

public class StavkaMenija
{
    public int Id { get; set; }
    public string Naziv { get; set; } = string.Empty;
    public string Opis { get; set; } = string.Empty;

    public string? DetaljanOpis { get; set; }
    public decimal Cena { get; set; }
    public string SlikaUrl { get; set; } = string.Empty;

    public int KategorijaId { get; set; }
    public KategorijaMenija Kategorija { get; set; } = null!;

    public bool Dostupno { get; set; } = true;

    public decimal? Popust { get; set; }

    public decimal CenaSaPopustom => Popust.HasValue
        ? Math.Round(Cena * (1 - Popust.Value / 100m), 2, MidpointRounding.AwayFromZero)
        : Cena;

    public DateTime DatumKreiranja { get; set; } = DateTime.UtcNow;

    public List<Receptura> Receptura { get; set; } = new();

    public List<SlikaStavkeMenija> DodatneSlike { get; set; } = new();
}
