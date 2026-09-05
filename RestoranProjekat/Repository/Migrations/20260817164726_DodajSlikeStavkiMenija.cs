using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Repository.Migrations
{
    public partial class DodajSlikeStavkiMenija : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SlikeStavkiMenija",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StavkaMenijaId = table.Column<int>(type: "int", nullable: false),
                    SlikaUrl = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Opis = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true),
                    Redosled = table.Column<int>(type: "int", nullable: false),
                    DatumDodavanja = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SlikeStavkiMenija", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SlikeStavkiMenija_StavkeMenija_StavkaMenijaId",
                        column: x => x.StavkaMenijaId,
                        principalTable: "StavkeMenija",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SlikeStavkiMenija_StavkaMenijaId_Redosled",
                table: "SlikeStavkiMenija",
                columns: new[] { "StavkaMenijaId", "Redosled" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SlikeStavkiMenija");
        }
    }
}
