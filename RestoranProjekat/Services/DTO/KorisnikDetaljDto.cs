using Domain.Enumi;

namespace Services.DTO;

public class KorisnikDetaljDto
{
    public KorisnikAdminDto Profil { get; set; } = new();

    public List<RecenzijaPregledDto> Recenzije { get; set; } = new();
    public List<RezervacijaPregledDto> Rezervacije { get; set; } = new();
    public List<PorukaPregledDto> Poruke { get; set; } = new();
    public List<RacunGostaDto> Racuni { get; set; } = new();

    public PouzdanostGostaDto Pouzdanost { get; set; } = new();

    public decimal UkupnoPotroseno { get; set; }
    public decimal UkupnaNapojnica { get; set; }
}

public class RecenzijaPregledDto
{
    public int Id { get; set; }
    public TipRecenzije TipRecenzije { get; set; }
    public string? NazivStavke { get; set; }
    public int Ocena { get; set; }
    public string? Naslov { get; set; }
    public DateTime DatumKreiranja { get; set; }

    public bool Aktivan { get; set; }
}

public class RezervacijaPregledDto
{
    public int Id { get; set; }
    public string KodRezervacije { get; set; } = string.Empty;
    public DateTime DatumVreme { get; set; }
    public int BrojGostiju { get; set; }
    public StatusRezervacije Status { get; set; }
}

public class PorukaPregledDto
{
    public int Id { get; set; }
    public KategorijaPoruke Kategorija { get; set; }
    public StatusPoruke Status { get; set; }
    public DateTime DatumSlanja { get; set; }
}

public class RacunGostaDto
{
    public int PorudzbinaId { get; set; }
    public string? KodRezervacije { get; set; }
    public DateTime? VremeZatvaranja { get; set; }
    public int BrojStola { get; set; }
    public decimal Ukupno { get; set; }
    public decimal Napojnica { get; set; }
}

public class PouzdanostGostaDto
{
    public int Realizovane { get; set; }
    public int Istekle { get; set; }
    public int Otkazane { get; set; }
    public int Aktivne { get; set; }

    public double? ProcenatPojavljivanja { get; set; }
}
