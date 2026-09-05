using Services.DTO;

namespace Services;

public interface IPostavkeServis
{
    Task<PostavkeDto> DobaviAsync();

    Task<PostavkeDto> IzmeniAsync(IzmenaPostavkiDto dto);
}
