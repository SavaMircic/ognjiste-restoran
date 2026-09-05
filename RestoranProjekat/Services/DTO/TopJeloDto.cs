namespace Services.DTO;

public class TopJeloDto
{
    public int Rang { get; set; }
    public int StavkaMenijaId { get; set; }
    public string Naziv { get; set; } = string.Empty;
    public string Kategorija { get; set; } = string.Empty;

    public int UkupnaKolicina { get; set; }

    public int BrojPorudzbina { get; set; }

    public decimal Prihod { get; set; }
}
