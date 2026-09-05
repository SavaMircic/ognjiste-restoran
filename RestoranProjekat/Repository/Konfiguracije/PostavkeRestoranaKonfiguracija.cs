using Domain.Entiteti;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Repository.Konfiguracije;

public class PostavkeRestoranaKonfiguracija : IEntityTypeConfiguration<PostavkeRestorana>
{
    public void Configure(EntityTypeBuilder<PostavkeRestorana> builder)
    {
        builder.Property(p => p.Id).ValueGeneratedNever();

        builder.Property(p => p.Adresa).IsRequired().HasMaxLength(200);
        builder.Property(p => p.Telefon).IsRequired().HasMaxLength(30);
        builder.Property(p => p.Email).IsRequired().HasMaxLength(100);
        builder.Property(p => p.RadnoVreme).IsRequired().HasMaxLength(200);
        builder.Property(p => p.OpisRestorana).HasMaxLength(2000);

        builder.Property(p => p.GeoSirina).HasColumnType("decimal(9,6)");
        builder.Property(p => p.GeoDuzina).HasColumnType("decimal(9,6)");
        builder.Property(p => p.FacebookUrl).HasMaxLength(300);
        builder.Property(p => p.InstagramUrl).HasMaxLength(300);

        builder.ToTable(t => t.HasCheckConstraint("CK_PostavkeRestorana_JedanRed", "[Id] = 1"));
    }
}
