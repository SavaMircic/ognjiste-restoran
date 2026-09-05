using FluentValidation;
using Services.DTO;

namespace Services.Validatori;

public class RedosledSlikaDtoValidator : AbstractValidator<RedosledSlikaDto>
{
    public RedosledSlikaDtoValidator()
    {
        RuleFor(x => x.IdRedom)
            .NotEmpty().WithMessage("Lista id-jeva ne može biti prazna.");

        RuleFor(x => x.IdRedom)
            .Must(lista => lista.All(id => id > 0))
            .WithMessage("Svi id-jevi moraju biti pozitivni.")
            .When(x => x.IdRedom.Count > 0);

        RuleFor(x => x.IdRedom)
            .Must(lista => lista.Distinct().Count() == lista.Count)
            .WithMessage("Lista sadrži isti id više puta.")
            .When(x => x.IdRedom.Count > 0);
    }
}
