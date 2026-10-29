using System.Collections;
using BriefQuest.Data;
using BriefQuest.DTOs;
using BriefQuest.Entities;
using BriefQuest.Services;
using Microsoft.EntityFrameworkCore;

namespace BriefQuest.Endpoints;

public static class BriefEndpoints
{

    public static void MapBriefEndpoints(this WebApplication app)
    {
        //var brieflist = new List<BriefDTO>();
        //int briefCounter = 1;
        var group = app.MapGroup("/briefs");

        

        //GET /briefs
        //group.MapGet("/", () => brieflist);
        //FROM DbContext
        group.MapGet("/", async (BriefQuestDbContext dbcontext) =>
        {
            return await dbcontext.briefs.AsNoTracking().Select(
                b => new BriefDTO(ID: b.ID, Title: b.Title, Text: b.Text)
            ).ToListAsync();
        });

        //GET /briefs/id
        group.MapGet("/{id}", async (int id, BriefQuestDbContext dbcontext) =>
        {
            //BriefDTO brief;
            //IN-MEMORY LIST
            //var brief = brieflist.FirstOrDefault(b => b.ID == id);

            //FROM DB IN CONTAINER
            var brief = await dbcontext.briefs.AsNoTracking().FirstOrDefaultAsync(
                b => b.ID == id
            );

            return brief == null ? Results.NotFound()
                : Results.Ok(
                    new BriefDTO(ID: brief.ID, Title: brief.Title, Text: brief.Text)
                );
        }).WithName("GetBrief");

        //POST a new brief
        group.MapPost("/", async (CreateBriefDTO newDTO, BriefQuestDbContext dbcontext) =>
        {
            //var dto = new BriefDTO(briefCounter, newDTO.Title, newDTO.Text);
            //briefCounter++;
            //brieflist.Add(dto);

            //Create entity instance
            var brief = new Brief { Title = newDTO.Title, Text = newDTO.Text, CreatedAt = DateTimeOffset.UtcNow };
            //Add entity to the DB list
            dbcontext.briefs.Add(brief);
            //Save DB changes
            await dbcontext.SaveChangesAsync();
            
            var dto = new BriefDTO(brief.ID, brief.Title, brief.Text);
            return Results.CreatedAtRoute("GetBrief", new { id = dto.ID }, dto);
        });

        //POST new brief to Gemini
        group.MapPost("/{id}/extract", async (int id, BriefQuestDbContext dbcontext, ExtractionService extraction) =>
        {
            //var brief = brieflist.FirstOrDefault(b => b.ID == id);
            Brief? brief = await dbcontext.briefs.FirstOrDefaultAsync(b => b.ID == id);
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
