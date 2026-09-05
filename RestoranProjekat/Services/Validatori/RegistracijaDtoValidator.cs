using FluentValidation;
using Services.DTO;

namespace Services.Validatori;

public class RegistracijaDtoValidator : AbstractValidator<RegistracijaDto>
{
    public RegistracijaDtoValidator()
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
        RuleFor(x => x.Lozinka)
            .NotEmpty().WithMessage("Lozinka je obavezna.")
            .MinimumLength(8).WithMessage("Lozinka mora imati bar 8 karaktera.")
            .Matches("[A-Z]").WithMessage("Lozinka mora sadržati bar jedno veliko slovo.")
            .Matches("[0-9]").WithMessage("Lozinka mora sadržati bar jedan broj.");
        RuleFor(x => x.BrojTelefona)
            .Matches(Obrasci.Telefon).WithMessage(Obrasci.TelefonPoruka)
            .When(x => !string.IsNullOrWhiteSpace(x.BrojTelefona));
    }
}
