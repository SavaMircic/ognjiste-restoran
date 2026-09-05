using FluentValidation;
using Services.DTO;

namespace Services.Validatori;

public class KreirajNamirnicuDtoValidator : AbstractValidator<KreirajNamirnicuDto>
{
    public KreirajNamirnicuDtoValidator()
    {
        RuleFor(x => x.Naziv).NotEmpty().WithMessage("Naziv je obavezan.").MaximumLength(100);
        RuleFor(x => x.JedinicaMere).IsInEnum().WithMessage("Jedinica mere mora biti Kg, L ili Kom.");
        RuleFor(x => x.MinimalniPrag).GreaterThanOrEqualTo(0).WithMessage("Minimalni prag ne može biti negativan.");
        RuleFor(x => x.PocetnaKolicina).GreaterThanOrEqualTo(0).WithMessage("Početna količina ne može biti negativna.");
    }
}
