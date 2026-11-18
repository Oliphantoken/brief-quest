using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BriefQuest.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddDeadlineOption : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsDeadline",
                table: "Quests",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsDeadline",
                table: "Quests");
        }
    }
}
