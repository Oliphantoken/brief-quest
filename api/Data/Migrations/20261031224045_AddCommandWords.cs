using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace BriefQuest.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddCommandWords : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CommandWords",
                columns: table => new
                {
                    Word = table.Column<string>(type: "text", nullable: false),
                    PlainMeaning = table.Column<string>(type: "text", nullable: false),
                    BloomLevel = table.Column<string>(type: "text", nullable: false),
                    Example = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CommandWords", x => x.Word);
                });

            migrationBuilder.InsertData(
                table: "CommandWords",
                columns: new[] { "Word", "BloomLevel", "Example", "PlainMeaning" },
                values: new object[,]
                {
                    { "Analyse", "Analyse", "Analyse how level design, controls, and rewards affect player motivation.", "Break something into parts and explain how the parts relate." },
                    { "Apply", "Apply", "Apply your research on player motivation to design the game’s reward system.", "Use a skill, method, or idea in a real task." },
                    { "Compare", "Analyse", "Compare the movement systems in two similar platformer games.", "Say how two or more things are similar and different." },
                    { "Contrast", "Analyse", "Contrast the reward systems in two mobile puzzle games.", "Focus mainly on the differences between things." },
                    { "Create", "Create", "Create a playable 2D platformer prototype that meets the brief.", "Make something new from your ideas, research, and skills." },
                    { "Critically evaluate", "Evaluate", "Critically evaluate two similar games and explain what your game will learn from each.", "Look carefully at strengths, weaknesses, and evidence before making a judgement." },
                    { "Demonstrate", "Apply", "Demonstrate that the player can complete the first level without getting stuck.", "Show that you can do something or that something works." },
                    { "Describe", "Understand", "Describe how the player moves and jumps in your game prototype.", "Say what something is like or what happens." },
                    { "Design", "Create", "Design a level that teaches the player the jump mechanic gradually.", "Decide how something will look, work, or be organised." },
                    { "Develop", "Create", "Develop your initial game concept into a playable Unity prototype.", "Build or improve an idea over time." },
                    { "Evaluate", "Evaluate", "Evaluate whether your prototype meets the brief’s Merit and Distinction requirements.", "Judge how good something is using clear reasons or criteria." },
                    { "Experiment", "Apply", "Experiment with different jump heights to find the most enjoyable control feel.", "Try different options to see what works best." },
                    { "Explain", "Understand", "Explain why you chose a side-scrolling camera for your platformer.", "Make something clear by saying how or why it works." },
                    { "Identify", "Remember", "Identify the main gameplay mechanics in your platformer prototype.", "Point out or name something specific." },
                    { "Implement", "Apply", "Implement the double-jump mechanic in Unity using your design document.", "Put a plan or idea into working form." },
                    { "Investigate", "Analyse", "Investigate how difficulty curves affect player retention in platformers.", "Find out more about something by researching or testing it." },
                    { "Justify", "Evaluate", "Justify your decision to use pixel-art visuals based on player research.", "Give strong reasons for a decision or choice." },
                    { "List", "Remember", "List the assets you need to create for your 2D platformer.", "Give items one by one, usually in a simple sequence." },
                    { "Plan", "Create", "Plan the stages of your platformer prototype from research to final submission.", "Decide what you will do, when, and how." },
                    { "Present", "Create", "Present your game concept and prototype progress to your tutor or class.", "Share your work or ideas clearly with an audience." },
                    { "Prioritise", "Evaluate", "Prioritise the essential mechanics you must finish before the submission deadline.", "Decide which things are most important and should be done first." },
                    { "Produce", "Create", "Produce a 1,500-word design document and a three-minute video walkthrough.", "Make and complete a finished piece of work." },
                    { "Recommend", "Evaluate", "Recommend improvements to the game based on your playtest results.", "Suggest the best option and explain why." },
                    { "Reflect", "Evaluate", "Reflect on how playtesting changed your ideas for the first level.", "Think back over what you did, what worked, and what you would change." },
                    { "Summarise", "Understand", "Summarise the main goals of your game design document.", "Give the main points in a shorter form." },
                    { "Test", "Evaluate", "Test your prototype with three players and record their feedback.", "Try something to check whether it works or meets a requirement." }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CommandWords");
        }
    }
}
