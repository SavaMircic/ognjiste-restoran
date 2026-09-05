namespace Domain.Entiteti;

public class PostavkeRestorana
{
    public int Id { get; set; } = 1;

    public string Adresa { get; set; } = string.Empty;
    public string Telefon { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;

    public string RadnoVreme { get; set; } = string.Empty;

    public string OpisRestorana { get; set; } = string.Empty;

    public decimal? GeoSirina { get; set; }
    public decimal? GeoDuzina { get; set; }

    public string? FacebookUrl { get; set; }
    public string? InstagramUrl { get; set; }
}
