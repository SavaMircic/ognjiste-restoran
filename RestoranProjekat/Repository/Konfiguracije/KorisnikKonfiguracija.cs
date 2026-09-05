using Domain.Entiteti;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Repository.Konfiguracije;

public class KorisnikKonfiguracija : IEntityTypeConfiguration<Korisnik>
{
    public void Configure(EntityTypeBuilder<Korisnik> builder)
    {
        builder.Property(k => k.Ime).IsRequired().HasMaxLength(100);
        builder.Property(k => k.Prezime).IsRequired().HasMaxLength(100);
        builder.Property(k => k.SlikaUrl).HasMaxLength(500);

    }
}
