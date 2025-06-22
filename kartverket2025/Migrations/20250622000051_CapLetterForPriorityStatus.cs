using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace kartverket2025.Migrations
{
    /// <inheritdoc />
    public partial class CapLetterForPriorityStatus : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "priorityStatus",
                table: "MapPriorityStatus",
                newName: "PriorityStatus");

            migrationBuilder.AddColumn<string>(
                name: "Title",
                table: "MapReport",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Title",
                table: "MapReport");

            migrationBuilder.RenameColumn(
                name: "PriorityStatus",
                table: "MapPriorityStatus",
                newName: "priorityStatus");
        }
    }
}
