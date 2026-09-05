using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Repository.Migrations
{
    public partial class SpajanjeStolovaURezervaciji : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Rezervacije_KodRezervacije",
                table: "Rezervacije");

            migrationBuilder.CreateIndex(
                name: "IX_Rezervacije_KodRezervacije_StoId",
                table: "Rezervacije",
                columns: new[] { "KodRezervacije", "StoId" },
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Rezervacije_KodRezervacije_StoId",
                table: "Rezervacije");

            migrationBuilder.CreateIndex(
                name: "IX_Rezervacije_KodRezervacije",
                table: "Rezervacije",
                column: "KodRezervacije",
                unique: true);
        }
    }
}
