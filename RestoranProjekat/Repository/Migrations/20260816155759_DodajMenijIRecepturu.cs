using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Repository.Migrations
{
    public partial class DodajMenijIRecepturu : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "KategorijeMenija",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Naziv = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Opis = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Redosled = table.Column<int>(type: "int", nullable: false),
                    Odrediste = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KategorijeMenija", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Namirnice",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Naziv = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    JedinicaMere = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    TrenutnaKolicina = table.Column<decimal>(type: "decimal(10,3)", nullable: false),
                    MinimalniPrag = table.Column<decimal>(type: "decimal(10,3)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Namirnice", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "StavkeMenija",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Naziv = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Opis = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false),
                    Cena = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    SlikaUrl = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    KategorijaId = table.Column<int>(type: "int", nullable: false),
                    Dostupno = table.Column<bool>(type: "bit", nullable: false),
                    Popust = table.Column<decimal>(type: "decimal(5,2)", nullable: true),
                    DatumKreiranja = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StavkeMenija", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StavkeMenija_KategorijeMenija_KategorijaId",
                        column: x => x.KategorijaId,
                        principalTable: "KategorijeMenija",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Recepture",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StavkaMenijaId = table.Column<int>(type: "int", nullable: false),
                    NamirnicaId = table.Column<int>(type: "int", nullable: false),
                    Kolicina = table.Column<decimal>(type: "decimal(10,3)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Recepture", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Recepture_Namirnice_NamirnicaId",
                        column: x => x.NamirnicaId,
                        principalTable: "Namirnice",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Recepture_StavkeMenija_StavkaMenijaId",
                        column: x => x.StavkaMenijaId,
                        principalTable: "StavkeMenija",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Recepture_NamirnicaId",
                table: "Recepture",
                column: "NamirnicaId");

            migrationBuilder.CreateIndex(
                name: "IX_Recepture_StavkaMenijaId_NamirnicaId",
                table: "Recepture",
                columns: new[] { "StavkaMenijaId", "NamirnicaId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_StavkeMenija_KategorijaId",
                table: "StavkeMenija",
                column: "KategorijaId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Recepture");

            migrationBuilder.DropTable(
                name: "Namirnice");

            migrationBuilder.DropTable(
                name: "StavkeMenija");

            migrationBuilder.DropTable(
                name: "KategorijeMenija");
        }
    }
}
