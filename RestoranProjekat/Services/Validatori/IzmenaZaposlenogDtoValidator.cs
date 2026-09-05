using Domain.Konstante;
using FluentValidation;
using Services.DTO;

namespace Services.Validatori;

public class IzmenaZaposlenogDtoValidator : AbstractValidator<IzmenaZaposlenogDto>
{
    public IzmenaZaposlenogDtoValidator()
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
        RuleFor(x => x.Uloga)
            .NotEmpty().WithMessage("Uloga je obavezna.")
            .Must(u => Uloge.Osoblje.Contains(u)).WithMessage($"Uloga mora biti jedna od: {string.Join(", ", Uloge.Osoblje)}.");
    }
}
