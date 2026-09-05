using Domain.Entiteti;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Repository.Konfiguracije;

public class PrijavaProblemaKonfiguracija : IEntityTypeConfiguration<PrijavaProblema>
{
    public void Configure(EntityTypeBuilder<PrijavaProblema> builder)
    {
        builder.Property(p => p.Naslov).IsRequired().HasMaxLength(150);
        builder.Property(p => p.Opis).IsRequired().HasMaxLength(2000);
        builder.Property(p => p.Odgovor).HasMaxLength(1000);

        builder.Property(p => p.Kategorija).HasConversion<string>().HasMaxLength(20);
        builder.Property(p => p.Prioritet).HasConversion<string>().HasMaxLength(20);
        builder.Property(p => p.Status).HasConversion<string>().HasMaxLength(20);

        builder.HasIndex(p => new { p.Status, p.DatumPrijave });

        builder.HasOne(p => p.PrijavioZaposleni)
            .WithMany()
            .HasForeignKey(p => p.PrijavioZaposleniId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(p => p.ResioZaposleni)
            .WithMany()
            .HasForeignKey(p => p.ResioZaposleniId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
