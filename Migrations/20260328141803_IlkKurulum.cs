using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SporBeslenmeWeb.Migrations
{
    /// <inheritdoc />
    public partial class IlkKurulum : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Egzersizler",
                columns: table => new
                {
                    EgzersizID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    KasGrupID = table.Column<int>(type: "int", nullable: false),
                    Ad = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NasilYapilir = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ZorlukSeviyesi = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    GorselYolu = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    VideoYolu = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Egzersizler", x => x.EgzersizID);
                });

            migrationBuilder.CreateTable(
                name: "KasGruplari",
                columns: table => new
                {
                    KasGrupID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Ad = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Aciklama = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KasGruplari", x => x.KasGrupID);
                });

            migrationBuilder.CreateTable(
                name: "Tarifler",
                columns: table => new
                {
                    TarifID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Ad = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Kategori = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Malzemeler = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Hazirlanis = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Kalori = table.Column<int>(type: "int", nullable: true),
                    KarbonhidratYuzdesi = table.Column<int>(type: "int", nullable: true),
                    ProteinYuzdesi = table.Column<int>(type: "int", nullable: true),
                    YagYuzdesi = table.Column<int>(type: "int", nullable: true),
                    GorselYolu = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tarifler", x => x.TarifID);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Egzersizler");

            migrationBuilder.DropTable(
                name: "KasGruplari");

            migrationBuilder.DropTable(
                name: "Tarifler");
        }
    }
}
