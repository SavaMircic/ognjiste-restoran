using Domain.Entiteti;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Repository.Konfiguracije;

public class SmenaKonfiguracija : IEntityTypeConfiguration<Smena>
{
    public void Configure(EntityTypeBuilder<Smena> builder)
    {
        builder.HasIndex(s => new { s.ZaposleniId, s.Datum });

        builder.HasOne(s => s.Zaposleni)
            .WithMany()
            .HasForeignKey(s => s.ZaposleniId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
