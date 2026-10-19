using BriefQuest.DTOs;
using Google.GenAI;
using Google.GenAI.Types;

namespace BriefQuest.Endpoints;

public static class OutcomeEndpoints
{
    public static void MapOutcomeEndpoints(this WebApplication app)
    {
        app.MapPost("/extract-requirements", async (BriefDTO brief, Client client) =>
{
    var instructions = """
    Extract a clear software requirements checklist from this school brief.
    Return only JSON in this exact shape:
    { "requirements": [ { "title": "", "description": "", "priority": "MustHave|ShouldHave|CouldHave" } ] }
    """;

    var prompt = $"{instructions}\n\nBrief:\n{brief.Text}";

    var response = await client.Models.GenerateContentAsync(
        model: "gemini-3.8-flash",
        contents: prompt,
        config: new GenerateContentConfig
        {
            ResponseMimeType = "application/json"
        });

    var json = response.Candidates?[0]?.Content?.Parts?[0]?.Text;

    return json is null
        ? Results.Problem("Gemini returned no text.")
        : Results.Content(json, "application/json");
});
    }

}
