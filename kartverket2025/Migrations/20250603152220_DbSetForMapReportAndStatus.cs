using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace kartverket2025.Migrations
{
    /// <inheritdoc />
    public partial class DbSetForMapReportAndStatus : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "MapReportStatus",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MapReportStatus", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MapReport",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CaseHandler = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AreaJson = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Kommunenavn = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Fylkenavn = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MapReportStatusId = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MapReport", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MapReport_MapReportStatus_MapReportStatusId",
                        column: x => x.MapReportStatusId,
                        principalTable: "MapReportStatus",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MapReport_MapReportStatusId",
                table: "MapReport",
                column: "MapReportStatusId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MapReport");

            migrationBuilder.DropTable(
                name: "MapReportStatus");
        }
    }
}
