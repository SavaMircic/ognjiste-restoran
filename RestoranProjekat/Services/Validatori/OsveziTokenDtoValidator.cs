using FluentValidation;
using Services.DTO;

namespace Services.Validatori;

public class OsveziTokenDtoValidator : AbstractValidator<OsveziTokenDto>
{
    public OsveziTokenDtoValidator()
    {
        RuleFor(x => x.AccessToken).NotEmpty().WithMessage("Access token je obavezan.");
        RuleFor(x => x.RefreshToken).NotEmpty().WithMessage("Refresh token je obavezan.");
    }
}
