namespace Services.Validatori;

public static class Obrasci
{
    public const string Telefon = @"^(?:\+381|00381|0)[\s\-/]?\d{2}[\s\-/]?\d{3}[\s\-/]?\d{3,4}$";

    public const string TelefonPoruka =
        "Broj telefona nije ispravan. Primeri: 064 123 4567, 021/555-123, +381641234567.";

    public const string Ime = @"^\p{L}+(?:[ '’\-]\p{L}+)*$";

    public const string ImePoruka = "Dozvoljena su samo slova, razmak, crtica i apostrof.";
}
