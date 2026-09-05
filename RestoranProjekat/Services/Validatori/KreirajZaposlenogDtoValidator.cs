using Domain.Konstante;
using FluentValidation;
using Services.DTO;

namespace Services.Validatori;

public class KreirajZaposlenogDtoValidator : AbstractValidator<KreirajZaposlenogDto>
{
    public KreirajZaposlenogDtoValidator()
    {
        RuleFor(x => x.Ime)
            .NotEmpty().WithMessage("Ime je obavezno.")
            .MinimumLength(2).WithMessage("Ime mora imati bar dva slova.")
            .MaximumLength(100)
            .Matches(Obrasci.Ime).WithMessage(Obrasci.ImePoruka);
        RuleFor(x => x.Prezime)
            .NotEmpty().WithMessage("Prezime je obavezno.")
            .MinimumLength(2).WithMessage("Prezime mora imati bar dva slova.")
            .MaximumLength(100)
            .Matches(Obrasci.Ime).WithMessage(Obrasci.ImePoruka);
        RuleFor(x => x.Email).NotEmpty().WithMessage("Email je obavezan.").EmailAddress().WithMessage("Email nije u ispravnom formatu.");
        RuleFor(x => x.PrivremenaLozinka)
            .NotEmpty().WithMessage("Privremena lozinka je obavezna.")
            .MinimumLength(8).WithMessage("Lozinka mora imati bar 8 karaktera.")
            .Matches("[A-Z]").WithMessage("Lozinka mora sadržati bar jedno veliko slovo.")
            .Matches("[0-9]").WithMessage("Lozinka mora sadržati bar jedan broj.");
        RuleFor(x => x.Uloga)
            .NotEmpty().WithMessage("Uloga je obavezna.")
            .Must(u => Uloge.Osoblje.Contains(u)).WithMessage($"Uloga mora biti jedna od: {string.Join(", ", Uloge.Osoblje)}.");
    }
}
