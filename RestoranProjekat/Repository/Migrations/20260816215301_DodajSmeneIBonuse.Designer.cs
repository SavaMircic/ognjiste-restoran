using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Repository;

#nullable disable

namespace Repository.Migrations
{
    [DbContext(typeof(RestoranDbContext))]
    [Migration("20260816215301_DodajSmeneIBonuse")]
    partial class DodajSmeneIBonuse
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "10.0.11")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("Domain.Entiteti.AuditLog", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<DateTime>("Datum")
                        .HasColumnType("datetime2");

                    b.Property<string>("KorisnikId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("NazivSlucajaKoriscenja")
                        .IsRequired()
                        .HasMaxLength(150)
                        .HasColumnType("nvarchar(150)");

                    b.Property<string>("Poruka")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("UspesnoIzvrseno")
                        .HasColumnType("bit");

                    b.HasKey("Id");

                    b.HasIndex("Datum");

                    b.HasIndex("KorisnikId");

                    b.HasIndex("NazivSlucajaKoriscenja");

                    b.ToTable("AuditLogovi");
                });

            modelBuilder.Entity("Domain.Entiteti.Bonus", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<DateTime>("DatumDodele")
                        .HasColumnType("datetime2");

                    b.Property<DateOnly>("DatumKraja")
                        .HasColumnType("date");

                    b.Property<DateOnly>("DatumPocetka")
                        .HasColumnType("date");

                    b.Property<int>("DodelioZaposleniId")
                        .HasColumnType("int");

                    b.Property<decimal>("Iznos")
                        .HasColumnType("decimal(10,2)");

                    b.Property<string>("Razlog")
                        .IsRequired()
                        .HasMaxLength(500)
                        .HasColumnType("nvarchar(500)");

                    b.Property<int>("ZaposleniId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("DodelioZaposleniId");

                    b.HasIndex("ZaposleniId", "DatumPocetka");

                    b.ToTable("Bonusi");
                });

            modelBuilder.Entity("Domain.Entiteti.KategorijaMenija", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Naziv")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<string>("Odrediste")
                        .IsRequired()
                        .HasMaxLength(20)
                        .HasColumnType("nvarchar(20)");

                    b.Property<string>("Opis")
                        .HasMaxLength(500)
                        .HasColumnType("nvarchar(500)");

                    b.Property<int>("Redosled")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.ToTable("KategorijeMenija");
                });

            modelBuilder.Entity("Domain.Entiteti.KorekcijaZaliha", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<DateTime>("Datum")
                        .HasColumnType("datetime2");

                    b.Property<int>("NamirnicaId")
                        .HasColumnType("int");

                    b.Property<decimal>("NovaKolicina")
                        .HasColumnType("decimal(10,3)");

                    b.Property<string>("Razlog")
                        .IsRequired()
                        .HasMaxLength(500)
                        .HasColumnType("nvarchar(500)");

                    b.Property<decimal>("StaraKolicina")
                        .HasColumnType("decimal(10,3)");

                    b.Property<int>("ZaposleniId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("Datum");

                    b.HasIndex("NamirnicaId");

                    b.HasIndex("ZaposleniId");

                    b.ToTable("KorekcijeZaliha");
                });

            modelBuilder.Entity("Domain.Entiteti.Korisnik", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("nvarchar(450)");

                    b.Property<int>("AccessFailedCount")
                        .HasColumnType("int");

                    b.Property<bool>("Aktivan")
                        .HasColumnType("bit");

                    b.Property<DateTime?>("BlokiranDo")
                        .HasColumnType("datetime2");

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("DatumRegistracije")
                        .HasColumnType("datetime2");

                    b.Property<string>("Email")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.Property<bool>("EmailConfirmed")
                        .HasColumnType("bit");

                    b.Property<string>("Ime")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<bool>("LockoutEnabled")
                        .HasColumnType("bit");

                    b.Property<DateTimeOffset?>("LockoutEnd")
                        .HasColumnType("datetimeoffset");

                    b.Property<string>("NormalizedEmail")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.Property<string>("NormalizedUserName")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.Property<string>("PasswordHash")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("PhoneNumber")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("PhoneNumberConfirmed")
                        .HasColumnType("bit");

                    b.Property<string>("Prezime")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<string>("RefreshToken")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("RefreshTokenIstice")
                        .HasColumnType("datetime2");

                    b.Property<string>("SecurityStamp")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("TwoFactorEnabled")
                        .HasColumnType("bit");

                    b.Property<string>("UserName")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.Property<DateTime?>("ZabranaKomentarisanjaDo")
                        .HasColumnType("datetime2");

                    b.HasKey("Id");

                    b.HasIndex("NormalizedEmail")
                        .HasDatabaseName("EmailIndex");

                    b.HasIndex("NormalizedUserName")
                        .IsUnique()
                        .HasDatabaseName("UserNameIndex")
                        .HasFilter("[NormalizedUserName] IS NOT NULL");

                    b.ToTable("Korisnici", (string)null);
                });

            modelBuilder.Entity("Domain.Entiteti.LajkJela", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<DateTime>("DatumKreiranja")
                        .HasColumnType("datetime2");

                    b.Property<string>("KorisnikId")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.Property<int>("StavkaMenijaId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("StavkaMenijaId");

                    b.HasIndex("KorisnikId", "StavkaMenijaId")
                        .IsUnique();

                    b.ToTable("LajkoviJela");
                });

            modelBuilder.Entity("Domain.Entiteti.Namirnica", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("JedinicaMere")
                        .IsRequired()
                        .HasMaxLength(10)
                        .HasColumnType("nvarchar(10)");

                    b.Property<decimal>("MinimalniPrag")
                        .HasColumnType("decimal(10,3)");

                    b.Property<string>("Naziv")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<decimal>("TrenutnaKolicina")
                        .HasColumnType("decimal(10,3)");

                    b.HasKey("Id");

                    b.HasIndex("Naziv")
                        .IsUnique();

                    b.ToTable("Namirnice");
                });

            modelBuilder.Entity("Domain.Entiteti.Porudzbina", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<decimal?>("IznosNapojnice")
                        .HasColumnType("decimal(10,2)");

                    b.Property<int>("KonobarId")
                        .HasColumnType("int");

                    b.Property<string>("NacinPlacanja")
                        .HasMaxLength(20)
                        .HasColumnType("nvarchar(20)");

                    b.Property<int?>("RezervacijaId")
                        .HasColumnType("int");

                    b.Property<string>("Status")
                        .IsRequired()
                        .HasMaxLength(20)
                        .HasColumnType("nvarchar(20)");

                    b.Property<int>("StoId")
                        .HasColumnType("int");

                    b.Property<DateTime>("VremeOtvaranja")
                        .HasColumnType("datetime2");

                    b.Property<DateTime?>("VremeZatvaranja")
                        .HasColumnType("datetime2");

                    b.HasKey("Id");

                    b.HasIndex("KonobarId");

                    b.HasIndex("RezervacijaId");

                    b.HasIndex("StoId");

                    b.ToTable("Porudzbine");
                });

            modelBuilder.Entity("Domain.Entiteti.Poruka", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<DateTime?>("DatumOdgovora")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("DatumSlanja")
                        .HasColumnType("datetime2");

                    b.Property<string>("GostEmail")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.Property<string>("GostIme")
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<string>("GostTelefon")
                        .HasMaxLength(20)
                        .HasColumnType("nvarchar(20)");

                    b.Property<string>("Kategorija")
                        .IsRequired()
                        .HasMaxLength(20)
                        .HasColumnType("nvarchar(20)");

                    b.Property<string>("KorisnikId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<int?>("ObradioZaposleniId")
                        .HasColumnType("int");

                    b.Property<string>("Odgovor")
                        .HasMaxLength(2000)
                        .HasColumnType("nvarchar(2000)");

                    b.Property<string>("Status")
                        .IsRequired()
                        .HasMaxLength(20)
                        .HasColumnType("nvarchar(20)");

                    b.Property<string>("Tekst")
                        .IsRequired()
                        .HasMaxLength(2000)
                        .HasColumnType("nvarchar(2000)");

                    b.Property<int?>("ZeljeniBrojGostiju")
                        .HasColumnType("int");

                    b.Property<DateTime?>("ZeljeniDatumVreme")
                        .HasColumnType("datetime2");

                    b.HasKey("Id");

                    b.HasIndex("DatumSlanja");

                    b.HasIndex("KorisnikId");

                    b.HasIndex("ObradioZaposleniId");

                    b.HasIndex("Status");

                    b.ToTable("Poruke");
                });

            modelBuilder.Entity("Domain.Entiteti.Recenzija", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<DateTime?>("DatumIzmene")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("DatumKreiranja")
                        .HasColumnType("datetime2");

                    b.Property<DateTime?>("DatumOdgovora")
                        .HasColumnType("datetime2");

                    b.Property<string>("KorisnikId")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("Naslov")
                        .HasMaxLength(150)
                        .HasColumnType("nvarchar(150)");

                    b.Property<int>("Ocena")
                        .HasColumnType("int");

                    b.Property<string>("OdgovorRestorana")
                        .HasMaxLength(1000)
                        .HasColumnType("nvarchar(1000)");

                    b.Property<int?>("StavkaMenijaId")
                        .HasColumnType("int");

                    b.Property<string>("Tekst")
                        .IsRequired()
                        .HasMaxLength(2000)
                        .HasColumnType("nvarchar(2000)");

                    b.Property<string>("TipRecenzije")
                        .IsRequired()
                        .HasMaxLength(20)
                        .HasColumnType("nvarchar(20)");

                    b.HasKey("Id");

                    b.HasIndex("StavkaMenijaId");

                    b.HasIndex("KorisnikId", "TipRecenzije")
                        .IsUnique()
                        .HasFilter("[StavkaMenijaId] IS NULL");

                    b.HasIndex("KorisnikId", "TipRecenzije", "StavkaMenijaId")
                        .IsUnique()
                        .HasFilter("[StavkaMenijaId] IS NOT NULL");

                    b.ToTable("Recenzije", t =>
                        {
                            t.HasCheckConstraint("CK_Recenzije_Ocena", "[Ocena] >= 1 AND [Ocena] <= 5");
                        });
                });

            modelBuilder.Entity("Domain.Entiteti.Receptura", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<decimal>("Kolicina")
                        .HasColumnType("decimal(10,3)");

                    b.Property<int>("NamirnicaId")
                        .HasColumnType("int");

                    b.Property<int>("StavkaMenijaId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("NamirnicaId");

                    b.HasIndex("StavkaMenijaId", "NamirnicaId")
                        .IsUnique();

                    b.ToTable("Recepture");
                });

            modelBuilder.Entity("Domain.Entiteti.Rezervacija", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int>("BrojGostiju")
                        .HasColumnType("int");

                    b.Property<DateTime>("DatumKreiranja")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("DatumVreme")
                        .HasColumnType("datetime2");

                    b.Property<string>("GostEmail")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.Property<string>("GostIme")
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<string>("GostTelefon")
                        .HasMaxLength(20)
                        .HasColumnType("nvarchar(20)");

                    b.Property<string>("KodRezervacije")
                        .IsRequired()
                        .HasMaxLength(10)
                        .HasColumnType("nvarchar(10)");

                    b.Property<string>("KorisnikId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<int?>("KreiraoZaposleniId")
                        .HasColumnType("int");

                    b.Property<string>("NacinKreiranja")
                        .IsRequired()
                        .HasMaxLength(20)
                        .HasColumnType("nvarchar(20)");

                    b.Property<string>("Status")
                        .IsRequired()
                        .HasMaxLength(20)
                        .HasColumnType("nvarchar(20)");

                    b.Property<int>("StoId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("DatumVreme");

                    b.HasIndex("KodRezervacije")
                        .IsUnique();

                    b.HasIndex("KorisnikId");

                    b.HasIndex("KreiraoZaposleniId");

                    b.HasIndex("StoId");

                    b.ToTable("Rezervacije");
                });

            modelBuilder.Entity("Domain.Entiteti.Smena", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<DateOnly>("Datum")
                        .HasColumnType("date");

                    b.Property<TimeOnly>("VremeKraja")
                        .HasColumnType("time");

                    b.Property<TimeOnly>("VremePocetka")
                        .HasColumnType("time");

                    b.Property<int>("ZaposleniId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("ZaposleniId", "Datum");

                    b.ToTable("Smene");
                });

            modelBuilder.Entity("Domain.Entiteti.StavkaMenija", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<decimal>("Cena")
                        .HasColumnType("decimal(10,2)");

                    b.Property<DateTime>("DatumKreiranja")
                        .HasColumnType("datetime2");

                    b.Property<bool>("Dostupno")
                        .HasColumnType("bit");

                    b.Property<int>("KategorijaId")
                        .HasColumnType("int");

                    b.Property<string>("Naziv")
                        .IsRequired()
                        .HasMaxLength(150)
                        .HasColumnType("nvarchar(150)");

                    b.Property<string>("Opis")
                        .IsRequired()
                        .HasMaxLength(2000)
                        .HasColumnType("nvarchar(2000)");

                    b.Property<decimal?>("Popust")
                        .HasColumnType("decimal(5,2)");

                    b.Property<string>("SlikaUrl")
                        .IsRequired()
                        .HasMaxLength(500)
                        .HasColumnType("nvarchar(500)");

                    b.HasKey("Id");

                    b.HasIndex("KategorijaId");

                    b.ToTable("StavkeMenija");
                });

            modelBuilder.Entity("Domain.Entiteti.StavkaPorudzbine", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<decimal>("CenaUTrenutkuNarudzbine")
                        .HasColumnType("decimal(10,2)");

                    b.Property<int>("Kolicina")
                        .HasColumnType("int");

                    b.Property<string>("Napomena")
                        .HasMaxLength(500)
                        .HasColumnType("nvarchar(500)");

                    b.Property<int>("PorudzbinaId")
                        .HasColumnType("int");

                    b.Property<int?>("PripremioZaposleniId")
                        .HasColumnType("int");

                    b.Property<string>("Status")
                        .IsRequired()
                        .HasMaxLength(20)
                        .HasColumnType("nvarchar(20)");

                    b.Property<int>("StavkaMenijaId")
                        .HasColumnType("int");

                    b.Property<DateTime?>("VremePreuzimanja")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("VremeSlanja")
                        .HasColumnType("datetime2");

                    b.Property<DateTime?>("VremeZavrsetka")
                        .HasColumnType("datetime2");

                    b.HasKey("Id");

                    b.HasIndex("PorudzbinaId");

                    b.HasIndex("PripremioZaposleniId");

                    b.HasIndex("Status");

                    b.HasIndex("StavkaMenijaId");

                    b.HasIndex("VremeSlanja");

                    b.ToTable("StavkePorudzbine");
                });

            modelBuilder.Entity("Domain.Entiteti.Sto", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int>("BrojStola")
                        .HasColumnType("int");

                    b.Property<int>("Kapacitet")
                        .HasColumnType("int");

                    b.Property<string>("TrenutniStatus")
                        .IsRequired()
                        .HasMaxLength(20)
                        .HasColumnType("nvarchar(20)");

                    b.HasKey("Id");

                    b.HasIndex("BrojStola")
                        .IsUnique();

                    b.ToTable("Stolovi");
                });

            modelBuilder.Entity("Domain.Entiteti.Zaposleni", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<DateTime>("DatumZaposlenja")
                        .HasColumnType("datetime2");

                    b.Property<string>("KorisnikId")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("Id");

                    b.HasIndex("KorisnikId")
                        .IsUnique();

                    b.ToTable("Zaposleni");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRole", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Name")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.Property<string>("NormalizedName")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.HasKey("Id");

                    b.HasIndex("NormalizedName")
                        .IsUnique()
                        .HasDatabaseName("RoleNameIndex")
                        .HasFilter("[NormalizedName] IS NOT NULL");

                    b.ToTable("Uloge", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<string>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("ClaimType")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ClaimValue")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("RoleId")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("Id");

                    b.HasIndex("RoleId");

                    b.ToTable("UlogaTvrdnje", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<string>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("ClaimType")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ClaimValue")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("UserId")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("KorisnikTvrdnje", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<string>", b =>
                {
                    b.Property<string>("LoginProvider")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("ProviderKey")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("ProviderDisplayName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("UserId")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("LoginProvider", "ProviderKey");

                    b.HasIndex("UserId");

                    b.ToTable("KorisnickePrijave", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserRole<string>", b =>
                {
                    b.Property<string>("UserId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("RoleId")
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("UserId", "RoleId");

                    b.HasIndex("RoleId");

                    b.ToTable("KorisnikUloge", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<string>", b =>
                {
                    b.Property<string>("UserId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("LoginProvider")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("Value")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("UserId", "LoginProvider", "Name");

                    b.ToTable("KorisnikTokeni", (string)null);
                });

            modelBuilder.Entity("Domain.Entiteti.AuditLog", b =>
                {
                    b.HasOne("Domain.Entiteti.Korisnik", "Korisnik")
                        .WithMany()
                        .HasForeignKey("KorisnikId")
                        .OnDelete(DeleteBehavior.SetNull);

                    b.Navigation("Korisnik");
                });

            modelBuilder.Entity("Domain.Entiteti.Bonus", b =>
                {
                    b.HasOne("Domain.Entiteti.Zaposleni", "DodelioZaposleni")
                        .WithMany()
                        .HasForeignKey("DodelioZaposleniId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("Domain.Entiteti.Zaposleni", "Zaposleni")
                        .WithMany()
                        .HasForeignKey("ZaposleniId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("DodelioZaposleni");

                    b.Navigation("Zaposleni");
                });

            modelBuilder.Entity("Domain.Entiteti.KorekcijaZaliha", b =>
                {
                    b.HasOne("Domain.Entiteti.Namirnica", "Namirnica")
                        .WithMany()
                        .HasForeignKey("NamirnicaId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Domain.Entiteti.Zaposleni", "Zaposleni")
                        .WithMany()
                        .HasForeignKey("ZaposleniId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("Namirnica");

                    b.Navigation("Zaposleni");
                });

            modelBuilder.Entity("Domain.Entiteti.LajkJela", b =>
                {
                    b.HasOne("Domain.Entiteti.Korisnik", "Korisnik")
                        .WithMany()
                        .HasForeignKey("KorisnikId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("Domain.Entiteti.StavkaMenija", "StavkaMenija")
                        .WithMany()
                        .HasForeignKey("StavkaMenijaId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("Korisnik");

                    b.Navigation("StavkaMenija");
                });

            modelBuilder.Entity("Domain.Entiteti.Porudzbina", b =>
                {
                    b.HasOne("Domain.Entiteti.Zaposleni", "Konobar")
                        .WithMany()
                        .HasForeignKey("KonobarId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("Domain.Entiteti.Rezervacija", "Rezervacija")
                        .WithMany()
                        .HasForeignKey("RezervacijaId")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.HasOne("Domain.Entiteti.Sto", "Sto")
                        .WithMany("Porudzbine")
                        .HasForeignKey("StoId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("Konobar");

                    b.Navigation("Rezervacija");

                    b.Navigation("Sto");
                });

            modelBuilder.Entity("Domain.Entiteti.Poruka", b =>
                {
                    b.HasOne("Domain.Entiteti.Korisnik", "Korisnik")
                        .WithMany()
                        .HasForeignKey("KorisnikId")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.HasOne("Domain.Entiteti.Zaposleni", "ObradioZaposleni")
                        .WithMany()
                        .HasForeignKey("ObradioZaposleniId")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.Navigation("Korisnik");

                    b.Navigation("ObradioZaposleni");
                });

            modelBuilder.Entity("Domain.Entiteti.Recenzija", b =>
                {
                    b.HasOne("Domain.Entiteti.Korisnik", "Korisnik")
                        .WithMany()
                        .HasForeignKey("KorisnikId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("Domain.Entiteti.StavkaMenija", "StavkaMenija")
                        .WithMany()
                        .HasForeignKey("StavkaMenijaId")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.Navigation("Korisnik");

                    b.Navigation("StavkaMenija");
                });

            modelBuilder.Entity("Domain.Entiteti.Receptura", b =>
                {
                    b.HasOne("Domain.Entiteti.Namirnica", "Namirnica")
                        .WithMany()
                        .HasForeignKey("NamirnicaId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("Domain.Entiteti.StavkaMenija", "StavkaMenija")
                        .WithMany("Receptura")
                        .HasForeignKey("StavkaMenijaId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Namirnica");

                    b.Navigation("StavkaMenija");
                });

            modelBuilder.Entity("Domain.Entiteti.Rezervacija", b =>
                {
                    b.HasOne("Domain.Entiteti.Korisnik", "Korisnik")
                        .WithMany()
                        .HasForeignKey("KorisnikId")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.HasOne("Domain.Entiteti.Zaposleni", "KreiraoZaposleni")
                        .WithMany()
                        .HasForeignKey("KreiraoZaposleniId")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.HasOne("Domain.Entiteti.Sto", "Sto")
                        .WithMany()
                        .HasForeignKey("StoId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("Korisnik");

                    b.Navigation("KreiraoZaposleni");

                    b.Navigation("Sto");
                });

            modelBuilder.Entity("Domain.Entiteti.Smena", b =>
                {
                    b.HasOne("Domain.Entiteti.Zaposleni", "Zaposleni")
                        .WithMany()
                        .HasForeignKey("ZaposleniId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("Zaposleni");
                });

            modelBuilder.Entity("Domain.Entiteti.StavkaMenija", b =>
                {
                    b.HasOne("Domain.Entiteti.KategorijaMenija", "Kategorija")
                        .WithMany("Stavke")
                        .HasForeignKey("KategorijaId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("Kategorija");
                });

            modelBuilder.Entity("Domain.Entiteti.StavkaPorudzbine", b =>
                {
                    b.HasOne("Domain.Entiteti.Porudzbina", "Porudzbina")
                        .WithMany("Stavke")
                        .HasForeignKey("PorudzbinaId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Domain.Entiteti.Zaposleni", "PripremioZaposleni")
                        .WithMany()
                        .HasForeignKey("PripremioZaposleniId")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.HasOne("Domain.Entiteti.StavkaMenija", "StavkaMenija")
                        .WithMany()
                        .HasForeignKey("StavkaMenijaId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("Porudzbina");

                    b.Navigation("PripremioZaposleni");

                    b.Navigation("StavkaMenija");
                });

            modelBuilder.Entity("Domain.Entiteti.Zaposleni", b =>
                {
                    b.HasOne("Domain.Entiteti.Korisnik", "Korisnik")
                        .WithOne("Zaposleni")
                        .HasForeignKey("Domain.Entiteti.Zaposleni", "KorisnikId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Korisnik");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<string>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityRole", null)
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<string>", b =>
                {
                    b.HasOne("Domain.Entiteti.Korisnik", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<string>", b =>
                {
                    b.HasOne("Domain.Entiteti.Korisnik", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserRole<string>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityRole", null)
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Domain.Entiteti.Korisnik", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<string>", b =>
                {
                    b.HasOne("Domain.Entiteti.Korisnik", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Domain.Entiteti.KategorijaMenija", b =>
                {
                    b.Navigation("Stavke");
                });

            modelBuilder.Entity("Domain.Entiteti.Korisnik", b =>
                {
                    b.Navigation("Zaposleni");
                });

            modelBuilder.Entity("Domain.Entiteti.Porudzbina", b =>
                {
                    b.Navigation("Stavke");
                });

            modelBuilder.Entity("Domain.Entiteti.StavkaMenija", b =>
                {
                    b.Navigation("Receptura");
                });

            modelBuilder.Entity("Domain.Entiteti.Sto", b =>
                {
                    b.Navigation("Porudzbine");
                });
#pragma warning restore 612, 618
        }
    }
}
