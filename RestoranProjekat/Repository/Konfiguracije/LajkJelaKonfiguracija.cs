using Domain.Entiteti;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Repository.Konfiguracije;

public class LajkJelaKonfiguracija : IEntityTypeConfiguration<LajkJela>
{
    public void Configure(EntityTypeBuilder<LajkJela> builder)
    {
        builder.HasIndex(l => new { l.KorisnikId, l.StavkaMenijaId }).IsUnique();

        builder.HasOne(l => l.Korisnik)
            .WithMany()
            .HasForeignKey(l => l.KorisnikId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(l => l.StavkaMenija)
            .WithMany()
            .HasForeignKey(l => l.StavkaMenijaId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
