using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Reader.Common.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddedReporttable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Reports",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OrganizationName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    StartDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    EndDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    ContactInfo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ReportID = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Reports", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "ReportPolicies",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PolicyType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PolicyStrings = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PolicyDomain = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MXHosts = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FailureDetails = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TotalSuccessfulSessionCount = table.Column<int>(type: "int", nullable: true),
                    TotalFailureSessionCount = table.Column<int>(type: "int", nullable: true),
                    ReportID = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReportPolicies", x => x.ID);
                    table.ForeignKey(
                        name: "FK_ReportPolicies_Reports_ReportID",
                        column: x => x.ReportID,
                        principalTable: "Reports",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ReportPolicies_ReportID",
                table: "ReportPolicies",
                column: "ReportID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ReportPolicies");

            migrationBuilder.DropTable(
                name: "Reports");
        }
    }
}
