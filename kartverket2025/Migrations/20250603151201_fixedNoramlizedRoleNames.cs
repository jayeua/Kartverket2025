using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace kartverket2025.Migrations
{
    /// <inheritdoc />
    public partial class fixedNoramlizedRoleNames : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "1",
                column: "NormalizedName",
                value: "SYSTEMADMIN");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "2",
                column: "NormalizedName",
                value: "CASEHANDLER");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "3",
                column: "NormalizedName",
                value: "MAPUSER");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "1",
                column: "NormalizedName",
                value: "SYSTEM ADMIN");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "2",
                column: "NormalizedName",
                value: "CASE HANDLER");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "3",
                column: "NormalizedName",
                value: "MAP USER");
        }
    }
}
