using FluentValidation;
using Services.DTO;

namespace Services.Validatori;

public class IzmenaProfilaDtoValidator : AbstractValidator<IzmenaProfilaDto>
{
    public IzmenaProfilaDtoValidator()
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

        RuleFor(x => x.BrojTelefona)
            .Matches(Obrasci.Telefon).WithMessage(Obrasci.TelefonPoruka)
            .When(x => !string.IsNullOrWhiteSpace(x.BrojTelefona));
    }
}
