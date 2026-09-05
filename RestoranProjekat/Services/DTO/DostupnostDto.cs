namespace Services.DTO;

public class DostupnostDto
{
    public List<StoDto> Stolovi { get; set; } = new();

    public List<PredlogSpajanjaDto> Predlozi { get; set; } = new();

    public int MaksGostijuOnline { get; set; }

    public string? Telefon { get; set; }
}

public class PredlogSpajanjaDto
{
    public List<StoDto> Stolovi { get; set; } = new();

    public int UkupnoMesta { get; set; }

    public int VisakMesta { get; set; }
}
