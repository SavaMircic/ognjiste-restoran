using FluentValidation;
using Services.DTO;

namespace Services.Validatori;

public class ZatvoriPorudzbinuDtoValidator : AbstractValidator<ZatvoriPorudzbinuDto>
{
    public ZatvoriPorudzbinuDtoValidator()
    {
        RuleFor(x => x.NacinPlacanja).IsInEnum().WithMessage("Način plaćanja mora biti Gotovina ili Kartica.");
        RuleFor(x => x.IznosNapojnice)
            .GreaterThanOrEqualTo(0).WithMessage("Napojnica ne može biti negativna.")
            .When(x => x.IznosNapojnice.HasValue);
    }
}
