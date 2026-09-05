using Domain.Enumi;
using FluentValidation;
using Services.DTO;

namespace Services.Validatori;

public class KreirajAdminRezervacijuDtoValidator : AbstractValidator<KreirajAdminRezervacijuDto>
{
    public KreirajAdminRezervacijuDtoValidator()
    {
        RuleFor(x => x.StoId).GreaterThan(0).WithMessage("Sto je obavezan.");
        RuleFor(x => x.DatumVreme).GreaterThan(DateTime.UtcNow).WithMessage("Termin mora biti u budućnosti.");
        RuleFor(x => x.BrojGostiju).GreaterThan(0).WithMessage("Broj gostiju mora biti veći od nule.");
        RuleFor(x => x.NacinKreiranja)
            .Must(n => n is NacinKreiranjaRezervacije.TelefonAdmin or NacinKreiranjaRezervacije.KontaktForma)
            .WithMessage("Način kreiranja mora biti TelefonAdmin ili KontaktForma.");

        RuleFor(x => x)
            .Must(x => !string.IsNullOrWhiteSpace(x.KorisnikId) ||
                       (!string.IsNullOrWhiteSpace(x.GostIme) && !string.IsNullOrWhiteSpace(x.GostEmail) && !string.IsNullOrWhiteSpace(x.GostTelefon)))
            .WithMessage("Mora biti prosleđen KorisnikId ili sva tri gost polja (GostIme, GostEmail, GostTelefon).");

        RuleFor(x => x.GostIme)
            .MinimumLength(2).WithMessage("Ime gosta mora imati bar dva slova.")
            .MaximumLength(100)
            .Matches(Obrasci.Ime).WithMessage(Obrasci.ImePoruka)
            .When(x => !string.IsNullOrWhiteSpace(x.GostIme));

        RuleFor(x => x.GostTelefon)
            .Matches(Obrasci.Telefon).WithMessage(Obrasci.TelefonPoruka)
            .When(x => !string.IsNullOrWhiteSpace(x.GostTelefon));

        RuleFor(x => x.GostEmail)
            .EmailAddress().WithMessage("Email gosta nije u ispravnom formatu.")
            .When(x => !string.IsNullOrWhiteSpace(x.GostEmail));
    }
}
