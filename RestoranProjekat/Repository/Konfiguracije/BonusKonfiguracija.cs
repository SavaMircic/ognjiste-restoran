using Domain.Entiteti;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Repository.Konfiguracije;

public class BonusKonfiguracija : IEntityTypeConfiguration<Bonus>
{
    public void Configure(EntityTypeBuilder<Bonus> builder)
    {
        builder.Property(b => b.Iznos).HasColumnType("decimal(10,2)");
        builder.Property(b => b.Razlog).IsRequired().HasMaxLength(500);

        builder.HasIndex(b => new { b.ZaposleniId, b.DatumPocetka });

        builder.HasOne(b => b.Zaposleni)
            .WithMany()
            .HasForeignKey(b => b.ZaposleniId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(b => b.DodelioZaposleni)
            .WithMany()
            .HasForeignKey(b => b.DodelioZaposleniId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
