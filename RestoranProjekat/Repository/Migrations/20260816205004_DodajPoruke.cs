using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Repository.Migrations
{
    public partial class DodajPoruke : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Poruke",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    KorisnikId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    GostIme = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    GostEmail = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    GostTelefon = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    Kategorija = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Tekst = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false),
                    DatumSlanja = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Odgovor = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    DatumOdgovora = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ObradioZaposleniId = table.Column<int>(type: "int", nullable: true),
                    ZeljeniDatumVreme = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ZeljeniBrojGostiju = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Poruke", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Poruke_Korisnici_KorisnikId",
                        column: x => x.KorisnikId,
                        principalTable: "Korisnici",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Poruke_Zaposleni_ObradioZaposleniId",
                        column: x => x.ObradioZaposleniId,
                        principalTable: "Zaposleni",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Poruke_DatumSlanja",
                table: "Poruke",
                column: "DatumSlanja");

            migrationBuilder.CreateIndex(
                name: "IX_Poruke_KorisnikId",
                table: "Poruke",
                column: "KorisnikId");

            migrationBuilder.CreateIndex(
                name: "IX_Poruke_ObradioZaposleniId",
                table: "Poruke",
                column: "ObradioZaposleniId");

            migrationBuilder.CreateIndex(
                name: "IX_Poruke_Status",
                table: "Poruke",
                column: "Status");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Poruke");
        }
    }
}
