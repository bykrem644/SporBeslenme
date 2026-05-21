using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SporBeslenmeWeb.Migrations
{
    /// <inheritdoc />
    public partial class KullaniciDetayTablosuEklendi : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "KullaniciDetaylari",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Yas = table.Column<int>(type: "int", nullable: false),
                    Boy = table.Column<double>(type: "float", nullable: false),
                    Kilo = table.Column<double>(type: "float", nullable: false),
                    VucutOlculeri = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Hastaliklar = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Hedef = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    OlusturulmaTarihi = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KullaniciDetaylari", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "KullaniciDetaylari");
        }
    }
}
