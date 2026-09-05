using Domain.Entiteti;

namespace Repository.Interfejsi;

public interface IPostavkeRepository : IRepository<PostavkeRestorana>
{
    Task<PostavkeRestorana?> DobaviAsync();
}
