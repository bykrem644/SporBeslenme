using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SporBeslenmeWeb.Migrations
{
    /// <inheritdoc />
    public partial class OlculerVeAvatarEklendi : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "VucutOlculeri",
                table: "KullaniciDetaylari",
                newName: "ProfilFotoUrl");

            migrationBuilder.AddColumn<double>(
                name: "Bel",
                table: "KullaniciDetaylari",
                type: "float",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "Gogus",
                table: "KullaniciDetaylari",
                type: "float",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "Kol",
                table: "KullaniciDetaylari",
                type: "float",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "Omuz",
                table: "KullaniciDetaylari",
                type: "float",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Bel",
                table: "KullaniciDetaylari");

            migrationBuilder.DropColumn(
                name: "Gogus",
                table: "KullaniciDetaylari");

            migrationBuilder.DropColumn(
                name: "Kol",
                table: "KullaniciDetaylari");

            migrationBuilder.DropColumn(
                name: "Omuz",
                table: "KullaniciDetaylari");

            migrationBuilder.RenameColumn(
                name: "ProfilFotoUrl",
                table: "KullaniciDetaylari",
                newName: "VucutOlculeri");
        }
    }
}
