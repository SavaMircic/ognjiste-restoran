using Domain.Enumi;

namespace Services.DTO;

public class RedCekanjaStavkaDto
{
    public int Id { get; set; }
    public int BrojStola { get; set; }
    public string NazivStavke { get; set; } = string.Empty;
    public int Kolicina { get; set; }
    public string? Napomena { get; set; }
    public DateTime VremeSlanja { get; set; }
    public StatusStavkePorudzbine Status { get; set; }

    public string? PripremioIme { get; set; }

    public bool MojaStavka { get; set; }
}
