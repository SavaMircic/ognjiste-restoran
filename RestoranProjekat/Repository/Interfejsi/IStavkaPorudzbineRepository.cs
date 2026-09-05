using Domain.Entiteti;
using Domain.Enumi;

namespace Repository.Interfejsi;

public interface IStavkaPorudzbineRepository : IRepository<StavkaPorudzbine>
{
    Task<List<StavkaPorudzbine>> RedCekanjaAsync(Odrediste odrediste);

    Task<StavkaPorudzbine?> DobaviSaDetaljimaAsync(int id);

    Task<bool> IkadaNarucenaAsync(int stavkaMenijaId);
}
