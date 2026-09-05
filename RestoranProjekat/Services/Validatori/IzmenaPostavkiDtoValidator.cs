using FluentValidation;
using Services.DTO;

namespace Services.Validatori;

public class IzmenaPostavkiDtoValidator : AbstractValidator<IzmenaPostavkiDto>
{
    public IzmenaPostavkiDtoValidator()
    {
        RuleFor(x => x.Adresa)
            .NotEmpty().WithMessage("Adresa je obavezna.")
            .MaximumLength(200);

        RuleFor(x => x.Telefon)
            .NotEmpty().WithMessage("Telefon je obavezan.")
            .Matches(Obrasci.Telefon).WithMessage(Obrasci.TelefonPoruka);

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email je obavezan.")
            .EmailAddress().WithMessage("Email nije u ispravnom formatu.")
            .MaximumLength(100);

        RuleFor(x => x.RadnoVreme)
            .NotEmpty().WithMessage("Radno vreme je obavezno.")
            .MaximumLength(200);

        RuleFor(x => x.OpisRestorana)
            .MaximumLength(2000).WithMessage("Opis restorana ne može biti duži od 2000 karaktera.");

        RuleFor(x => x.GeoSirina)
            .InclusiveBetween(-90, 90).WithMessage("Geografska širina mora biti između -90 i 90.")
            .When(x => x.GeoSirina.HasValue);

        RuleFor(x => x.GeoDuzina)
            .InclusiveBetween(-180, 180).WithMessage("Geografska dužina mora biti između -180 i 180.")
            .When(x => x.GeoDuzina.HasValue);

        RuleFor(x => x.FacebookUrl)
            .Must(v => DrustveneMreze.JeLink(v, DrustveneMreze.Facebook))
            .WithMessage(DrustveneMreze.Poruka("Facebook", "facebook.com", "https://www.facebook.com/ognjiste.novisad"))
            .MaximumLength(300)
            .When(x => !string.IsNullOrWhiteSpace(x.FacebookUrl));

        RuleFor(x => x.InstagramUrl)
            .Must(v => DrustveneMreze.JeLink(v, DrustveneMreze.Instagram))
            .WithMessage(DrustveneMreze.Poruka("Instagram", "instagram.com", "https://www.instagram.com/ognjiste.novisad"))
            .MaximumLength(300)
            .When(x => !string.IsNullOrWhiteSpace(x.InstagramUrl));
    }
}
