using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SporBeslenmeWeb.Migrations
{
    /// <inheritdoc />
    public partial class GelismisSistemAltyapisi : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AnaMalzemeler",
                table: "Tarifler",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "IcerdigiAlerjenler",
                table: "Tarifler",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AntrenmanSeviyesi",
                table: "KullaniciDetaylari",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "BeslenmeKisitlamalari",
                table: "KullaniciDetaylari",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OrtopedikRahatsizliklar",
                table: "KullaniciDetaylari",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SevmedigiBesinler",
                table: "KullaniciDetaylari",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "KaloriGecmisleri",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    GunlukIhtiyac = table.Column<double>(type: "float", nullable: false),
                    Hedef = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Tarih = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KaloriGecmisleri", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "VkiGecmisleri",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Boy = table.Column<double>(type: "float", nullable: false),
                    Kilo = table.Column<double>(type: "float", nullable: false),
                    VkiSonucu = table.Column<double>(type: "float", nullable: false),
                    Durum = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Tarih = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VkiGecmisleri", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "KaloriGecmisleri");

            migrationBuilder.DropTable(
                name: "VkiGecmisleri");

            migrationBuilder.DropColumn(
                name: "AnaMalzemeler",
                table: "Tarifler");

            migrationBuilder.DropColumn(
                name: "IcerdigiAlerjenler",
                table: "Tarifler");

            migrationBuilder.DropColumn(
                name: "AntrenmanSeviyesi",
                table: "KullaniciDetaylari");

            migrationBuilder.DropColumn(
                name: "BeslenmeKisitlamalari",
                table: "KullaniciDetaylari");

            migrationBuilder.DropColumn(
                name: "OrtopedikRahatsizliklar",
                table: "KullaniciDetaylari");

            migrationBuilder.DropColumn(
                name: "SevmedigiBesinler",
                table: "KullaniciDetaylari");
        }
    }
}
