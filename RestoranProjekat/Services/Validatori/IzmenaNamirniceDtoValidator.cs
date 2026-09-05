using FluentValidation;
using Services.DTO;

namespace Services.Validatori;

public class IzmenaNamirniceDtoValidator : AbstractValidator<IzmenaNamirniceDto>
{
    public IzmenaNamirniceDtoValidator()
    {
        RuleFor(x => x.Naziv).NotEmpty().WithMessage("Naziv je obavezan.").MaximumLength(100);
        RuleFor(x => x.MinimalniPrag).GreaterThanOrEqualTo(0).WithMessage("Minimalni prag ne može biti negativan.");
    }
}
