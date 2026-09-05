using Domain.Entiteti;

namespace Repository.Interfejsi;

public interface ILajkJelaRepository : IRepository<LajkJela>
{
    Task<LajkJela?> DobaviAsync(string korisnikId, int stavkaMenijaId);

    Task<List<StavkaMenija>> ListirajOmiljenaJelaAsync(string korisnikId);

    Task<Dictionary<int, int>> BrojLajkovaZaStavkeAsync(IEnumerable<int> stavkaMenijaIds);
}
