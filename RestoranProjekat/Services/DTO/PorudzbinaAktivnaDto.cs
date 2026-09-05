namespace Services.DTO;

public class PorudzbinaAktivnaDto
{
    public int Id { get; set; }
    public int BrojStola { get; set; }

    public int? RezervacijaId { get; set; }

    public DateTime VremeOtvaranja { get; set; }
    public int BrojStavkiPoslato { get; set; }
    public int BrojStavkiUPripremi { get; set; }
    public int BrojStavkiSpremno { get; set; }
}
