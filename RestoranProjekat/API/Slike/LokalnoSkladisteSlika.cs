using Services;
using Services.DTO;

namespace API.Slike;

public class LokalnoSkladisteSlika : ISkladisteSlika
{
    private const long MaksVelicinaBajtova = 2 * 1024 * 1024;
    private const string KorenSlika = "slike";

    private static readonly string[] DozvoljeneEkstenzije = [".jpg", ".jpeg", ".png", ".webp"];

    private static readonly string[] DozvoljeniTipovi =
        ["image/jpeg", "image/png", "image/webp", "application/octet-stream"];

    private readonly IWebHostEnvironment _okruzenje;
    private readonly ILogger<LokalnoSkladisteSlika> _logger;

    public LokalnoSkladisteSlika(IWebHostEnvironment okruzenje, ILogger<LokalnoSkladisteSlika> logger)
    {
        _okruzenje = okruzenje;
        _logger = logger;
    }

    public async Task<(bool Uspesno, string? Greska, string? Url)> SacuvajAsync(
        DatotekaZaUploadDto datoteka, string podfolder)
    {
        if (datoteka.Velicina <= 0)
            return (false, "Datoteka je prazna.", null);

        if (datoteka.Velicina > MaksVelicinaBajtova)
            return (false, $"Slika ne može biti veća od {MaksVelicinaBajtova / 1024 / 1024} MB.", null);

        var ekstenzija = Path.GetExtension(datoteka.ImeDatoteke).ToLowerInvariant();
        if (!DozvoljeneEkstenzije.Contains(ekstenzija))
            return (false, $"Dozvoljeni formati: {string.Join(", ", DozvoljeneEkstenzije)}.", null);

        if (!DozvoljeniTipovi.Contains(datoteka.TipSadrzaja.ToLowerInvariant()))
            return (false, "Sadržaj datoteke nije podržana slika.", null);

        var folder = Path.Combine(_okruzenje.WebRootPath, KorenSlika, podfolder);
        Directory.CreateDirectory(folder);

        var imeNaDisku = $"{Guid.NewGuid():N}{ekstenzija}";
        var punaPutanja = Path.Combine(folder, imeNaDisku);

        await using (var izlaz = File.Create(punaPutanja))
        {
            datoteka.Sadrzaj.Position = 0;
            await datoteka.Sadrzaj.CopyToAsync(izlaz);
        }

        return (true, null, $"/{KorenSlika}/{podfolder}/{imeNaDisku}");
    }

    public Task ObrisiAsync(string? slikaUrl)
    {
        if (string.IsNullOrWhiteSpace(slikaUrl))
            return Task.CompletedTask;

        if (slikaUrl.Equals(ISkladisteSlika.RezervnaSlikaJela, StringComparison.OrdinalIgnoreCase))
            return Task.CompletedTask;

        var delovi = slikaUrl.TrimStart('/').Split('/');
        if (delovi.Length != 3 || !delovi[0].Equals(KorenSlika, StringComparison.OrdinalIgnoreCase))
            return Task.CompletedTask;

        var imeDatoteke = Path.GetFileName(delovi[2]);
        var putanja = Path.Combine(_okruzenje.WebRootPath, KorenSlika, delovi[1], imeDatoteke);

        try
        {
            if (File.Exists(putanja)) File.Delete(putanja);
        }
        catch (IOException greska)
        {
            _logger.LogWarning(greska, "Nije uspelo brisanje slike {Putanja}", putanja);
        }

        return Task.CompletedTask;
    }
}
