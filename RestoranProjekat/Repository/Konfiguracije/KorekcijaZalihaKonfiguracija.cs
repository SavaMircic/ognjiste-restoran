using Domain.Entiteti;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Repository.Konfiguracije;

public class KorekcijaZalihaKonfiguracija : IEntityTypeConfiguration<KorekcijaZaliha>
{
    public void Configure(EntityTypeBuilder<KorekcijaZaliha> builder)
    {
        builder.Property(k => k.StaraKolicina).HasColumnType("decimal(10,3)");
        builder.Property(k => k.NovaKolicina).HasColumnType("decimal(10,3)");
        builder.Property(k => k.Razlog).IsRequired().HasMaxLength(500);
        builder.HasIndex(k => k.Datum);

        builder.HasOne(k => k.Namirnica)
            .WithMany()
            .HasForeignKey(k => k.NamirnicaId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(k => k.Zaposleni)
            .WithMany()
            .HasForeignKey(k => k.ZaposleniId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
