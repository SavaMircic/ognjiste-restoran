using Domain.Konstante;
using FluentValidation;
using Services.DTO;

namespace Services.Validatori;

public class KreirajRezervacijuDtoValidator : AbstractValidator<KreirajRezervacijuDto>
{
    public KreirajRezervacijuDtoValidator()
    {
        RuleFor(x => x.StoIds)
            .NotEmpty().WithMessage("Izaberite bar jedan sto.")
            .Must(x => x.Count <= PoslovnaPravila.MaksSpojenihStolova)
            .WithMessage($"Najviše {PoslovnaPravila.MaksSpojenihStolova} stola po rezervaciji.")
            .Must(x => x.Distinct().Count() == x.Count).WithMessage("Isti sto je izabran više puta.")
            .Must(x => x.All(id => id > 0)).WithMessage("Neispravan sto.");

        RuleFor(x => x.DatumVreme).GreaterThan(DateTime.UtcNow).WithMessage("Termin mora biti u budućnosti.");

        RuleFor(x => x.BrojGostiju)
            .GreaterThan(0).WithMessage("Broj gostiju mora biti veći od nule.")
            .LessThanOrEqualTo(PoslovnaPravila.MaksGostijuOnline)
            .WithMessage($"Preko sajta se rezerviše za najviše {PoslovnaPravila.MaksGostijuOnline} gostiju. " +
                         "Za veće društvo nas pozovite telefonom.");
    }
}
