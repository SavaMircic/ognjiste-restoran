using Domain.Entiteti;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Repository.Konfiguracije;

public class ZaposleniKonfiguracija : IEntityTypeConfiguration<Zaposleni>
{
    public void Configure(EntityTypeBuilder<Zaposleni> builder)
    {
        builder.Property(z => z.SlikaUrl).HasMaxLength(500);
        builder.Property(z => z.Biografija).HasMaxLength(1000);
        builder.Property(z => z.LinkedInUrl).HasMaxLength(300);
        builder.Property(z => z.InstagramUrl).HasMaxLength(300);

        builder.HasIndex(z => new { z.PrikaziNaSajtu, z.Redosled });

        builder.HasOne(z => z.Korisnik)
            .WithOne(k => k.Zaposleni)
            .HasForeignKey<Zaposleni>(z => z.KorisnikId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
