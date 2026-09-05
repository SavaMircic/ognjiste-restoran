namespace Services.DTO;

public class PaginacijaParametriDto
{
    private const int MaksVelicinaStrane = 100;
    private int _velicinaStrane = 20;
    private int _strana = 1;

    public int Strana
    {
        get => _strana;
        set => _strana = value < 1 ? 1 : value;
    }

    public int VelicinaStrane
    {
        get => _velicinaStrane;
        set => _velicinaStrane = value > MaksVelicinaStrane ? MaksVelicinaStrane : (value < 1 ? 1 : value);
    }

    public string? Pretraga { get; set; }
}
