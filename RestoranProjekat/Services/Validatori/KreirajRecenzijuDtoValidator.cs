using Domain.Enumi;
using FluentValidation;
using Services.DTO;

namespace Services.Validatori;

public class KreirajRecenzijuDtoValidator : AbstractValidator<KreirajRecenzijuDto>
{
    public KreirajRecenzijuDtoValidator()
    {
        RuleFor(x => x.TipRecenzije).IsInEnum();
        RuleFor(x => x.Ocena).InclusiveBetween(1, 5).WithMessage("Ocena mora biti između 1 i 5.");
        RuleFor(x => x.Naslov).MaximumLength(150);
        RuleFor(x => x.Tekst).NotEmpty().WithMessage("Tekst je obavezan.").MaximumLength(2000);

        RuleFor(x => x.StavkaMenijaId)
            .NotNull().WithMessage("StavkaMenijaId je obavezan za recenziju jela.")
            .When(x => x.TipRecenzije == TipRecenzije.Jelo);

        RuleFor(x => x.StavkaMenijaId)
            .Null().WithMessage("StavkaMenijaId se ne postavlja za ovaj tip recenzije.")
            .When(x => x.TipRecenzije != TipRecenzije.Jelo);
    }
}
