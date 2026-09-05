using Domain.Enumi;

namespace Domain.Entiteti;

public class Porudzbina
{
    public int Id { get; set; }

    public int StoId { get; set; }
    public Sto Sto { get; set; } = null!;

    public int KonobarId { get; set; }
    public Zaposleni Konobar { get; set; } = null!;

    public int? RezervacijaId { get; set; }
    public Rezervacija? Rezervacija { get; set; }

    public DateTime VremeOtvaranja { get; set; } = DateTime.UtcNow;
    public DateTime? VremeZatvaranja { get; set; }
    public StatusPorudzbine Status { get; set; } = StatusPorudzbine.Otvorena;
    public NacinPlacanja? NacinPlacanja { get; set; }
    public decimal? IznosNapojnice { get; set; }

    public List<StavkaPorudzbine> Stavke { get; set; } = new();
}
