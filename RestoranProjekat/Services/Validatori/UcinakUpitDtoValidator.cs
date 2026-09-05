using FluentValidation;
using Services.DTO;

namespace Services.Validatori;

public class UcinakUpitDtoValidator : IzvestajPeriodValidator<UcinakUpitDto>
{
    public UcinakUpitDtoValidator()
    {
        RuleFor(x => x.ZaposleniId)
            .GreaterThan(0).WithMessage("ZaposleniId mora biti pozitivan broj.")
            .When(x => x.ZaposleniId.HasValue);
    }
}
