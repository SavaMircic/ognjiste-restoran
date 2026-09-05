using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Repository.Migrations
{
    public partial class DodajDetaljanOpisIGrupuGalerije : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_GalerijaSlike_Aktivan_Redosled",
                table: "GalerijaSlike");

            migrationBuilder.AddColumn<string>(
                name: "DetaljanOpis",
                table: "StavkeMenija",
                type: "nvarchar(4000)",
                maxLength: 4000,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Grupa",
                table: "GalerijaSlike",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.Sql(@"
                UPDATE GalerijaSlike SET Grupa = N'Sala',     Redosled = 10 WHERE SlikaUrl = N'/slike/galerija/enterijer-sala.jpg';
                UPDATE GalerijaSlike SET Grupa = N'Sala',     Redosled = 11 WHERE SlikaUrl = N'/slike/galerija/detalj-sto.jpg';
                UPDATE GalerijaSlike SET Grupa = N'Sala',     Redosled = 19 WHERE SlikaUrl = N'/slike/galerija/stara-fasada.jpg';
                UPDATE GalerijaSlike SET Grupa = N'Bašta',    Redosled = 20 WHERE SlikaUrl = N'/slike/galerija/basta-leto.jpg';
                UPDATE GalerijaSlike SET Grupa = N'Šank',     Redosled = 30 WHERE SlikaUrl = N'/slike/galerija/sank.jpg';
                UPDATE GalerijaSlike SET Grupa = N'Kuhinja',  Redosled = 40 WHERE SlikaUrl = N'/slike/galerija/kuhinja.jpg';
                UPDATE GalerijaSlike SET Grupa = N'Roštilj',  Redosled = 50 WHERE SlikaUrl = N'/slike/galerija/rostilj.jpg';
                UPDATE GalerijaSlike SET Grupa = N'Proslave', Redosled = 60 WHERE SlikaUrl = N'/slike/galerija/proslava.jpg';

                UPDATE GalerijaSlike SET Grupa = N'Ostalo' WHERE Grupa = N'';
            ");

            migrationBuilder.CreateIndex(
                name: "IX_GalerijaSlike_Aktivan_Grupa_Redosled",
                table: "GalerijaSlike",
                columns: new[] { "Aktivan", "Grupa", "Redosled" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_GalerijaSlike_Aktivan_Grupa_Redosled",
                table: "GalerijaSlike");

            migrationBuilder.DropColumn(
                name: "DetaljanOpis",
                table: "StavkeMenija");

            migrationBuilder.DropColumn(
                name: "Grupa",
                table: "GalerijaSlike");

            migrationBuilder.CreateIndex(
                name: "IX_GalerijaSlike_Aktivan_Redosled",
                table: "GalerijaSlike",
                columns: new[] { "Aktivan", "Redosled" });
        }
    }
}
