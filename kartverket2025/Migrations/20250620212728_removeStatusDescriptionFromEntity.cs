using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace kartverket2025.Migrations
{
    /// <inheritdoc />
    public partial class removeStatusDescriptionFromEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "StatusDescription",
                table: "MapReport");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "StatusDescription",
                table: "MapReport",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
