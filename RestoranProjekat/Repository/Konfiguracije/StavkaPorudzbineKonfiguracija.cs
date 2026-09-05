using Domain.Entiteti;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Repository.Konfiguracije;

public class StavkaPorudzbineKonfiguracija : IEntityTypeConfiguration<StavkaPorudzbine>
{
    public void Configure(EntityTypeBuilder<StavkaPorudzbine> builder)
    {
        builder.Property(s => s.Napomena).HasMaxLength(500);
        builder.Property(s => s.CenaUTrenutkuNarudzbine).HasColumnType("decimal(10,2)");
        builder.Property(s => s.Status).HasConversion<string>().HasMaxLength(20);
        builder.HasIndex(s => s.Status);
        builder.HasIndex(s => s.VremeSlanja);

        builder.HasOne(s => s.Porudzbina)
            .WithMany(p => p.Stavke)
            .HasForeignKey(s => s.PorudzbinaId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(s => s.StavkaMenija)
            .WithMany()
            .HasForeignKey(s => s.StavkaMenijaId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(s => s.PripremioZaposleni)
            .WithMany()
            .HasForeignKey(s => s.PripremioZaposleniId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
