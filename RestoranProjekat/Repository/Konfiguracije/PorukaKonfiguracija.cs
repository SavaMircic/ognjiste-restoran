using Domain.Entiteti;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Repository.Konfiguracije;

public class PorukaKonfiguracija : IEntityTypeConfiguration<Poruka>
{
    public void Configure(EntityTypeBuilder<Poruka> builder)
    {
        builder.Property(p => p.GostIme).HasMaxLength(100);
        builder.Property(p => p.GostEmail).HasMaxLength(256);
        builder.Property(p => p.GostTelefon).HasMaxLength(20);
        builder.Property(p => p.Kategorija).HasConversion<string>().HasMaxLength(20);
        builder.Property(p => p.Status).HasConversion<string>().HasMaxLength(20);
        builder.Property(p => p.Tekst).IsRequired().HasMaxLength(2000);
        builder.Property(p => p.Odgovor).HasMaxLength(2000);

        builder.HasIndex(p => p.Status);
        builder.HasIndex(p => p.DatumSlanja);

        builder.HasOne(p => p.Korisnik)
            .WithMany()
            .HasForeignKey(p => p.KorisnikId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(p => p.ObradioZaposleni)
            .WithMany()
            .HasForeignKey(p => p.ObradioZaposleniId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
