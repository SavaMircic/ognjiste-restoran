using Domain.Enumi;

namespace Domain.Entiteti;

public class Poruka
{
    public int Id { get; set; }

    public string? KorisnikId { get; set; }
    public Korisnik? Korisnik { get; set; }

    public string? GostIme { get; set; }
    public string? GostEmail { get; set; }
    public string? GostTelefon { get; set; }

    public KategorijaPoruke Kategorija { get; set; }
    public string Tekst { get; set; } = string.Empty;
    public DateTime DatumSlanja { get; set; } = DateTime.UtcNow;
    public StatusPoruke Status { get; set; } = StatusPoruke.Novo;

    public string? Odgovor { get; set; }
    public DateTime? DatumOdgovora { get; set; }

    public int? ObradioZaposleniId { get; set; }
    public Zaposleni? ObradioZaposleni { get; set; }

    public DateTime? ZeljeniDatumVreme { get; set; }
    public int? ZeljeniBrojGostiju { get; set; }
}
