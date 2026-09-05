using Domain.Entiteti;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Repository.Konfiguracije;

public class AdminAkcijaKonfiguracija : IEntityTypeConfiguration<AdminAkcija>
{
    public void Configure(EntityTypeBuilder<AdminAkcija> builder)
    {
        builder.Property(a => a.TipAkcije).IsRequired().HasMaxLength(50);
        builder.Property(a => a.Opis).IsRequired().HasMaxLength(500);

        builder.HasIndex(a => a.Datum);
        builder.HasIndex(a => a.TipAkcije);
        builder.HasIndex(a => a.CiljniKorisnikId);

        builder.HasOne(a => a.Administrator)
            .WithMany()
            .HasForeignKey(a => a.AdministratorId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(a => a.CiljniKorisnik)
            .WithMany()
            .HasForeignKey(a => a.CiljniKorisnikId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}
