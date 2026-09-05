namespace Services.DTO;

public class KorisnikAdminDto
{
    public string Id { get; set; } = string.Empty;
    public string Ime { get; set; } = string.Empty;
    public string Prezime { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? BrojTelefona { get; set; }
    public DateTime DatumRegistracije { get; set; }
    public bool Aktivan { get; set; }
    public DateTime? BlokiranDo { get; set; }
    public DateTime? ZabranaKomentarisanjaDo { get; set; }
    public List<string> Uloge { get; set; } = new();

    public bool Blokiran { get; set; }
}
