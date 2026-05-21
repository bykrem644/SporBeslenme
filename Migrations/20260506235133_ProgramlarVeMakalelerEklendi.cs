using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SporBeslenmeWeb.Migrations
{
    /// <inheritdoc />
    public partial class ProgramlarVeMakalelerEklendi : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AntrenmanProgramlari",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Baslik = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Aciklama = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    GunSayisi = table.Column<int>(type: "int", nullable: false),
                    Seviye = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AntrenmanProgramlari", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Makaleler",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Baslik = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Ozet = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Icerik = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Yazar = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    YayinTarihi = table.Column<DateTime>(type: "datetime2", nullable: false),
                    KapakGorseliUrl = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Makaleler", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ProgramVideolari",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AntrenmanProgramiId = table.Column<int>(type: "int", nullable: false),
                    VideoBaslik = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Aciklama = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    VideoYolu = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Sira = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProgramVideolari", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProgramVideolari_AntrenmanProgramlari_AntrenmanProgramiId",
                        column: x => x.AntrenmanProgramiId,
                        principalTable: "AntrenmanProgramlari",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ProgramVideolari_AntrenmanProgramiId",
                table: "ProgramVideolari",
                column: "AntrenmanProgramiId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Makaleler");

            migrationBuilder.DropTable(
                name: "ProgramVideolari");

            migrationBuilder.DropTable(
                name: "AntrenmanProgramlari");
        }
    }
}
