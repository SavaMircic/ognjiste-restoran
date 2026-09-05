using Domain.Enumi;

namespace Services.DTO;

public class PrihodUpitDto : IzvestajPeriodDto
{
    public GrupisanjePrihoda GrupisanjePo { get; set; } = GrupisanjePrihoda.Dan;
}
