using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Repository.Migrations
{
    public partial class DodajRecenzijeILajkove : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "LajkoviJela",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    KorisnikId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    StavkaMenijaId = table.Column<int>(type: "int", nullable: false),
                    DatumKreiranja = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LajkoviJela", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LajkoviJela_Korisnici_KorisnikId",
                        column: x => x.KorisnikId,
                        principalTable: "Korisnici",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_LajkoviJela_StavkeMenija_StavkaMenijaId",
                        column: x => x.StavkaMenijaId,
                        principalTable: "StavkeMenija",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Recenzije",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    KorisnikId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    TipRecenzije = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    StavkaMenijaId = table.Column<int>(type: "int", nullable: true),
                    Ocena = table.Column<int>(type: "int", nullable: false),
                    Naslov = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    Tekst = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false),
                    DatumKreiranja = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DatumIzmene = table.Column<DateTime>(type: "datetime2", nullable: true),
                    OdgovorRestorana = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    DatumOdgovora = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Recenzije", x => x.Id);
                    table.CheckConstraint("CK_Recenzije_Ocena", "[Ocena] >= 1 AND [Ocena] <= 5");
                    table.ForeignKey(
                        name: "FK_Recenzije_Korisnici_KorisnikId",
                        column: x => x.KorisnikId,
                        principalTable: "Korisnici",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Recenzije_StavkeMenija_StavkaMenijaId",
                        column: x => x.StavkaMenijaId,
                        principalTable: "StavkeMenija",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_LajkoviJela_KorisnikId_StavkaMenijaId",
                table: "LajkoviJela",
                columns: new[] { "KorisnikId", "StavkaMenijaId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_LajkoviJela_StavkaMenijaId",
                table: "LajkoviJela",
                column: "StavkaMenijaId");

            migrationBuilder.CreateIndex(
                name: "IX_Recenzije_KorisnikId_TipRecenzije",
                table: "Recenzije",
                columns: new[] { "KorisnikId", "TipRecenzije" },
                unique: true,
                filter: "[StavkaMenijaId] IS NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Recenzije_KorisnikId_TipRecenzije_StavkaMenijaId",
                table: "Recenzije",
                columns: new[] { "KorisnikId", "TipRecenzije", "StavkaMenijaId" },
                unique: true,
                filter: "[StavkaMenijaId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Recenzije_StavkaMenijaId",
                table: "Recenzije",
                column: "StavkaMenijaId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "LajkoviJela");

            migrationBuilder.DropTable(
                name: "Recenzije");
        }
    }
}
