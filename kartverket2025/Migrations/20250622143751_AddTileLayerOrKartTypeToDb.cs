using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace kartverket2025.Migrations
{
    /// <inheritdoc />
    public partial class AddTileLayerOrKartTypeToDb : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TileLayer",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    KartType = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TileLayer", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "TileLayer",
                columns: new[] { "Id", "KartType" },
                values: new object[,]
                {
                    { 1, "Topofarge" },
                    { 2, "Topogråtone" },
                    { 3, "Turkart" },
                    { 4, "Sjøkart" },
                    { 5, "Carto Light" },
                    { 6, "Carto Dark" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TileLayer");
        }
    }
}
