using FluentValidation;
using Services.DTO;

namespace Services.Validatori;

public class StavkaMenijaUlazDtoValidator : AbstractValidator<StavkaMenijaUlazDto>
{
    public StavkaMenijaUlazDtoValidator()
    {
        RuleFor(x => x.Naziv).NotEmpty().WithMessage("Naziv je obavezan.").MaximumLength(150);
        RuleFor(x => x.Opis).NotEmpty().WithMessage("Opis je obavezan.").MaximumLength(2000);
        RuleFor(x => x.DetaljanOpis).MaximumLength(4000);
        RuleFor(x => x.Cena).GreaterThan(0).WithMessage("Cena mora biti veća od nule.");
        RuleFor(x => x.KategorijaId).GreaterThan(0).WithMessage("Kategorija je obavezna.");
        RuleFor(x => x.Popust)
            .InclusiveBetween(0, 100).WithMessage("Popust mora biti procenat između 0 i 100.")
            .When(x => x.Popust.HasValue);
    }
}
