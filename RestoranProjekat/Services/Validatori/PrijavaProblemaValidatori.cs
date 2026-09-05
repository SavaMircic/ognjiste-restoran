using Domain.Enumi;
using FluentValidation;
using Services.DTO;

namespace Services.Validatori;

public class PrijaviProblemDtoValidator : AbstractValidator<PrijaviProblemDto>
{
    public PrijaviProblemDtoValidator()
    {
        RuleFor(x => x.Naslov)
            .NotEmpty().WithMessage("Naslov je obavezan.")
            .MaximumLength(150);

        RuleFor(x => x.Opis)
            .NotEmpty().WithMessage("Opis problema je obavezan.")
            .MaximumLength(2000);

        RuleFor(x => x.Kategorija).IsInEnum().WithMessage("Nepoznata kategorija problema.");
        RuleFor(x => x.Prioritet).IsInEnum().WithMessage("Nepoznat prioritet.");
    }
}

public class ResiPrijavuProblemaDtoValidator : AbstractValidator<ResiPrijavuProblemaDto>
{
    public ResiPrijavuProblemaDtoValidator()
    {
        RuleFor(x => x.Status).IsInEnum().WithMessage("Nepoznat status prijave.");

        RuleFor(x => x.Status)
            .NotEqual(StatusPrijaveProblema.Nova)
            .WithMessage("Prijava se ne može vratiti u status Nova.");

        RuleFor(x => x.Odgovor).MaximumLength(1000);

        RuleFor(x => x.Odgovor)
            .NotEmpty().WithMessage("Odgovor je obavezan kada se prijava rešava ili odbija.")
            .When(x => x.Status is StatusPrijaveProblema.Resena or StatusPrijaveProblema.Odbijena);
    }
}
