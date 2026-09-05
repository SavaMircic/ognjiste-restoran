using Domain.Enumi;

namespace Services.DTO;

public class StavkaPorudzbineDto
{
    public int Id { get; set; }
    public int StavkaMenijaId { get; set; }
    public string NazivStavke { get; set; } = string.Empty;
    public int Kolicina { get; set; }
    public string? Napomena { get; set; }
    public decimal CenaUTrenutkuNarudzbine { get; set; }
    public StatusStavkePorudzbine Status { get; set; }
    public int? PripremioZaposleniId { get; set; }
    public DateTime VremeSlanja { get; set; }
    public DateTime? VremePreuzimanja { get; set; }
    public DateTime? VremeZavrsetka { get; set; }
}
