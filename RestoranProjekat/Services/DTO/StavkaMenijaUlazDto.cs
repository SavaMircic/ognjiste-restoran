namespace Services.DTO;

public class StavkaMenijaUlazDto
{
    public string Naziv { get; set; } = string.Empty;

    public string Opis { get; set; } = string.Empty;

    public string? DetaljanOpis { get; set; }

    public decimal Cena { get; set; }
    public int KategorijaId { get; set; }
    public decimal? Popust { get; set; }
}
