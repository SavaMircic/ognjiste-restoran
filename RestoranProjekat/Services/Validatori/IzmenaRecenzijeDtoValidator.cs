using FluentValidation;
using Services.DTO;

namespace Services.Validatori;

public class IzmenaRecenzijeDtoValidator : AbstractValidator<IzmenaRecenzijeDto>
{
    public IzmenaRecenzijeDtoValidator()
    {
        RuleFor(x => x.Ocena).InclusiveBetween(1, 5).WithMessage("Ocena mora biti između 1 i 5.");
        RuleFor(x => x.Naslov).MaximumLength(150);
        RuleFor(x => x.Tekst).NotEmpty().WithMessage("Tekst je obavezan.").MaximumLength(2000);
    }
}
