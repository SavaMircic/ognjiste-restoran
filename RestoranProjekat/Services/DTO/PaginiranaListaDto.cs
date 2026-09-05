namespace Services.DTO;

public class PaginiranaListaDto<T>
{
    public List<T> Podaci { get; set; } = new();
    public int UkupnoZapisa { get; set; }
    public int TrenutnaStrana { get; set; }
    public int UkupnoStrana { get; set; }
}
