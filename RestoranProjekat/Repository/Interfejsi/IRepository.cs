namespace Repository.Interfejsi;

public interface IRepository<T> where T : class
{
    Task<T?> DobaviPoIdAsync(object id);
    IQueryable<T> Upit();
    Task<T> DodajAsync(T entitet);
    void Azuriraj(T entitet);
    void Obrisi(T entitet);
    Task<int> SacuvajPromeneAsync();
}
