using Domain.Entiteti;

namespace Repository.Interfejsi;

public interface IReceptureRepository : IRepository<Receptura>
{
    Task<List<Receptura>> ListirajZaStavkuAsync(int stavkaMenijaId);
    Task<Receptura?> DobaviAsync(int stavkaMenijaId, int namirnicaId);
}
