using Services.DTO;

namespace Services;

public interface ISkladisteSlika
{
    public static class Podfolderi
    {
        public const string Jela = "jela";
        public const string Profilne = "profilne";
        public const string Zaposleni = "zaposleni";
        public const string Galerija = "galerija";
    }

    public const string RezervnaSlikaJela = "/slike/jela/placeholder.svg";

    Task<(bool Uspesno, string? Greska, string? Url)> SacuvajAsync(DatotekaZaUploadDto datoteka, string podfolder);

    Task ObrisiAsync(string? slikaUrl);
}
