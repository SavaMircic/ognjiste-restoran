using Domain.Entiteti;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Repository.Konfiguracije;

public class AuditLogKonfiguracija : IEntityTypeConfiguration<AuditLog>
{
    public void Configure(EntityTypeBuilder<AuditLog> builder)
    {
        builder.Property(a => a.NazivSlucajaKoriscenja).IsRequired().HasMaxLength(150);
        builder.HasIndex(a => a.NazivSlucajaKoriscenja);
        builder.HasIndex(a => a.Datum);

        builder.HasOne(a => a.Korisnik)
            .WithMany()
            .HasForeignKey(a => a.KorisnikId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}
