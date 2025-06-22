using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace kartverket2025.Migrations
{
    /// <inheritdoc />
    public partial class AddAndSeedPriorityStatusModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "MapPriorityStatusModelId",
                table: "MapReport",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "MapPriorityStatus",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    priorityStatus = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MapPriorityStatus", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "MapPriorityStatus",
                columns: new[] { "Id", "priorityStatus" },
                values: new object[,]
                {
                    { 1, "Low" },
                    { 2, "Medium" },
                    { 3, "High" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_MapReport_MapPriorityStatusModelId",
                table: "MapReport",
                column: "MapPriorityStatusModelId");

            migrationBuilder.AddForeignKey(
                name: "FK_MapReport_MapPriorityStatus_MapPriorityStatusModelId",
                table: "MapReport",
                column: "MapPriorityStatusModelId",
                principalTable: "MapPriorityStatus",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MapReport_MapPriorityStatus_MapPriorityStatusModelId",
                table: "MapReport");

            migrationBuilder.DropTable(
                name: "MapPriorityStatus");

            migrationBuilder.DropIndex(
                name: "IX_MapReport_MapPriorityStatusModelId",
                table: "MapReport");

            migrationBuilder.DropColumn(
                name: "MapPriorityStatusModelId",
                table: "MapReport");
        }
    }
}
