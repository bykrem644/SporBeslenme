using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SporBeslenmeWeb.Migrations
{
    /// <inheritdoc />
    public partial class PdfEklendi : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PdfDosyaUrl",
                table: "Makaleler",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PdfDosyaUrl",
                table: "Makaleler");
        }
    }
}
