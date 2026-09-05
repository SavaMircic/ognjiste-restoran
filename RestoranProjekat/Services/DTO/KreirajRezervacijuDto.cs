namespace Services.DTO;

public class KreirajRezervacijuDto
{
    public List<int> StoIds { get; set; } = new();

    public DateTime DatumVreme { get; set; }
    public int BrojGostiju { get; set; }
}
