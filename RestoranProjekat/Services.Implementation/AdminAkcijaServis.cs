using Domain.Entiteti;
using Repository.Interfejsi;
using Services.DTO;

namespace Services.Implementation;

public class AdminAkcijaServis : IAdminAkcijaServis
{
    private readonly IAdminAkcijaRepository _adminAkcijaRepository;
    private readonly IZaposleniRepository _zaposleniRepository;

    public AdminAkcijaServis(
        IAdminAkcijaRepository adminAkcijaRepository, IZaposleniRepository zaposleniRepository)
    {
        _adminAkcijaRepository = adminAkcijaRepository;
        _zaposleniRepository = zaposleniRepository;
    }

    public async Task ZabeleziAsync(string adminKorisnikId, string tipAkcije, string? ciljniKorisnikId, string opis)
    {
        var admin = await _zaposleniRepository.DobaviPoKorisnikIdAsync(adminKorisnikId);

        if (admin == null) return;

        await _adminAkcijaRepository.DodajAsync(new AdminAkcija
        {
            AdministratorId = admin.Id,
            TipAkcije = tipAkcije,
            CiljniKorisnikId = ciljniKorisnikId,
            Opis = opis,
            Datum = DateTime.UtcNow
        });

        await _adminAkcijaRepository.SacuvajPromeneAsync();
    }

    public async Task<PaginiranaListaDto<AdminAkcijaDto>> PretraziAsync(AdminAkcijePretragaDto filter)
    {
        var (podaci, ukupno) = await _adminAkcijaRepository.PretraziAsync(
            filter.TipAkcije, filter.CiljniKorisnikId, filter.Pretraga,
            filter.DatumOd, filter.DatumDo,
            filter.Strana, filter.VelicinaStrane);

        return new PaginiranaListaDto<AdminAkcijaDto>
        {
            Podaci = podaci.Select(a => new AdminAkcijaDto
            {
                Id = a.Id,
                AdministratorId = a.AdministratorId,
                AdministratorIme = a.Administrator?.Korisnik == null
                    ? string.Empty
                    : $"{a.Administrator.Korisnik.Ime} {a.Administrator.Korisnik.Prezime}",
                TipAkcije = a.TipAkcije,
                CiljniKorisnikId = a.CiljniKorisnikId,
                CiljniKorisnikEmail = a.CiljniKorisnik?.Email,
                Opis = a.Opis,
                Datum = a.Datum
            }).ToList(),
            UkupnoZapisa = ukupno,
            TrenutnaStrana = filter.Strana,
            UkupnoStrana = (int)Math.Ceiling(ukupno / (double)filter.VelicinaStrane)
        };
    }
}
