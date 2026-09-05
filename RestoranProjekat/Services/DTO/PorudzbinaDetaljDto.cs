using Domain.Enumi;

namespace Services.DTO;

public class PorudzbinaDetaljDto
{
    public int Id { get; set; }
    public int StoId { get; set; }
    public int BrojStola { get; set; }
    public int KonobarId { get; set; }
    public string KonobarIme { get; set; } = string.Empty;
    public DateTime VremeOtvaranja { get; set; }
    public DateTime? VremeZatvaranja { get; set; }
    public StatusPorudzbine Status { get; set; }
    public NacinPlacanja? NacinPlacanja { get; set; }
    public decimal? IznosNapojnice { get; set; }
    public List<StavkaPorudzbineDto> Stavke { get; set; } = new();
}
