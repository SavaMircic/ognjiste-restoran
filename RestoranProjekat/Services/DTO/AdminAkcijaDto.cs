namespace Services.DTO;

public class AdminAkcijaDto
{
    public int Id { get; set; }
    public int AdministratorId { get; set; }
    public string AdministratorIme { get; set; } = string.Empty;
    public string TipAkcije { get; set; } = string.Empty;
    public string? CiljniKorisnikId { get; set; }
    public string? CiljniKorisnikEmail { get; set; }
    public string Opis { get; set; } = string.Empty;
    public DateTime Datum { get; set; }
}

public class AdminAkcijePretragaDto : PaginacijaParametriDto
{
    public string? TipAkcije { get; set; }
    public string? CiljniKorisnikId { get; set; }
    public DateTime? DatumOd { get; set; }
    public DateTime? DatumDo { get; set; }

}
