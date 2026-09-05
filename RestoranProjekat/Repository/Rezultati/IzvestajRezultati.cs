namespace Repository.Rezultati;

public class PrihodPoDanuRezultat
{
    public DateTime Dan { get; set; }
    public decimal Prihod { get; set; }
    public int Kolicina { get; set; }
}

public class PrihodPoGrupiRezultat
{
    public string Grupa { get; set; } = string.Empty;
    public decimal Prihod { get; set; }
    public int Kolicina { get; set; }
}

public class ZbirPrihodaRezultat
{
    public decimal UkupanPrihod { get; set; }
    public decimal UkupnaNapojnica { get; set; }
    public int BrojPorudzbina { get; set; }
}

public class UcinakKonobaraRezultat
{
    public int ZaposleniId { get; set; }
    public string Ime { get; set; } = string.Empty;
    public int BrojStolova { get; set; }
    public decimal VrednostStolova { get; set; }
    public decimal Napojnica { get; set; }
}

public class UcinakPripremeRezultat
{
    public int ZaposleniId { get; set; }
    public string Ime { get; set; } = string.Empty;
    public int BrojStavki { get; set; }
    public int UkupnaKolicina { get; set; }
    public decimal VrednostStavki { get; set; }
}

public class TopJeloRezultat
{
    public int StavkaMenijaId { get; set; }
    public string Naziv { get; set; } = string.Empty;
    public string Kategorija { get; set; } = string.Empty;
    public int UkupnaKolicina { get; set; }
    public int BrojPorudzbina { get; set; }
    public decimal Prihod { get; set; }
}
