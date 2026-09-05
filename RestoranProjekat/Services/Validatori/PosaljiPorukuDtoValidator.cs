using Domain.Enumi;
using FluentValidation;
using Services.DTO;

namespace Services.Validatori;

public class PosaljiPorukuDtoValidator : AbstractValidator<PosaljiPorukuDto>
{
    public PosaljiPorukuDtoValidator()
    {
        RuleFor(x => x.Kategorija).IsInEnum();
        RuleFor(x => x.Tekst).NotEmpty().WithMessage("Tekst poruke je obavezan.").MaximumLength(2000);

        RuleFor(x => x.Ime)
            .MinimumLength(2).WithMessage("Ime mora imati bar dva slova.")
            .MaximumLength(100)
            .Matches(Obrasci.Ime).WithMessage(Obrasci.ImePoruka)
            .When(x => !string.IsNullOrWhiteSpace(x.Ime));
        RuleFor(x => x.Email)
            .EmailAddress().WithMessage("Email nije u ispravnom formatu.")
            .When(x => !string.IsNullOrWhiteSpace(x.Email));
        RuleFor(x => x.Telefon)
            .Matches(Obrasci.Telefon).WithMessage(Obrasci.TelefonPoruka)
            .When(x => !string.IsNullOrWhiteSpace(x.Telefon));

        RuleFor(x => x.ZeljeniDatumVreme)
            .NotNull().WithMessage("Za zahtev za rezervaciju je obavezan željeni termin.")
            .GreaterThan(DateTime.UtcNow).WithMessage("Željeni termin mora biti u budućnosti.")
            .When(x => x.Kategorija == KategorijaPoruke.Rezervacija);

        RuleFor(x => x.ZeljeniBrojGostiju)
            .NotNull().WithMessage("Za zahtev za rezervaciju je obavezan broj gostiju.")
            .GreaterThan(0).WithMessage("Broj gostiju mora biti veći od nule.")
            .When(x => x.Kategorija == KategorijaPoruke.Rezervacija);
    }
}
