using FluentValidation;
using Microsoft.AspNetCore.Mvc;

namespace API.Ulaz;

public class UploadSlikeDto
{
    [FromForm(Name = "slika")]
    public IFormFile? Slika { get; set; }

    [FromForm(Name = "opis")]
    public string? Opis { get; set; }
}

public class UploadGalerijeDto
{
    [FromForm(Name = "slika")]
    public IFormFile? Slika { get; set; }

    [FromForm(Name = "naslov")]
    public string Naslov { get; set; } = string.Empty;

    [FromForm(Name = "opis")]
    public string? Opis { get; set; }

    [FromForm(Name = "grupa")]
    public string Grupa { get; set; } = string.Empty;
}

public class UploadGalerijeDtoValidator : AbstractValidator<UploadGalerijeDto>
{
    public UploadGalerijeDtoValidator()
    {
        RuleFor(x => x.Slika).NotNull().WithMessage("Slika je obavezna.");
        RuleFor(x => x.Naslov).NotEmpty().WithMessage("Naslov je obavezan.").MaximumLength(150);
        RuleFor(x => x.Opis).MaximumLength(500);
        RuleFor(x => x.Grupa).NotEmpty().WithMessage("Oblast je obavezna.").MaximumLength(100);

        When(x => x.Slika != null, () =>
            RuleFor(x => x.Slika!).SetValidator(new PravilaZaSliku()));
    }
}

public class PravilaZaSliku : AbstractValidator<IFormFile>
{
    private const long MaksVelicinaBajtova = 2 * 1024 * 1024;
    private static readonly string[] Ekstenzije = [".jpg", ".jpeg", ".png", ".webp"];
    private static readonly string[] Tipovi = ["image/jpeg", "image/png", "image/webp", "application/octet-stream"];

    public PravilaZaSliku()
    {
        RuleFor(f => f.Length)
            .GreaterThan(0).WithMessage("Slika je prazna.")
            .LessThanOrEqualTo(MaksVelicinaBajtova)
            .WithMessage($"Slika ne može biti veća od {MaksVelicinaBajtova / 1024 / 1024} MB.");

        RuleFor(f => f.FileName)
            .Must(ime => Ekstenzije.Contains(Path.GetExtension(ime).ToLowerInvariant()))
            .WithMessage($"Dozvoljeni formati: {string.Join(", ", Ekstenzije)}.");

        RuleFor(f => f.ContentType)
            .Must(tip => Tipovi.Contains(tip.ToLowerInvariant()))
            .WithMessage("Sadržaj datoteke nije podržana slika.");
    }
}

public class UploadSlikeDtoValidator : AbstractValidator<UploadSlikeDto>
{
    private const long MaksVelicinaBajtova = 2 * 1024 * 1024;
    private static readonly string[] DozvoljeneEkstenzije = [".jpg", ".jpeg", ".png", ".webp"];

    private static readonly string[] DozvoljeniTipovi =
        ["image/jpeg", "image/png", "image/webp", "application/octet-stream"];

    public UploadSlikeDtoValidator()
    {
        RuleFor(x => x.Slika)
            .NotNull().WithMessage("Slika je obavezna.");

        When(x => x.Slika != null, () =>
        {
            RuleFor(x => x.Slika!.Length)
                .GreaterThan(0).WithMessage("Slika je prazna.")
                .LessThanOrEqualTo(MaksVelicinaBajtova)
                .WithMessage($"Slika ne može biti veća od {MaksVelicinaBajtova / 1024 / 1024} MB.");

            RuleFor(x => x.Slika!.FileName)
                .Must(ime => DozvoljeneEkstenzije.Contains(Path.GetExtension(ime).ToLowerInvariant()))
                .WithMessage($"Dozvoljeni formati: {string.Join(", ", DozvoljeneEkstenzije)}.");

            RuleFor(x => x.Slika!.ContentType)
                .Must(tip => DozvoljeniTipovi.Contains(tip.ToLowerInvariant()))
                .WithMessage("Sadržaj datoteke nije podržana slika.");
        });

        RuleFor(x => x.Opis).MaximumLength(300);
    }
}
