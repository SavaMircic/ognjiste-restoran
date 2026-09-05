namespace Services.DTO;

public class IstorijaRadaUpitDto : IzvestajPeriodDto
{
}

public class IstorijaStavkaDto
{
    public string Naziv { get; set; } = string.Empty;
    public int Kolicina { get; set; }
    public decimal Cena { get; set; }
    public decimal Ukupno { get; set; }
}

public class IstorijaRacunDto
{
    public int PorudzbinaId { get; set; }
    public int BrojStola { get; set; }
    public DateTime VremeOtvaranja { get; set; }
    public DateTime? VremeZatvaranja { get; set; }
    public string? NacinPlacanja { get; set; }
    public decimal Napojnica { get; set; }
    public decimal Ukupno { get; set; }
    public List<IstorijaStavkaDto> Stavke { get; set; } = [];
}

public class IstorijaPripremaDto
{
    public int StavkaId { get; set; }
    public string Naziv { get; set; } = string.Empty;
    public int Kolicina { get; set; }
    public int BrojStola { get; set; }
    public DateTime VremeZavrsetka { get; set; }
    public decimal Vrednost { get; set; }
}

public class IstorijaDanDto
{
    public DateOnly Datum { get; set; }

    public List<IstorijaRacunDto> Racuni { get; set; } = [];
    public List<IstorijaPripremaDto> Pripreme { get; set; } = [];

    public int BrojRacuna { get; set; }
    public decimal PrometRacuna { get; set; }
    public decimal Napojnica { get; set; }

    public int BrojPripremljenihStavki { get; set; }
    public int KolicinaPripremljena { get; set; }
    public decimal VrednostPripremljenog { get; set; }
}
