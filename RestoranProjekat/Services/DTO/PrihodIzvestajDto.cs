using Domain.Enumi;

namespace Services.DTO;

public class PrihodIzvestajDto
{
    public DateOnly DatumOd { get; set; }
    public DateOnly DatumDo { get; set; }
    public GrupisanjePrihoda GrupisanjePo { get; set; }

    public decimal UkupanPrihod { get; set; }

    public decimal UkupnaNapojnica { get; set; }
    public int BrojPorudzbina { get; set; }

    public decimal ProsecanRacun { get; set; }

    public List<PrihodStavkaDto> Stavke { get; set; } = new();
}

public class PrihodStavkaDto
{
    public string Grupa { get; set; } = string.Empty;
    public decimal Prihod { get; set; }

    public int Kolicina { get; set; }

    public decimal UdeoProcenat { get; set; }
}
