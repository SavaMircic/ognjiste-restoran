using Domain.Entiteti;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Repository.Konfiguracije;

public class PorudzbinaKonfiguracija : IEntityTypeConfiguration<Porudzbina>
{
    public void Configure(EntityTypeBuilder<Porudzbina> builder)
    {
        builder.Property(p => p.Status).HasConversion<string>().HasMaxLength(20);
        builder.Property(p => p.NacinPlacanja).HasConversion<string>().HasMaxLength(20);
        builder.Property(p => p.IznosNapojnice).HasColumnType("decimal(10,2)");

        builder.HasOne(p => p.Sto)
            .WithMany(s => s.Porudzbine)
            .HasForeignKey(p => p.StoId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(p => p.Konobar)
            .WithMany()
            .HasForeignKey(p => p.KonobarId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(p => p.Rezervacija)
            .WithMany()
            .HasForeignKey(p => p.RezervacijaId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
