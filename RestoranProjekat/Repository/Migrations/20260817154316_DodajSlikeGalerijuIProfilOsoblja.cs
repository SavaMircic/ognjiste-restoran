using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Repository.Migrations
{
    public partial class DodajSlikeGalerijuIProfilOsoblja : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Zaposleni_Korisnici_KorisnikId",
                table: "Zaposleni");

            migrationBuilder.AddColumn<string>(
                name: "Biografija",
                table: "Zaposleni",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "InstagramUrl",
                table: "Zaposleni",
                type: "nvarchar(300)",
                maxLength: 300,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LinkedInUrl",
                table: "Zaposleni",
                type: "nvarchar(300)",
                maxLength: 300,
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "PrikaziNaSajtu",
                table: "Zaposleni",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "Redosled",
                table: "Zaposleni",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "SlikaUrl",
                table: "Zaposleni",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FacebookUrl",
                table: "PostavkeRestorana",
                type: "nvarchar(300)",
                maxLength: 300,
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "GeoDuzina",
                table: "PostavkeRestorana",
                type: "decimal(9,6)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "GeoSirina",
                table: "PostavkeRestorana",
                type: "decimal(9,6)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "InstagramUrl",
                table: "PostavkeRestorana",
                type: "nvarchar(300)",
                maxLength: 300,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SlikaUrl",
                table: "Korisnici",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.CreateTable(
                name: "GalerijaSlike",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SlikaUrl = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Naslov = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Opis = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Redosled = table.Column<int>(type: "int", nullable: false),
                    Aktivan = table.Column<bool>(type: "bit", nullable: false),
                    DatumDodavanja = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GalerijaSlike", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Zaposleni_PrikaziNaSajtu_Redosled",
                table: "Zaposleni",
                columns: new[] { "PrikaziNaSajtu", "Redosled" });

            migrationBuilder.CreateIndex(
                name: "IX_GalerijaSlike_Aktivan_Redosled",
                table: "GalerijaSlike",
                columns: new[] { "Aktivan", "Redosled" });

            migrationBuilder.AddForeignKey(
                name: "FK_Zaposleni_Korisnici_KorisnikId",
                table: "Zaposleni",
                column: "KorisnikId",
                principalTable: "Korisnici",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Zaposleni_Korisnici_KorisnikId",
                table: "Zaposleni");

            migrationBuilder.DropTable(
                name: "GalerijaSlike");

            migrationBuilder.DropIndex(
                name: "IX_Zaposleni_PrikaziNaSajtu_Redosled",
                table: "Zaposleni");

            migrationBuilder.DropColumn(
                name: "Biografija",
                table: "Zaposleni");

            migrationBuilder.DropColumn(
                name: "InstagramUrl",
                table: "Zaposleni");

            migrationBuilder.DropColumn(
                name: "LinkedInUrl",
                table: "Zaposleni");

            migrationBuilder.DropColumn(
                name: "PrikaziNaSajtu",
                table: "Zaposleni");

            migrationBuilder.DropColumn(
                name: "Redosled",
                table: "Zaposleni");

            migrationBuilder.DropColumn(
                name: "SlikaUrl",
                table: "Zaposleni");

            migrationBuilder.DropColumn(
                name: "FacebookUrl",
                table: "PostavkeRestorana");

            migrationBuilder.DropColumn(
                name: "GeoDuzina",
                table: "PostavkeRestorana");

            migrationBuilder.DropColumn(
                name: "GeoSirina",
                table: "PostavkeRestorana");

            migrationBuilder.DropColumn(
                name: "InstagramUrl",
                table: "PostavkeRestorana");

            migrationBuilder.DropColumn(
                name: "SlikaUrl",
                table: "Korisnici");

            migrationBuilder.AddForeignKey(
                name: "FK_Zaposleni_Korisnici_KorisnikId",
                table: "Zaposleni",
                column: "KorisnikId",
                principalTable: "Korisnici",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
