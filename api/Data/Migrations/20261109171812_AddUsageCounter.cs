using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BriefQuest.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddUsageCounter : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "UsageCounters",
                columns: table => new
                {
                    OwnerID = table.Column<string>(type: "text", nullable: false),
                    Day = table.Column<DateOnly>(type: "date", nullable: false),
                    Count = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UsageCounters", x => new { x.OwnerID, x.Day });
                    table.ForeignKey(
                        name: "FK_UsageCounters_AspNetUsers_OwnerID",
                        column: x => x.OwnerID,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UsageCounters");
        }
    }
}
