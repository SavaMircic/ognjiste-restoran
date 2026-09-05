using FluentValidation;
using Services.DTO;

namespace Services.Validatori;

public abstract class IzvestajPeriodValidator<T> : AbstractValidator<T> where T : IzvestajPeriodDto
{
    private const int MaksimalnoDana = 366;

    protected IzvestajPeriodValidator()
    {
        RuleFor(x => x.DatumOd)
            .NotEqual(default(DateOnly)).WithMessage("Datum od je obavezan.");

        RuleFor(x => x.DatumDo)
            .NotEqual(default(DateOnly)).WithMessage("Datum do je obavezan.");

        RuleFor(x => x.DatumDo)
            .GreaterThanOrEqualTo(x => x.DatumOd)
            .WithMessage("Datum do ne može biti pre datuma od.")
            .When(x => x.DatumOd != default && x.DatumDo != default);

        RuleFor(x => x)
            .Must(x => x.DatumDo.DayNumber - x.DatumOd.DayNumber <= MaksimalnoDana)
            .WithMessage($"Period izveštaja ne može biti duži od {MaksimalnoDana} dana.")
            .When(x => x.DatumOd != default && x.DatumDo != default && x.DatumDo >= x.DatumOd);
    }
}
