using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Repository.Migrations
{
    public partial class DodajKorekcijeZaliha : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "KorekcijeZaliha",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NamirnicaId = table.Column<int>(type: "int", nullable: false),
                    StaraKolicina = table.Column<decimal>(type: "decimal(10,3)", nullable: false),
                    NovaKolicina = table.Column<decimal>(type: "decimal(10,3)", nullable: false),
                    Razlog = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    ZaposleniId = table.Column<int>(type: "int", nullable: false),
                    Datum = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KorekcijeZaliha", x => x.Id);
                    table.ForeignKey(
                        name: "FK_KorekcijeZaliha_Namirnice_NamirnicaId",
                        column: x => x.NamirnicaId,
                        principalTable: "Namirnice",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_KorekcijeZaliha_Zaposleni_ZaposleniId",
                        column: x => x.ZaposleniId,
                        principalTable: "Zaposleni",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Namirnice_Naziv",
                table: "Namirnice",
                column: "Naziv",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_KorekcijeZaliha_Datum",
                table: "KorekcijeZaliha",
                column: "Datum");

            migrationBuilder.CreateIndex(
                name: "IX_KorekcijeZaliha_NamirnicaId",
                table: "KorekcijeZaliha",
                column: "NamirnicaId");

            migrationBuilder.CreateIndex(
                name: "IX_KorekcijeZaliha_ZaposleniId",
                table: "KorekcijeZaliha",
                column: "ZaposleniId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "KorekcijeZaliha");

            migrationBuilder.DropIndex(
                name: "IX_Namirnice_Naziv",
                table: "Namirnice");
        }
    }
}
