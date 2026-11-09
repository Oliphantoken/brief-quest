using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BriefQuest.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddBriefOwner : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "OwnerID",
                table: "briefs",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_briefs_OwnerID",
                table: "briefs",
                column: "OwnerID");

            migrationBuilder.AddForeignKey(
                name: "FK_briefs_AspNetUsers_OwnerID",
                table: "briefs",
                column: "OwnerID",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_briefs_AspNetUsers_OwnerID",
                table: "briefs");

            migrationBuilder.DropIndex(
                name: "IX_briefs_OwnerID",
                table: "briefs");

            migrationBuilder.DropColumn(
                name: "OwnerID",
                table: "briefs");
        }
    }
}
