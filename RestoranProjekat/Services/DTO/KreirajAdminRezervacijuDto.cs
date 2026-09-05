using Domain.Enumi;

namespace Services.DTO;

public class KreirajAdminRezervacijuDto
{
    public int StoId { get; set; }
    public DateTime DatumVreme { get; set; }
    public int BrojGostiju { get; set; }
    public NacinKreiranjaRezervacije NacinKreiranja { get; set; }
    public string? KorisnikId { get; set; }
    public string? GostIme { get; set; }
    public string? GostEmail { get; set; }
    public string? GostTelefon { get; set; }
}
