using FluentValidation;
using Services.DTO;

namespace Services.Validatori;

public class StavkaZaDodavanjeDtoValidator : AbstractValidator<StavkaZaDodavanjeDto>
{
    public StavkaZaDodavanjeDtoValidator()
    {
        RuleFor(x => x.StavkaMenijaId).GreaterThan(0).WithMessage("Stavka menija je obavezna.");
        RuleFor(x => x.Kolicina).GreaterThan(0).WithMessage("Količina mora biti veća od nule.");
        RuleFor(x => x.Napomena).MaximumLength(500);
    }
}
