using Domain.Entiteti;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Repository.Konfiguracije;

public class StavkaMenijaKonfiguracija : IEntityTypeConfiguration<StavkaMenija>
{
    public void Configure(EntityTypeBuilder<StavkaMenija> builder)
    {
        builder.Property(s => s.Naziv).IsRequired().HasMaxLength(150);
        builder.Property(s => s.Opis).IsRequired().HasMaxLength(2000);
        builder.Property(s => s.DetaljanOpis).HasMaxLength(4000);
        builder.Property(s => s.Cena).HasColumnType("decimal(10,2)");
        builder.Property(s => s.Popust).HasColumnType("decimal(5,2)");
        builder.Property(s => s.SlikaUrl).IsRequired().HasMaxLength(500);

        builder.Ignore(s => s.CenaSaPopustom);

        builder.HasOne(s => s.Kategorija)
            .WithMany(k => k.Stavke)
            .HasForeignKey(s => s.KategorijaId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
