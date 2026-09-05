using Domain.Enumi;

namespace Domain.Entiteti;

public class StavkaPorudzbine
{
    public int Id { get; set; }

    public int PorudzbinaId { get; set; }
    public Porudzbina Porudzbina { get; set; } = null!;

    public int StavkaMenijaId { get; set; }
    public StavkaMenija StavkaMenija { get; set; } = null!;

    public int Kolicina { get; set; }
    public string? Napomena { get; set; }

    public decimal CenaUTrenutkuNarudzbine { get; set; }

    public StatusStavkePorudzbine Status { get; set; } = StatusStavkePorudzbine.Poslato;

    public int? PripremioZaposleniId { get; set; }
    public Zaposleni? PripremioZaposleni { get; set; }

    public DateTime VremeSlanja { get; set; } = DateTime.UtcNow;
    public DateTime? VremePreuzimanja { get; set; }
    public DateTime? VremeZavrsetka { get; set; }
}
