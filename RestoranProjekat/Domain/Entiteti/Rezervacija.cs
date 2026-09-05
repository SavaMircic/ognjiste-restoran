using Domain.Enumi;

namespace Domain.Entiteti;

public class Rezervacija
{
    public int Id { get; set; }

    public string? KorisnikId { get; set; }
    public Korisnik? Korisnik { get; set; }

    public string? GostIme { get; set; }
    public string? GostEmail { get; set; }
    public string? GostTelefon { get; set; }

    public int StoId { get; set; }
    public Sto Sto { get; set; } = null!;

    public DateTime DatumVreme { get; set; }
    public int BrojGostiju { get; set; }
    public string KodRezervacije { get; set; } = string.Empty;
    public StatusRezervacije Status { get; set; } = StatusRezervacije.Aktivna;

    public NacinKreiranjaRezervacije NacinKreiranja { get; set; }

    public int? KreiraoZaposleniId { get; set; }
    public Zaposleni? KreiraoZaposleni { get; set; }

    public DateTime DatumKreiranja { get; set; } = DateTime.UtcNow;
}
