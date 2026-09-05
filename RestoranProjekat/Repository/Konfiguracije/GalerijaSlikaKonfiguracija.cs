using Domain.Entiteti;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Repository.Konfiguracije;

public class GalerijaSlikaKonfiguracija : IEntityTypeConfiguration<GalerijaSlika>
{
    public void Configure(EntityTypeBuilder<GalerijaSlika> builder)
    {
        builder.Property(g => g.SlikaUrl).IsRequired().HasMaxLength(500);
        builder.Property(g => g.Naslov).IsRequired().HasMaxLength(150);
        builder.Property(g => g.Opis).HasMaxLength(500);
        builder.Property(g => g.Grupa).IsRequired().HasMaxLength(100);

        builder.HasIndex(g => new { g.Aktivan, g.Grupa, g.Redosled });
    }
}
