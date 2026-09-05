namespace Repository.Rezultati;

public class RacunGostaRezultat
{
    public int PorudzbinaId { get; set; }
    public string? KodRezervacije { get; set; }
    public DateTime? VremeZatvaranja { get; set; }
    public int BrojStola { get; set; }
    public decimal Ukupno { get; set; }
    public decimal Napojnica { get; set; }
}

public class PouzdanostGostaRezultat
{
    public int Realizovane { get; set; }
    public int Istekle { get; set; }
    public int Otkazane { get; set; }
    public int Aktivne { get; set; }
}
