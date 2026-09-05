using Domain.Enumi;

namespace Services.DTO;

public class RezervacijaDto
{
    public int Id { get; set; }
    public string? KorisnikId { get; set; }
    public string ImeZaPrikaz { get; set; } = string.Empty;
    public int StoId { get; set; }
    public int BrojStola { get; set; }

    public List<int> BrojeviStolova { get; set; } = new();

    public DateTime DatumVreme { get; set; }

    public DateTime VaziOd { get; set; }
    public DateTime VaziDo { get; set; }

    public DateTime TolerancijaDo { get; set; }
    public int BrojGostiju { get; set; }
    public string KodRezervacije { get; set; } = string.Empty;
    public StatusRezervacije Status { get; set; }
    public NacinKreiranjaRezervacije NacinKreiranja { get; set; }
    public DateTime DatumKreiranja { get; set; }
}
