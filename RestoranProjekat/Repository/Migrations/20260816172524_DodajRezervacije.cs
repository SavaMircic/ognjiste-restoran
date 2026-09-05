using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Repository.Migrations
{
    public partial class DodajRezervacije : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Rezervacije",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    KorisnikId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    GostIme = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    GostEmail = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    GostTelefon = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    StoId = table.Column<int>(type: "int", nullable: false),
                    DatumVreme = table.Column<DateTime>(type: "datetime2", nullable: false),
                    BrojGostiju = table.Column<int>(type: "int", nullable: false),
                    KodRezervacije = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    NacinKreiranja = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    KreiraoZaposleniId = table.Column<int>(type: "int", nullable: true),
                    DatumKreiranja = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Rezervacije", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Rezervacije_Korisnici_KorisnikId",
                        column: x => x.KorisnikId,
                        principalTable: "Korisnici",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Rezervacije_Stolovi_StoId",
                        column: x => x.StoId,
                        principalTable: "Stolovi",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Rezervacije_Zaposleni_KreiraoZaposleniId",
                        column: x => x.KreiraoZaposleniId,
                        principalTable: "Zaposleni",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Porudzbine_RezervacijaId",
                table: "Porudzbine",
                column: "RezervacijaId");

            migrationBuilder.CreateIndex(
                name: "IX_Rezervacije_DatumVreme",
                table: "Rezervacije",
                column: "DatumVreme");

            migrationBuilder.CreateIndex(
                name: "IX_Rezervacije_KodRezervacije",
                table: "Rezervacije",
                column: "KodRezervacije",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Rezervacije_KorisnikId",
                table: "Rezervacije",
                column: "KorisnikId");

            migrationBuilder.CreateIndex(
                name: "IX_Rezervacije_KreiraoZaposleniId",
                table: "Rezervacije",
                column: "KreiraoZaposleniId");

            migrationBuilder.CreateIndex(
                name: "IX_Rezervacije_StoId",
                table: "Rezervacije",
                column: "StoId");

            migrationBuilder.AddForeignKey(
                name: "FK_Porudzbine_Rezervacije_RezervacijaId",
                table: "Porudzbine",
                column: "RezervacijaId",
                principalTable: "Rezervacije",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Porudzbine_Rezervacije_RezervacijaId",
                table: "Porudzbine");

            migrationBuilder.DropTable(
                name: "Rezervacije");

            migrationBuilder.DropIndex(
                name: "IX_Porudzbine_RezervacijaId",
                table: "Porudzbine");
        }
    }
}
