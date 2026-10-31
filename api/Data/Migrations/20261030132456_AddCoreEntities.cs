using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace BriefQuest.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddCoreEntities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Title",
                table: "briefs",
                type: "character varying(200)",
                maxLength: 200,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.CreateTable(
                name: "Outcomes",
                columns: table => new
                {
                    ID = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    BriefID = table.Column<int>(type: "integer", nullable: false),
                    Code = table.Column<string>(type: "text", nullable: false),
                    Text = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Outcomes", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Outcomes_briefs_BriefID",
                        column: x => x.BriefID,
                        principalTable: "briefs",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Criteria",
                columns: table => new
                {
                    ID = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    OutcomeID = table.Column<int>(type: "integer", nullable: false),
                    Tier = table.Column<string>(type: "text", nullable: false),
                    Text = table.Column<string>(type: "text", nullable: false),
                    SourceSentence = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Criteria", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Criteria_Outcomes_OutcomeID",
                        column: x => x.OutcomeID,
                        principalTable: "Outcomes",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Quests",
                columns: table => new
                {
                    ID = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    BriefID = table.Column<int>(type: "integer", nullable: false),
                    CriterionID = table.Column<int>(type: "integer", nullable: true),
                    Text = table.Column<string>(type: "text", nullable: false),
                    SourceSentence = table.Column<string>(type: "text", nullable: false),
                    Tier = table.Column<string>(type: "text", nullable: true),
                    Status = table.Column<string>(type: "text", nullable: false),
                    FirstStep = table.Column<string>(type: "text", nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Quests", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Quests_Criteria_CriterionID",
                        column: x => x.CriterionID,
                        principalTable: "Criteria",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Quests_briefs_BriefID",
                        column: x => x.BriefID,
                        principalTable: "briefs",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Criteria_OutcomeID",
                table: "Criteria",
                column: "OutcomeID");

            migrationBuilder.CreateIndex(
                name: "IX_Outcomes_BriefID_Code",
                table: "Outcomes",
                columns: new[] { "BriefID", "Code" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Quests_BriefID",
                table: "Quests",
                column: "BriefID");

            migrationBuilder.CreateIndex(
                name: "IX_Quests_CriterionID",
                table: "Quests",
                column: "CriterionID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Quests");

            migrationBuilder.DropTable(
                name: "Criteria");

            migrationBuilder.DropTable(
                name: "Outcomes");

            migrationBuilder.AlterColumn<string>(
                name: "Title",
                table: "briefs",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(200)",
                oldMaxLength: 200);
        }
    }
}
