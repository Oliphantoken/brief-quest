using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BriefQuest.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddExtractedAtToBrief : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "ExtractedAt",
                table: "briefs",
                type: "timestamp with time zone",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ExtractedAt",
                table: "briefs");
        }
    }
}
