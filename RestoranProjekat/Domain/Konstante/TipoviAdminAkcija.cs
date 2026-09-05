namespace Domain.Konstante;

public static class TipoviAdminAkcija
{
    public const string Blokiranje = "Blokiranje";
    public const string Odblokiranje = "Odblokiranje";
    public const string ZabranaKomentarisanja = "ZabranaKomentarisanja";
    public const string UkidanjeZabraneKomentarisanja = "UkidanjeZabraneKomentarisanja";
    public const string BrisanjeRecenzije = "BrisanjeRecenzije";

    public static readonly string[] Sve =
    {
        Blokiranje, Odblokiranje, ZabranaKomentarisanja, UkidanjeZabraneKomentarisanja,
        BrisanjeRecenzije
    };
}
