using Domain.Enumi;
using Services.DTO;

namespace Services;

public interface INotifikacijaServis
{
    Task PosaljiNovaStavkaAsync(Odrediste odrediste, NovaStavkaNotifikacijaDto podaci);
    Task PosaljiStavkaSpremnaAsync(string konobarKorisnikId, StavkaSpremnaNotifikacijaDto podaci);
}
