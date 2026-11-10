using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BriefQuest.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddQuestBloomLevel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "BloomLevel",
                table: "Quests",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BloomLevel",
                table: "Quests");
        }
    }
}
