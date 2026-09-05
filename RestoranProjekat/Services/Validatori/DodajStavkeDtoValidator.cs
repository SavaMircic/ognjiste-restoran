using FluentValidation;
using Services.DTO;

namespace Services.Validatori;

public class DodajStavkeDtoValidator : AbstractValidator<DodajStavkeDto>
{
    public DodajStavkeDtoValidator()
    {
        RuleFor(x => x.Stavke).NotEmpty().WithMessage("Morate dodati bar jednu stavku.");
        RuleForEach(x => x.Stavke).SetValidator(new StavkaZaDodavanjeDtoValidator());
    }
}
