using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Repository.Migrations
{
    public partial class DodajAdminAkcijeIPrijaveProblema : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Aktivan",
                table: "Recenzije",
                type: "bit",
                nullable: false,
                defaultValue: true);

            migrationBuilder.CreateTable(
                name: "AdminAkcije",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AdministratorId = table.Column<int>(type: "int", nullable: false),
                    TipAkcije = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    CiljniKorisnikId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    Opis = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Datum = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AdminAkcije", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AdminAkcije_Korisnici_CiljniKorisnikId",
                        column: x => x.CiljniKorisnikId,
                        principalTable: "Korisnici",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_AdminAkcije_Zaposleni_AdministratorId",
                        column: x => x.AdministratorId,
                        principalTable: "Zaposleni",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PrijaveProblema",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PrijavioZaposleniId = table.Column<int>(type: "int", nullable: false),
                    Naslov = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Opis = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false),
                    Kategorija = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Prioritet = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    DatumPrijave = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ResioZaposleniId = table.Column<int>(type: "int", nullable: true),
                    Odgovor = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    DatumResavanja = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PrijaveProblema", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PrijaveProblema_Zaposleni_PrijavioZaposleniId",
                        column: x => x.PrijavioZaposleniId,
                        principalTable: "Zaposleni",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PrijaveProblema_Zaposleni_ResioZaposleniId",
                        column: x => x.ResioZaposleniId,
                        principalTable: "Zaposleni",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AdminAkcije_AdministratorId",
                table: "AdminAkcije",
                column: "AdministratorId");

            migrationBuilder.CreateIndex(
                name: "IX_AdminAkcije_CiljniKorisnikId",
                table: "AdminAkcije",
                column: "CiljniKorisnikId");

            migrationBuilder.CreateIndex(
                name: "IX_AdminAkcije_Datum",
                table: "AdminAkcije",
                column: "Datum");

            migrationBuilder.CreateIndex(
                name: "IX_AdminAkcije_TipAkcije",
                table: "AdminAkcije",
                column: "TipAkcije");

            migrationBuilder.CreateIndex(
                name: "IX_PrijaveProblema_PrijavioZaposleniId",
                table: "PrijaveProblema",
                column: "PrijavioZaposleniId");

            migrationBuilder.CreateIndex(
                name: "IX_PrijaveProblema_ResioZaposleniId",
                table: "PrijaveProblema",
                column: "ResioZaposleniId");

            migrationBuilder.CreateIndex(
                name: "IX_PrijaveProblema_Status_DatumPrijave",
                table: "PrijaveProblema",
                columns: new[] { "Status", "DatumPrijave" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AdminAkcije");

            migrationBuilder.DropTable(
                name: "PrijaveProblema");

            migrationBuilder.DropColumn(
                name: "Aktivan",
                table: "Recenzije");
        }
    }
}
