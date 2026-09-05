namespace Services.DTO;

public class StavkaMenijaDto
{
    public int Id { get; set; }
    public string Naziv { get; set; } = string.Empty;
    public string Opis { get; set; } = string.Empty;

    public string? DetaljanOpis { get; set; }

    public decimal Cena { get; set; }

    public string SlikaUrl { get; set; } = string.Empty;

    public List<SlikaStavkeMenijaDto> DodatneSlike { get; set; } = new();
    public int KategorijaId { get; set; }
    public string KategorijaNaziv { get; set; } = string.Empty;
    public bool Dostupno { get; set; }
    public decimal? Popust { get; set; }

    public decimal CenaSaPopustom { get; set; }

    public DateTime DatumKreiranja { get; set; }

    public double? ProsecnaOcena { get; set; }
    public int BrojRecenzija { get; set; }
    public int BrojLajkova { get; set; }
}
