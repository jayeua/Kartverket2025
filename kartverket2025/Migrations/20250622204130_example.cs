using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace kartverket2025.Migrations
{
    /// <inheritdoc />
    public partial class example : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "TileLayerId",
                table: "MapReport",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "TileLayerModelId",
                table: "MapReport",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_MapReport_TileLayerModelId",
                table: "MapReport",
                column: "TileLayerModelId");

            migrationBuilder.AddForeignKey(
                name: "FK_MapReport_TileLayer_TileLayerModelId",
                table: "MapReport",
                column: "TileLayerModelId",
                principalTable: "TileLayer",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MapReport_TileLayer_TileLayerModelId",
                table: "MapReport");

            migrationBuilder.DropIndex(
                name: "IX_MapReport_TileLayerModelId",
                table: "MapReport");

            migrationBuilder.DropColumn(
                name: "TileLayerId",
                table: "MapReport");

            migrationBuilder.DropColumn(
                name: "TileLayerModelId",
                table: "MapReport");
        }
    }
}
