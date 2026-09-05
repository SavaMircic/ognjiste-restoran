namespace Services.DTO;

public class KorekcijaKolicineDto
{
    public decimal? NovaKolicina { get; set; }
    public decimal? Delta { get; set; }
    public string Razlog { get; set; } = string.Empty;
}
