using Domain.Entiteti;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Repository.Konfiguracije;

public class RezervacijaKonfiguracija : IEntityTypeConfiguration<Rezervacija>
{
    public void Configure(EntityTypeBuilder<Rezervacija> builder)
    {
        builder.Property(r => r.GostIme).HasMaxLength(100);
        builder.Property(r => r.GostEmail).HasMaxLength(256);
        builder.Property(r => r.GostTelefon).HasMaxLength(20);
        builder.Property(r => r.KodRezervacije).IsRequired().HasMaxLength(10);
        builder.Property(r => r.Status).HasConversion<string>().HasMaxLength(20);
        builder.Property(r => r.NacinKreiranja).HasConversion<string>().HasMaxLength(20);

        builder.HasIndex(r => new { r.KodRezervacije, r.StoId }).IsUnique();
        builder.HasIndex(r => r.DatumVreme);

        builder.HasOne(r => r.Korisnik)
            .WithMany()
            .HasForeignKey(r => r.KorisnikId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(r => r.Sto)
            .WithMany()
            .HasForeignKey(r => r.StoId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(r => r.KreiraoZaposleni)
            .WithMany()
            .HasForeignKey(r => r.KreiraoZaposleniId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
