using Domain.Entiteti;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Repository.Konfiguracije;

public class SlikaStavkeMenijaKonfiguracija : IEntityTypeConfiguration<SlikaStavkeMenija>
{
    public void Configure(EntityTypeBuilder<SlikaStavkeMenija> builder)
    {
        builder.Property(s => s.SlikaUrl).IsRequired().HasMaxLength(500);
        builder.Property(s => s.Opis).HasMaxLength(300);

        builder.HasIndex(s => new { s.StavkaMenijaId, s.Redosled });

        builder.HasOne(s => s.StavkaMenija)
            .WithMany(m => m.DodatneSlike)
            .HasForeignKey(s => s.StavkaMenijaId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
