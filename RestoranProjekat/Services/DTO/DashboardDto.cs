namespace Services.DTO;

public class MenadzerskiDashboardDto
{
    public decimal DanasnjiPrihod { get; set; }
    public int BrojZatvorenihRacunaDanas { get; set; }
    public decimal ProsecanRacunDanas { get; set; }
    public decimal DanasnjaNapojnica { get; set; }

    public int BrojOtvorenihStolova { get; set; }
    public int BrojNamirnicaIspodPraga { get; set; }
    public List<string> NamirniceIspodPraga { get; set; } = new();

    public int BrojNovihPrijavaProblema { get; set; }
}

public class AdministratorskiDashboardDto
{
    public int BrojNovihPoruka { get; set; }

    public int BrojPrioritetnihPoruka { get; set; }

    public int BrojAktivnihRezervacijaDanas { get; set; }
    public int BrojOtvorenihStolova { get; set; }
    public int BrojBlokiranihKorisnika { get; set; }
    public int BrojRecenzijaBezOdgovora { get; set; }
}
