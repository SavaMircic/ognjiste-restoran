using Domain.Entiteti;
using Domain.Konstante;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Repository;

public static partial class PocetniPodaci
{
    public static async Task PopuniAsync(
        RoleManager<IdentityRole> roleManager, UserManager<Korisnik> userManager, RestoranDbContext kontekst)
    {
        foreach (var uloga in Uloge.Sve)
        {
            if (!await roleManager.RoleExistsAsync(uloga))
                await roleManager.CreateAsync(new IdentityRole(uloga));
        }

        await PopuniOsobljeAsync(userManager, kontekst);
        await PopuniGosteAsync(userManager, kontekst);
        await PopuniProfileOsobljaAsync(kontekst);
        await PopuniMenijAsync(kontekst);
        await PopuniZaliheAsync(kontekst);
        await PopuniStoloveAsync(kontekst);
        await PopuniRezervacijeAsync(kontekst);
        await ZatvoriZaostaleRacuneAsync(kontekst);
        await PopuniOtvorenePorudzbineAsync(kontekst);
        await PopuniIstorijuPorudzbinaAsync(kontekst);
        await PopuniRecenzijeILajkoveAsync(kontekst);
        await PopuniPorukeAsync(kontekst);
        await PopuniRasporedIBonuseAsync(kontekst);
        await PopuniGalerijuIPostavkeAsync(kontekst);
        await PopuniAdministracijuAsync(kontekst);

        await OsveziDemoPodatkeAsync(kontekst);
    }

    private static async Task<Korisnik?> KreirajKorisnikaAsync(
        UserManager<Korisnik> userManager, RestoranDbContext kontekst,
        string email, string lozinka, string ime, string prezime, string uloga, bool jeZaposleni,
        DateTime? datumRegistracije = null)
    {
        var postojeci = await userManager.FindByEmailAsync(email);
        if (postojeci != null) return postojeci;

        var korisnik = new Korisnik
        {
            UserName = email,
            Email = email,
            Ime = ime,
            Prezime = prezime,
            DatumRegistracije = datumRegistracije ?? DateTime.UtcNow,
            Aktivan = true,
            EmailConfirmed = true
        };

        var rezultat = await userManager.CreateAsync(korisnik, lozinka);
        if (!rezultat.Succeeded) return null;

        await userManager.AddToRoleAsync(korisnik, uloga);

        if (jeZaposleni)
        {
            kontekst.Zaposleni.Add(new Zaposleni
            {
                KorisnikId = korisnik.Id,
                DatumZaposlenja = datumRegistracije ?? DateTime.UtcNow
            });
            await kontekst.SaveChangesAsync();
        }

        return korisnik;
    }

    private static async Task<Zaposleni?> ZaposleniPoMejluAsync(RestoranDbContext kontekst, string email) =>
        await kontekst.Zaposleni.Include(z => z.Korisnik).FirstOrDefaultAsync(z => z.Korisnik.Email == email);
}
