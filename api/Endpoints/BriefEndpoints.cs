using System.Collections;
using BriefQuest.DTOs;
using BriefQuest.Services;

namespace BriefQuest.Endpoints;

public static class BriefEndpoints
{

    public static void MapBriefEndpoints(this WebApplication app)
    {
        var brieflist = new List<BriefDTO>();
        int briefCounter = 1;
        var group = app.MapGroup("/briefs");

        //GET /briefs
        group.MapGet("/", () => brieflist);

        //GET /briefs/id
        group.MapGet("/{id}", (int id) =>
        {
            var brief = brieflist.FirstOrDefault(b => b.ID == id);
            return brief == null ? Results.NotFound() : Results.Ok(brief);
        }).WithName("GetBrief");

        //POST a new brief
        group.MapPost("/", (CreateBriefDTO newDTO) =>
        {
            var dto = new BriefDTO(briefCounter, newDTO.Title, newDTO.Text);
            briefCounter++;
            brieflist.Add(dto);

            return Results.CreatedAtRoute("GetBrief", new { id = dto.ID }, dto);
        });

        //POST new brief to Gemini
        group.MapPost("/{id}/extract", async (int id, ExtractionService extraction) =>
        {
            var brief = brieflist.FirstOrDefault(b => b.ID == id);
            if (brief is null) return Results.NotFound();

            try {
                var json = await extraction.ExtractAsync(brief.Text);
                return Results.Ok(json);
            } catch (GeminiUnavailableException ex) {
                var status = ex.StatusCode is 429 or >= 500 ? 503 : 502;
                return Results.Problem(title: "Looks like the AI service is unavailable at the moment. Please try again soon.", statusCode: status);
            } catch (InvalidAiOutputException)
            { //My insepctor rejected the answer from the server
                return Results.Problem(title: "The AI's answer could not be verified. Please try again.", statusCode: 502);
            }
        });

        app.MapGet("/criterion", () =>
        {
            return Results.Ok(new CriterionDTO(1, Tier.Merit, "some text", "some sentence"));
        });
    }

}
