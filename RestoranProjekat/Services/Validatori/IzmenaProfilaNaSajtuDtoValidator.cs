using FluentValidation;
using Services.DTO;

namespace Services.Validatori;

public class IzmenaProfilaNaSajtuDtoValidator : AbstractValidator<IzmenaProfilaNaSajtuDto>
{
    public IzmenaProfilaNaSajtuDtoValidator()
    {
        RuleFor(x => x.Biografija)
            .MaximumLength(1000).WithMessage("Biografija ne može biti duža od 1000 karaktera.");

        RuleFor(x => x.LinkedInUrl)
            .Must(v => DrustveneMreze.JeLink(v, DrustveneMreze.LinkedIn))
            .WithMessage(DrustveneMreze.Poruka("LinkedIn", "linkedin.com", "https://www.linkedin.com/in/ime-prezime"))
            .MaximumLength(300)
            .When(x => !string.IsNullOrWhiteSpace(x.LinkedInUrl));

        RuleFor(x => x.InstagramUrl)
            .Must(v => DrustveneMreze.JeLink(v, DrustveneMreze.Instagram))
            .WithMessage(DrustveneMreze.Poruka("Instagram", "instagram.com", "https://www.instagram.com/korisnicko.ime"))
            .MaximumLength(300)
            .When(x => !string.IsNullOrWhiteSpace(x.InstagramUrl));

        RuleFor(x => x.Redosled)
            .InclusiveBetween(0, 999).WithMessage("Redosled mora biti između 0 i 999.");
    }
}
