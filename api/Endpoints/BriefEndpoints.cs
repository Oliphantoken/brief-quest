using BriefQuest.Data;
using BriefQuest.DTOs;
using BriefQuest.Utils;
using BriefQuest.Entities;
using BriefQuest.Services;
using Microsoft.EntityFrameworkCore;
using System.Runtime.InteropServices;
using System.Diagnostics;

namespace BriefQuest.Endpoints;

public static class BriefEndpoints
{

    public static void MapBriefEndpoints(this WebApplication app)
    {
        //var brieflist = new List<BriefDTO>();
        //int briefCounter = 1;
        var group = app.MapGroup("/briefs");

        //GET /briefs FROM DbContext
        group.MapGet("/", async (BriefQuestDbContext dbcontext) =>
        {
            return await dbcontext.briefs.AsNoTracking()
            .Where(b => b.ArchivedAt == null)
            .Select(
                b => new BriefDTO(ID: b.ID, Title: b.Title, Text: b.Text)
            ).ToListAsync();
        });

        //GET /briefs/id
        group.MapGet("/{id}", async (int id, BriefQuestDbContext dbcontext) =>
        {
            //FROM DB IN CONTAINER
            BriefDetailDTO? brief = await dbcontext.briefs.AsNoTracking()
                .Where( b => b.ID == id && b.ArchivedAt == null)
                .Select(b => new BriefDetailDTO(
                    b.ID, b.Title, b.Text, 
                    b.Outcomes.Select(
                        o => new OutcomeDetailDTO(
                            o.ID, o.Code, o.Text,
                            o.Criterion.Select(
                                c => new CriterionDTO(c.ID, c.Tier, c.Text, c.SourceSentence)
                            ).ToList()
                        )
                    ).ToList(),
                    b.Quests.Select(
                        q => new QuestDTO(q.ID, q.Text, q.SourceSentence, q.Tier, q.Status, q.FirstStep)
                    ).ToList())
                )
                .FirstOrDefaultAsync();                            
                

            return brief == null ? Results.NotFound(): Results.Ok(brief);
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
            if(brief.ExtractedAt != null) { //Status code 409
                return Results.Conflict(new {message = "This brief has already been extracted."});
            }

            try {
                //1. Extract requirements
                var result = await extraction.ExtractAsync(brief.Text);

                //2. Create a list of quests from valid requirements
                var quests = result.validRequirements.Select(r => new Quest
                {
                    BriefID = brief.ID,
                    Text = r.Text,
                    SourceSentence = r.SourceSentence,
                    Tier = r.Tier
                }).ToList();

                //3. Add quests to DB context
                dbcontext.Quests.AddRange(quests);

                //4. Time stamp the brief
                brief.ExtractedAt = DateTimeOffset.UtcNow;

                //5. Save quests in DB
                await dbcontext.SaveChangesAsync();

                return Results.Ok(quests.Select(q => new QuestDTO(q.ID, q.Text, q.SourceSentence, q.Tier, q.Status, q.FirstStep)));

            } catch (GeminiUnavailableException ex) {
                var status = ex.StatusCode is 429 or >= 500 ? 503 : 502;
                return Results.Problem(title: "Looks like the AI service is unavailable at the moment. Please try again soon.", statusCode: status);
            } catch (InvalidAiOutputException)
            { //My insepctor rejected the answer from the server
                return Results.Problem(title: "The AI's answer could not be verified. Please try again.", statusCode: 502);
            }
        });

        group.MapGet("/{id}/quests", async (int id, Tier? tier, QuestStatus? status, BriefQuestDbContext dbcontext) =>
        {
            var query = dbcontext.Quests.AsNoTracking().Where(q => q.BriefID == id);
            if(tier != null) query = query.Where(q => q.Tier == tier);
            if(status != null) query = query.Where(q => q.Status == status);
            var quests = await query.Select(q => new QuestDTO(q.ID, q.Text, q.SourceSentence, q.Tier, q.Status, q.FirstStep))
                                    .ToListAsync();

                                    Debug.WriteLine("questslist:" + quests);

            return quests == null ? Results.NoContent() : Results.Ok(quests);            
        });

        app.MapGet("/criterion", () =>
        {
            return Results.Ok(new CriterionDTO(1, Tier.Merit, "some text", "some sentence"));
        });

    }

}
