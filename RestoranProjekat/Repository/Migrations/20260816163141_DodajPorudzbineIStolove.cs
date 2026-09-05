using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Repository.Migrations
{
    public partial class DodajPorudzbineIStolove : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Stolovi",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BrojStola = table.Column<int>(type: "int", nullable: false),
                    Kapacitet = table.Column<int>(type: "int", nullable: false),
                    TrenutniStatus = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Stolovi", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Porudzbine",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StoId = table.Column<int>(type: "int", nullable: false),
                    KonobarId = table.Column<int>(type: "int", nullable: false),
                    RezervacijaId = table.Column<int>(type: "int", nullable: true),
                    VremeOtvaranja = table.Column<DateTime>(type: "datetime2", nullable: false),
                    VremeZatvaranja = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    NacinPlacanja = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    IznosNapojnice = table.Column<decimal>(type: "decimal(10,2)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Porudzbine", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Porudzbine_Stolovi_StoId",
                        column: x => x.StoId,
                        principalTable: "Stolovi",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Porudzbine_Zaposleni_KonobarId",
                        column: x => x.KonobarId,
                        principalTable: "Zaposleni",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "StavkePorudzbine",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PorudzbinaId = table.Column<int>(type: "int", nullable: false),
                    StavkaMenijaId = table.Column<int>(type: "int", nullable: false),
                    Kolicina = table.Column<int>(type: "int", nullable: false),
                    Napomena = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CenaUTrenutkuNarudzbine = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    PripremioZaposleniId = table.Column<int>(type: "int", nullable: true),
                    VremeSlanja = table.Column<DateTime>(type: "datetime2", nullable: false),
                    VremePreuzimanja = table.Column<DateTime>(type: "datetime2", nullable: true),
                    VremeZavrsetka = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StavkePorudzbine", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StavkePorudzbine_Porudzbine_PorudzbinaId",
                        column: x => x.PorudzbinaId,
                        principalTable: "Porudzbine",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_StavkePorudzbine_StavkeMenija_StavkaMenijaId",
                        column: x => x.StavkaMenijaId,
                        principalTable: "StavkeMenija",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_StavkePorudzbine_Zaposleni_PripremioZaposleniId",
                        column: x => x.PripremioZaposleniId,
                        principalTable: "Zaposleni",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Porudzbine_KonobarId",
                table: "Porudzbine",
                column: "KonobarId");

            migrationBuilder.CreateIndex(
                name: "IX_Porudzbine_StoId",
                table: "Porudzbine",
                column: "StoId");

            migrationBuilder.CreateIndex(
                name: "IX_StavkePorudzbine_PorudzbinaId",
                table: "StavkePorudzbine",
                column: "PorudzbinaId");

            migrationBuilder.CreateIndex(
                name: "IX_StavkePorudzbine_PripremioZaposleniId",
                table: "StavkePorudzbine",
                column: "PripremioZaposleniId");

            migrationBuilder.CreateIndex(
                name: "IX_StavkePorudzbine_Status",
                table: "StavkePorudzbine",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_StavkePorudzbine_StavkaMenijaId",
                table: "StavkePorudzbine",
                column: "StavkaMenijaId");

            migrationBuilder.CreateIndex(
                name: "IX_StavkePorudzbine_VremeSlanja",
                table: "StavkePorudzbine",
                column: "VremeSlanja");

            migrationBuilder.CreateIndex(
                name: "IX_Stolovi_BrojStola",
                table: "Stolovi",
                column: "BrojStola",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "StavkePorudzbine");

            migrationBuilder.DropTable(
                name: "Porudzbine");

            migrationBuilder.DropTable(
                name: "Stolovi");
        }
    }
}
