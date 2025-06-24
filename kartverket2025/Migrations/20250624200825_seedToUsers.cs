using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace kartverket2025.Migrations
{
    /// <inheritdoc />
    public partial class seedToUsers : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "AspNetUsers",
                columns: new[] { "Id", "AccessFailedCount", "ConcurrencyStamp", "Email", "EmailConfirmed", "FirstName", "LastName", "LockoutEnabled", "LockoutEnd", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "SecurityStamp", "TwoFactorEnabled", "UserName" },
                values: new object[,]
                {
                    { "1", 0, "d3d392e9-772c-40c0-b513-ce4b625085a1", "systemadmin@test.com", false, "System", "Admin", false, null, "SYSTEMADMIN@TEST.COM", "SYSTEMADMIN@TEST.COM", "AQAAAAIAAYagAAAAEMxXbxWDOpXWxsaSbzc6nByShTKuyfVFE6U5OfnHP09Ve2hAwkcEcAmI/WyIh3RmGA==", null, false, "bdf1da39-c543-4cc8-944f-e670ffd3025b", false, "systemadmin@test.com" },
                    { "2", 0, "6f995867-8d03-4e6f-a972-864be9bdc02b", "casehandler@test.com", false, "Case", "Handler", false, null, "CASEHANDLER@TEST.COM", "CASEHANDLER@TEST.COM", "AQAAAAIAAYagAAAAEGMgFXqmrdAf8av5kHlVUhmv9352yoJ7ud0GZ6ceQFZbLpAOg3VVluMBvZqJ5oFIAA==", null, false, "a3d8f7ee-ba89-4884-b48c-721bfed96e6f", false, "casehandler@test.com" },
                    { "3", 0, "c5272542-7503-4852-98f7-ad08c65e21b6", "mapuser@test.com", false, "Map", "User", false, null, "MAPUSER@TEST.COM", "MAPUSER@TEST.COM", "AQAAAAIAAYagAAAAEG4lfwhMXTAE2rvE/YnLA2//qNOy71vIfRTj/eZyTe01gd2PIbjlHUt6cLE1t8zxoQ==", null, false, "a3d59936-8334-4ee5-a4d8-b2cc6de0a788", false, "mapuser@test.com" }
                });

            migrationBuilder.InsertData(
                table: "AspNetUserRoles",
                columns: new[] { "RoleId", "UserId" },
                values: new object[,]
                {
                    { "1", "1" },
                    { "2", "2" },
                    { "3", "3" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { "1", "1" });

            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { "2", "2" });

            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { "3", "3" });

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "1");

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "2");

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "3");
        }
    }
}
