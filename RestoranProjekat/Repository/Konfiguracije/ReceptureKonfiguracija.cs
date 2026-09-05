using Domain.Entiteti;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Repository.Konfiguracije;

public class ReceptureKonfiguracija : IEntityTypeConfiguration<Receptura>
{
    public void Configure(EntityTypeBuilder<Receptura> builder)
    {
        builder.Property(r => r.Kolicina).HasColumnType("decimal(10,3)");

        builder.HasIndex(r => new { r.StavkaMenijaId, r.NamirnicaId }).IsUnique();

        builder.HasOne(r => r.StavkaMenija)
            .WithMany(s => s.Receptura)
            .HasForeignKey(r => r.StavkaMenijaId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(r => r.Namirnica)
            .WithMany()
            .HasForeignKey(r => r.NamirnicaId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
