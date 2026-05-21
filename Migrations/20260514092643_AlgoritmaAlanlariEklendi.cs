using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SporBeslenmeWeb.Migrations
{
    /// <inheritdoc />
    public partial class AlgoritmaAlanlariEklendi : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "TarifKategorisi",
                table: "Tarifler",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "HedefKitle",
                table: "Egzersizler",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "RiskliDurumlar",
                table: "Egzersizler",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TarifKategorisi",
                table: "Tarifler");

            migrationBuilder.DropColumn(
                name: "HedefKitle",
                table: "Egzersizler");

            migrationBuilder.DropColumn(
                name: "RiskliDurumlar",
                table: "Egzersizler");
        }
    }
}
