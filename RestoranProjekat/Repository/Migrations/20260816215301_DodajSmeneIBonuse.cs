using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Repository.Migrations
{
    public partial class DodajSmeneIBonuse : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Bonusi",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ZaposleniId = table.Column<int>(type: "int", nullable: false),
                    Iznos = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    DatumPocetka = table.Column<DateOnly>(type: "date", nullable: false),
                    DatumKraja = table.Column<DateOnly>(type: "date", nullable: false),
                    Razlog = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    DodelioZaposleniId = table.Column<int>(type: "int", nullable: false),
                    DatumDodele = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Bonusi", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Bonusi_Zaposleni_DodelioZaposleniId",
                        column: x => x.DodelioZaposleniId,
                        principalTable: "Zaposleni",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Bonusi_Zaposleni_ZaposleniId",
                        column: x => x.ZaposleniId,
                        principalTable: "Zaposleni",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Smene",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ZaposleniId = table.Column<int>(type: "int", nullable: false),
                    Datum = table.Column<DateOnly>(type: "date", nullable: false),
                    VremePocetka = table.Column<TimeOnly>(type: "time", nullable: false),
                    VremeKraja = table.Column<TimeOnly>(type: "time", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Smene", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Smene_Zaposleni_ZaposleniId",
                        column: x => x.ZaposleniId,
                        principalTable: "Zaposleni",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Bonusi_DodelioZaposleniId",
                table: "Bonusi",
                column: "DodelioZaposleniId");

            migrationBuilder.CreateIndex(
                name: "IX_Bonusi_ZaposleniId_DatumPocetka",
                table: "Bonusi",
                columns: new[] { "ZaposleniId", "DatumPocetka" });

            migrationBuilder.CreateIndex(
                name: "IX_Smene_ZaposleniId_Datum",
                table: "Smene",
                columns: new[] { "ZaposleniId", "Datum" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Bonusi");

            migrationBuilder.DropTable(
                name: "Smene");
        }
    }
}
