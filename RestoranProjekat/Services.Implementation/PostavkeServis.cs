using Domain.Entiteti;
using Repository.Interfejsi;
using Services.DTO;

namespace Services.Implementation;

public class PostavkeServis : IPostavkeServis
{
    private readonly IPostavkeRepository _postavkeRepository;

    public PostavkeServis(IPostavkeRepository postavkeRepository)
    {
        _postavkeRepository = postavkeRepository;
    }

    public async Task<PostavkeDto> DobaviAsync()
    {
        var postavke = await _postavkeRepository.DobaviAsync();

        return postavke == null ? new PostavkeDto() : Mapiraj(postavke);
    }

    public async Task<PostavkeDto> IzmeniAsync(IzmenaPostavkiDto dto)
    {
        var postavke = await _postavkeRepository.DobaviAsync();

        if (postavke == null)
        {
            postavke = new PostavkeRestorana { Id = 1 };
            await _postavkeRepository.DodajAsync(postavke);
        }

        postavke.Adresa = dto.Adresa;
        postavke.Telefon = dto.Telefon;
        postavke.Email = dto.Email;
        postavke.RadnoVreme = dto.RadnoVreme;
        postavke.OpisRestorana = dto.OpisRestorana;
        postavke.GeoSirina = dto.GeoSirina;
        postavke.GeoDuzina = dto.GeoDuzina;
        postavke.FacebookUrl = dto.FacebookUrl;
        postavke.InstagramUrl = dto.InstagramUrl;

        await _postavkeRepository.SacuvajPromeneAsync();
        return Mapiraj(postavke);
    }

    private static PostavkeDto Mapiraj(PostavkeRestorana p) => new()
    {
        Adresa = p.Adresa,
        Telefon = p.Telefon,
        Email = p.Email,
        RadnoVreme = p.RadnoVreme,
        OpisRestorana = p.OpisRestorana,
        GeoSirina = p.GeoSirina,
        GeoDuzina = p.GeoDuzina,
        FacebookUrl = p.FacebookUrl,
        InstagramUrl = p.InstagramUrl
    };
}
