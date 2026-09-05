using Domain.Entiteti;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Repository.Konfiguracije;

public class NamirnicaKonfiguracija : IEntityTypeConfiguration<Namirnica>
{
    public void Configure(EntityTypeBuilder<Namirnica> builder)
    {
        builder.Property(n => n.Naziv).IsRequired().HasMaxLength(100);
        builder.Property(n => n.JedinicaMere).HasConversion<string>().HasMaxLength(10);
        builder.Property(n => n.TrenutnaKolicina).HasColumnType("decimal(10,3)");
        builder.Property(n => n.MinimalniPrag).HasColumnType("decimal(10,3)");

        builder.HasIndex(n => n.Naziv).IsUnique();
    }
}
