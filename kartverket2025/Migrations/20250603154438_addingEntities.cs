using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace kartverket2025.Migrations
{
    /// <inheritdoc />
    public partial class addingEntities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Status",
                table: "MapReport",
                newName: "StatusDescription");

            migrationBuilder.InsertData(
                table: "MapReportStatus",
                columns: new[] { "Id", "Status" },
                values: new object[,]
                {
                    { 1, "Waiting" },
                    { 2, "On the way" },
                    { 3, "Finished" },
                    { 4, "Revoked" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "MapReportStatus",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "MapReportStatus",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "MapReportStatus",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "MapReportStatus",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.RenameColumn(
                name: "StatusDescription",
                table: "MapReport",
                newName: "Status");
        }
    }
}
