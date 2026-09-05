using Domain.Entiteti;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Repository.Konfiguracije;

public class StoKonfiguracija : IEntityTypeConfiguration<Sto>
{
    public void Configure(EntityTypeBuilder<Sto> builder)
    {
        builder.Property(s => s.TrenutniStatus).HasConversion<string>().HasMaxLength(20);
        builder.HasIndex(s => s.BrojStola).IsUnique();
    }
}
