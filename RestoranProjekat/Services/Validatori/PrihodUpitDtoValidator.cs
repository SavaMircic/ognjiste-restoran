using FluentValidation;
using Services.DTO;

namespace Services.Validatori;

public class PrihodUpitDtoValidator : IzvestajPeriodValidator<PrihodUpitDto>
{
    public PrihodUpitDtoValidator()
    {
        RuleFor(x => x.GrupisanjePo)
            .IsInEnum().WithMessage("Grupisanje mora biti: dan, nedelja, mesec, kategorija ili artikal.");
    }
}
