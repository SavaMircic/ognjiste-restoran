using Domain.Entiteti;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Repository;

public class RestoranDbContext : IdentityDbContext<Korisnik>
{
    public RestoranDbContext(DbContextOptions<RestoranDbContext> options) : base(options) { }

    public DbSet<Zaposleni> Zaposleni => Set<Zaposleni>();
    public DbSet<AuditLog> AuditLogovi => Set<AuditLog>();
    public DbSet<KategorijaMenija> KategorijeMenija => Set<KategorijaMenija>();
    public DbSet<StavkaMenija> StavkeMenija => Set<StavkaMenija>();
    public DbSet<Namirnica> Namirnice => Set<Namirnica>();
    public DbSet<Receptura> Recepture => Set<Receptura>();
    public DbSet<Sto> Stolovi => Set<Sto>();
    public DbSet<Porudzbina> Porudzbine => Set<Porudzbina>();
    public DbSet<StavkaPorudzbine> StavkePorudzbine => Set<StavkaPorudzbine>();
    public DbSet<Rezervacija> Rezervacije => Set<Rezervacija>();
    public DbSet<Recenzija> Recenzije => Set<Recenzija>();
    public DbSet<LajkJela> LajkoviJela => Set<LajkJela>();
    public DbSet<Poruka> Poruke => Set<Poruka>();
    public DbSet<KorekcijaZaliha> KorekcijeZaliha => Set<KorekcijaZaliha>();
    public DbSet<Smena> Smene => Set<Smena>();
    public DbSet<Bonus> Bonusi => Set<Bonus>();
    public DbSet<PostavkeRestorana> PostavkeRestorana => Set<PostavkeRestorana>();
    public DbSet<GalerijaSlika> GalerijaSlike => Set<GalerijaSlika>();
    public DbSet<SlikaStavkeMenija> SlikeStavkiMenija => Set<SlikaStavkeMenija>();
    public DbSet<AdminAkcija> AdminAkcije => Set<AdminAkcija>();
    public DbSet<PrijavaProblema> PrijaveProblema => Set<PrijavaProblema>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<Korisnik>().ToTable("Korisnici");
        builder.Entity<IdentityRole>().ToTable("Uloge");
        builder.Entity<IdentityUserRole<string>>().ToTable("KorisnikUloge");
        builder.Entity<IdentityUserClaim<string>>().ToTable("KorisnikTvrdnje");
        builder.Entity<IdentityUserLogin<string>>().ToTable("KorisnickePrijave");
        builder.Entity<IdentityUserToken<string>>().ToTable("KorisnikTokeni");
        builder.Entity<IdentityRoleClaim<string>>().ToTable("UlogaTvrdnje");

        builder.ApplyConfigurationsFromAssembly(typeof(RestoranDbContext).Assembly);
    }
}
