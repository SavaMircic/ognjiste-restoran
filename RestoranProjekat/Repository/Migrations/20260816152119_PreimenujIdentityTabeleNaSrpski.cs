using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Repository.Migrations
{
    public partial class PreimenujIdentityTabeleNaSrpski : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetRoleClaims_AspNetRoles_RoleId",
                table: "AspNetRoleClaims");

            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUserClaims_AspNetUsers_UserId",
                table: "AspNetUserClaims");

            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUserLogins_AspNetUsers_UserId",
                table: "AspNetUserLogins");

            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUserRoles_AspNetRoles_RoleId",
                table: "AspNetUserRoles");

            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUserRoles_AspNetUsers_UserId",
                table: "AspNetUserRoles");

            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUserTokens_AspNetUsers_UserId",
                table: "AspNetUserTokens");

            migrationBuilder.DropForeignKey(
                name: "FK_AuditLogovi_AspNetUsers_KorisnikId",
                table: "AuditLogovi");

            migrationBuilder.DropForeignKey(
                name: "FK_Zaposleni_AspNetUsers_KorisnikId",
                table: "Zaposleni");

            migrationBuilder.DropPrimaryKey(
                name: "PK_AspNetUserTokens",
                table: "AspNetUserTokens");

            migrationBuilder.DropPrimaryKey(
                name: "PK_AspNetUsers",
                table: "AspNetUsers");

            migrationBuilder.DropPrimaryKey(
                name: "PK_AspNetUserRoles",
                table: "AspNetUserRoles");

            migrationBuilder.DropPrimaryKey(
                name: "PK_AspNetUserLogins",
                table: "AspNetUserLogins");

            migrationBuilder.DropPrimaryKey(
                name: "PK_AspNetUserClaims",
                table: "AspNetUserClaims");

            migrationBuilder.DropPrimaryKey(
                name: "PK_AspNetRoles",
                table: "AspNetRoles");

            migrationBuilder.DropPrimaryKey(
                name: "PK_AspNetRoleClaims",
                table: "AspNetRoleClaims");

            migrationBuilder.RenameTable(
                name: "AspNetUserTokens",
                newName: "KorisnikTokeni");

            migrationBuilder.RenameTable(
                name: "AspNetUsers",
                newName: "Korisnici");

            migrationBuilder.RenameTable(
                name: "AspNetUserRoles",
                newName: "KorisnikUloge");

            migrationBuilder.RenameTable(
                name: "AspNetUserLogins",
                newName: "KorisnickePrijave");

            migrationBuilder.RenameTable(
                name: "AspNetUserClaims",
                newName: "KorisnikTvrdnje");

            migrationBuilder.RenameTable(
                name: "AspNetRoles",
                newName: "Uloge");

            migrationBuilder.RenameTable(
                name: "AspNetRoleClaims",
                newName: "UlogaTvrdnje");

            migrationBuilder.RenameIndex(
                name: "IX_AspNetUserRoles_RoleId",
                table: "KorisnikUloge",
                newName: "IX_KorisnikUloge_RoleId");

            migrationBuilder.RenameIndex(
                name: "IX_AspNetUserLogins_UserId",
                table: "KorisnickePrijave",
                newName: "IX_KorisnickePrijave_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_AspNetUserClaims_UserId",
                table: "KorisnikTvrdnje",
                newName: "IX_KorisnikTvrdnje_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_AspNetRoleClaims_RoleId",
                table: "UlogaTvrdnje",
                newName: "IX_UlogaTvrdnje_RoleId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_KorisnikTokeni",
                table: "KorisnikTokeni",
                columns: new[] { "UserId", "LoginProvider", "Name" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_Korisnici",
                table: "Korisnici",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_KorisnikUloge",
                table: "KorisnikUloge",
                columns: new[] { "UserId", "RoleId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_KorisnickePrijave",
                table: "KorisnickePrijave",
                columns: new[] { "LoginProvider", "ProviderKey" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_KorisnikTvrdnje",
                table: "KorisnikTvrdnje",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Uloge",
                table: "Uloge",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UlogaTvrdnje",
                table: "UlogaTvrdnje",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_AuditLogovi_Korisnici_KorisnikId",
                table: "AuditLogovi",
                column: "KorisnikId",
                principalTable: "Korisnici",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_KorisnickePrijave_Korisnici_UserId",
                table: "KorisnickePrijave",
                column: "UserId",
                principalTable: "Korisnici",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_KorisnikTokeni_Korisnici_UserId",
                table: "KorisnikTokeni",
                column: "UserId",
                principalTable: "Korisnici",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_KorisnikTvrdnje_Korisnici_UserId",
                table: "KorisnikTvrdnje",
                column: "UserId",
                principalTable: "Korisnici",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_KorisnikUloge_Korisnici_UserId",
                table: "KorisnikUloge",
                column: "UserId",
                principalTable: "Korisnici",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_KorisnikUloge_Uloge_RoleId",
                table: "KorisnikUloge",
                column: "RoleId",
                principalTable: "Uloge",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UlogaTvrdnje_Uloge_RoleId",
                table: "UlogaTvrdnje",
                column: "RoleId",
                principalTable: "Uloge",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Zaposleni_Korisnici_KorisnikId",
                table: "Zaposleni",
                column: "KorisnikId",
                principalTable: "Korisnici",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AuditLogovi_Korisnici_KorisnikId",
                table: "AuditLogovi");

            migrationBuilder.DropForeignKey(
                name: "FK_KorisnickePrijave_Korisnici_UserId",
                table: "KorisnickePrijave");

            migrationBuilder.DropForeignKey(
                name: "FK_KorisnikTokeni_Korisnici_UserId",
                table: "KorisnikTokeni");

            migrationBuilder.DropForeignKey(
                name: "FK_KorisnikTvrdnje_Korisnici_UserId",
                table: "KorisnikTvrdnje");

            migrationBuilder.DropForeignKey(
                name: "FK_KorisnikUloge_Korisnici_UserId",
                table: "KorisnikUloge");

            migrationBuilder.DropForeignKey(
                name: "FK_KorisnikUloge_Uloge_RoleId",
                table: "KorisnikUloge");

            migrationBuilder.DropForeignKey(
                name: "FK_UlogaTvrdnje_Uloge_RoleId",
                table: "UlogaTvrdnje");

            migrationBuilder.DropForeignKey(
                name: "FK_Zaposleni_Korisnici_KorisnikId",
                table: "Zaposleni");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Uloge",
                table: "Uloge");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UlogaTvrdnje",
                table: "UlogaTvrdnje");

            migrationBuilder.DropPrimaryKey(
                name: "PK_KorisnikUloge",
                table: "KorisnikUloge");

            migrationBuilder.DropPrimaryKey(
                name: "PK_KorisnikTvrdnje",
                table: "KorisnikTvrdnje");

            migrationBuilder.DropPrimaryKey(
                name: "PK_KorisnikTokeni",
                table: "KorisnikTokeni");

            migrationBuilder.DropPrimaryKey(
                name: "PK_KorisnickePrijave",
                table: "KorisnickePrijave");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Korisnici",
                table: "Korisnici");

            migrationBuilder.RenameTable(
                name: "Uloge",
                newName: "AspNetRoles");

            migrationBuilder.RenameTable(
                name: "UlogaTvrdnje",
                newName: "AspNetRoleClaims");

            migrationBuilder.RenameTable(
                name: "KorisnikUloge",
                newName: "AspNetUserRoles");

            migrationBuilder.RenameTable(
                name: "KorisnikTvrdnje",
                newName: "AspNetUserClaims");

            migrationBuilder.RenameTable(
                name: "KorisnikTokeni",
                newName: "AspNetUserTokens");

            migrationBuilder.RenameTable(
                name: "KorisnickePrijave",
                newName: "AspNetUserLogins");

            migrationBuilder.RenameTable(
                name: "Korisnici",
                newName: "AspNetUsers");

            migrationBuilder.RenameIndex(
                name: "IX_UlogaTvrdnje_RoleId",
                table: "AspNetRoleClaims",
                newName: "IX_AspNetRoleClaims_RoleId");

            migrationBuilder.RenameIndex(
                name: "IX_KorisnikUloge_RoleId",
                table: "AspNetUserRoles",
                newName: "IX_AspNetUserRoles_RoleId");

            migrationBuilder.RenameIndex(
                name: "IX_KorisnikTvrdnje_UserId",
                table: "AspNetUserClaims",
                newName: "IX_AspNetUserClaims_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_KorisnickePrijave_UserId",
                table: "AspNetUserLogins",
                newName: "IX_AspNetUserLogins_UserId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_AspNetRoles",
                table: "AspNetRoles",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_AspNetRoleClaims",
                table: "AspNetRoleClaims",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_AspNetUserRoles",
                table: "AspNetUserRoles",
                columns: new[] { "UserId", "RoleId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_AspNetUserClaims",
                table: "AspNetUserClaims",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_AspNetUserTokens",
                table: "AspNetUserTokens",
                columns: new[] { "UserId", "LoginProvider", "Name" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_AspNetUserLogins",
                table: "AspNetUserLogins",
                columns: new[] { "LoginProvider", "ProviderKey" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_AspNetUsers",
                table: "AspNetUsers",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetRoleClaims_AspNetRoles_RoleId",
                table: "AspNetRoleClaims",
                column: "RoleId",
                principalTable: "AspNetRoles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUserClaims_AspNetUsers_UserId",
                table: "AspNetUserClaims",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUserLogins_AspNetUsers_UserId",
                table: "AspNetUserLogins",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUserRoles_AspNetRoles_RoleId",
                table: "AspNetUserRoles",
                column: "RoleId",
                principalTable: "AspNetRoles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUserRoles_AspNetUsers_UserId",
                table: "AspNetUserRoles",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUserTokens_AspNetUsers_UserId",
                table: "AspNetUserTokens",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AuditLogovi_AspNetUsers_KorisnikId",
                table: "AuditLogovi",
                column: "KorisnikId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Zaposleni_AspNetUsers_KorisnikId",
                table: "Zaposleni",
                column: "KorisnikId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
