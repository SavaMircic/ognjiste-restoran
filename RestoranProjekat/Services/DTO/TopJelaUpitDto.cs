using Domain.Enumi;

namespace Services.DTO;

public class TopJelaUpitDto : IzvestajPeriodDto
{
    public RedosledTopJela Redosled { get; set; } = RedosledTopJela.Najprodavanije;
}
