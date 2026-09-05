using FluentValidation;
using Services.DTO;

namespace Services.Validatori;

public class TopJelaUpitDtoValidator : IzvestajPeriodValidator<TopJelaUpitDto>
{
    public TopJelaUpitDtoValidator()
    {
        RuleFor(x => x.Redosled)
            .IsInEnum().WithMessage("Redosled mora biti: najprodavanije ili najmanjeprodavano.");
    }
}
