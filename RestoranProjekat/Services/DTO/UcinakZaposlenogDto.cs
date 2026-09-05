namespace Services.DTO;

public class UcinakZaposlenogDto
{
    public int ZaposleniId { get; set; }
    public string ImeZaposlenog { get; set; } = string.Empty;

    public int BrojStolova { get; set; }
    public decimal VrednostStolova { get; set; }
    public decimal UkupnaNapojnica { get; set; }

    public int BrojPripremljenihStavki { get; set; }
    public int UkupnaKolicinaPripremljena { get; set; }
    public decimal VrednostPripremljenih { get; set; }

    public decimal ProsecanRacun { get; set; }
}
