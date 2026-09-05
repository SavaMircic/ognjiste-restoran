using Domain.Entiteti;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Repository.Konfiguracije;

public class RecenzijaKonfiguracija : IEntityTypeConfiguration<Recenzija>
{
    public void Configure(EntityTypeBuilder<Recenzija> builder)
    {
        builder.Property(r => r.TipRecenzije).HasConversion<string>().HasMaxLength(20);
        builder.Property(r => r.Naslov).HasMaxLength(150);
        builder.Property(r => r.Tekst).IsRequired().HasMaxLength(2000);
        builder.Property(r => r.OdgovorRestorana).HasMaxLength(1000);

        builder.Property(r => r.Aktivan).HasDefaultValue(true);

        builder.ToTable(t => t.HasCheckConstraint("CK_Recenzije_Ocena", "[Ocena] >= 1 AND [Ocena] <= 5"));

        builder.HasIndex(r => new { r.KorisnikId, r.TipRecenzije, r.StavkaMenijaId })
            .IsUnique()
            .HasFilter("[StavkaMenijaId] IS NOT NULL");

        builder.HasIndex(r => new { r.KorisnikId, r.TipRecenzije })
            .IsUnique()
            .HasFilter("[StavkaMenijaId] IS NULL");

        builder.HasOne(r => r.Korisnik)
            .WithMany()
            .HasForeignKey(r => r.KorisnikId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(r => r.StavkaMenija)
            .WithMany()
            .HasForeignKey(r => r.StavkaMenijaId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
