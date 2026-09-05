using Domain.Entiteti;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Repository.Konfiguracije;

public class KategorijaMenijaKonfiguracija : IEntityTypeConfiguration<KategorijaMenija>
{
    public void Configure(EntityTypeBuilder<KategorijaMenija> builder)
    {
        builder.Property(k => k.Naziv).IsRequired().HasMaxLength(100);
        builder.Property(k => k.Opis).HasMaxLength(500);
        builder.Property(k => k.Odrediste).HasConversion<string>().HasMaxLength(20);
    }
}
