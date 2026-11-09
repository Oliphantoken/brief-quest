using BriefQuest.Data;
using BriefQuest.DTOs;
using BriefQuest.Utils;
using BriefQuest.Entities;
using BriefQuest.Services;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;

namespace BriefQuest.Endpoints;

public static class BriefEndpoints
{

    static string CurrentUserID(ClaimsPrincipal user) =>
    user.FindFirstValue(ClaimTypes.NameIdentifier)
    ?? throw new InvalidOperationException("No user ID in token.");

    public static void MapBriefEndpoints(this WebApplication app)
    {
        //The "group" is locked behind an authentication check ("are you allowed to be here?")
        var briefGroup = app.MapGroup("/briefs").RequireAuthorization();
        var accountGroup = app.MapGroup("/account").RequireAuthorization();

        //GET /briefs FROM DbContext
        briefGroup.MapGet("/", async (ClaimsPrincipal user, BriefQuestDbContext dbcontext) =>
        {
            var userID = CurrentUserID(user);
            return await dbcontext.briefs.AsNoTracking()
            .Where(b => b.ArchivedAt == null && b.OwnerID == userID)
            .Select(
                b => new BriefDTO(ID: b.ID, Title: b.Title, Text: b.Text)
            ).ToListAsync();
        }).RequireAuthorization();

        //GET /briefs/id
        briefGroup.MapGet("/{id}", async (ClaimsPrincipal user, int id, BriefQuestDbContext dbcontext) =>
        {
            var userID = CurrentUserID(user);
            //FROM DB IN CONTAINER
            BriefDetailDTO? brief = await dbcontext.briefs.AsNoTracking()
                .Where(b => b.ID == id && b.OwnerID == userID && b.ArchivedAt == null)
                .Select(b => new BriefDetailDTO(
                    b.ID, b.Title, b.Text, 
                    b.Outcomes.Select(
                        o => new OutcomeDetailDTO(
                            o.ID, o.Code, o.Text,
                            o.Criteria.Select(
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
        briefGroup.MapPost("/", async (ClaimsPrincipal user, CreateBriefDTO newDTO, BriefQuestDbContext dbcontext) =>
        {
            var userID = CurrentUserID(user);

            //Create entity instance
            var brief = new Brief { OwnerID = userID, Title = newDTO.Title, Text = newDTO.Text, CreatedAt = DateTimeOffset.UtcNow };
            //Add entity to the DB list
            dbcontext.briefs.Add(brief);
            //Save DB changes
            await dbcontext.SaveChangesAsync();
            
            var dto = new BriefDTO(brief.ID, brief.Title, brief.Text);
            return Results.CreatedAtRoute("GetBrief", new { id = dto.ID }, dto);
        });

        //POST new brief to Gemini
        briefGroup.MapPost("/{id}/extract", async (ClaimsPrincipal user, int id, BriefQuestDbContext dbcontext, ExtractionService extractionService) =>
        {
            var userID = CurrentUserID(user);

            Brief? brief = await dbcontext.briefs.FirstOrDefaultAsync(b => b.OwnerID == userID && b.ID == id && b.ArchivedAt == null);
            if (brief is null) return Results.NotFound();
            if(brief.ExtractedAt != null) { //Status code 409
                return Results.Conflict(new {message = "This brief has already been extracted."});
            }

            //Extraction service
            ValidatedExtraction extractedItems;
            try {
                //1. Extract brief items
                extractedItems = await extractionService.ExtractAsync(brief.Text);

            } catch (GeminiUnavailableException ex) {
                var status = ex.StatusCode is 429 or >= 500 ? 503 : 502;
                return Results.Problem(title: "Looks like the AI service is unavailable at the moment. Please try again soon.", statusCode: status);
            } catch (InvalidAiOutputException)
            { //My inspector rejected the answer from the server
                return Results.Problem(title: "The AI's answer could not be verified. Please try again.", statusCode: 502);
            }

            //Organise extraction into lists
            var outcomeEntities = new List<Outcome>();
            var quests = new List<Quest>();

            //2. Convert JSON brief items into C# entities. Start with JSON Outcomes list.
            foreach(var o in extractedItems.Outcomes)
            {
                //For every JSON outcome object, create an Outcome entity
                var outcome = new Outcome {BriefID = brief.ID, Code = o.Code, Text = o.Text };
                
                //For every JSON Criterion object, create a criterion entity
                foreach(var c in o.Criteria)
                {
                    //Create a criterion entity from the extracted brief items
                    var criterion = new Criterion { Tier = c.Tier!.Value, Text = c.Text, SourceSentence = c.SourceSentence };
                    //Add it to the new outcome entity
                    outcome.Criteria.Add(criterion);

                    //Create a quest entity for every criteria and add to the quests list
                    quests.Add(new Quest { BriefID = brief.ID, Criterion = criterion, Text = c.Text,
                                            SourceSentence = c.SourceSentence, Tier = c.Tier});
                }

                //The new outcome entity filled with its criteria entities is added to the outcomesEntities list
                outcomeEntities.Add(outcome);
            }

            //2.b Create a quest for every requirement as well and add to the quests list
            foreach(var r in extractedItems.Requirements)
            {
                quests.Add(new Quest { BriefID = brief.ID, Text = r.Text, SourceSentence = r.SourceSentence, Tier = r.Tier });
            }

            //3. Add outcomes and quests lists to DB context
            dbcontext.Outcomes.AddRange(outcomeEntities);
            dbcontext.Quests.AddRange(quests);

            //4. Time stamp the brief
            brief.ExtractedAt = DateTimeOffset.UtcNow;

            //5. Save everything to the DB
            await dbcontext.SaveChangesAsync();

            //6. Create the DTOs to send back to the client
            var outcomeDTOs = outcomeEntities.Select(o => new OutcomeDetailDTO(o.ID, o.Code, o.Text, 
                        o.Criteria.Select(c => new CriterionDTO(c.ID, c.Tier, c.Text, c.SourceSentence))
                        .ToList()))
                    .ToList();
            var questDTOs = quests.Select(q => new QuestDTO(q.ID, q.Text, q.SourceSentence, q.Tier, q.Status, q.FirstStep))
                                    .ToList();

            //7. Go drink some water after this process
            return Results.Ok(new ExtractionSummaryDTO(outcomeDTOs, questDTOs, extractedItems.Rejected));

        });

        //GET filtered quests
        briefGroup.MapGet("/{id}/quests", async (ClaimsPrincipal user, int id, Tier? tier, QuestStatus? status, BriefQuestDbContext dbcontext) =>
        {
            var userID = CurrentUserID(user);
            var query = dbcontext.Quests.AsNoTracking().Where(q => q.BriefID == id && q.Brief.OwnerID == userID);
            if(tier != null) query = query.Where(q => q.Tier == tier);
            if(status != null) query = query.Where(q => q.Status == status);
            var quests = await query.Select(q => new QuestDTO(q.ID, q.Text, q.SourceSentence, q.Tier, q.Status, q.FirstStep))
                                    .ToListAsync();

            return quests == null ? Results.NoContent() : Results.Ok(quests);            
        });

        //GET brief progress
        briefGroup.MapGet("/{id}/progress", async (ClaimsPrincipal user, int id, BriefQuestDbContext dbcontext) =>
        {
            var userID = CurrentUserID(user);
            var brief = await dbcontext.briefs.AnyAsync(b => b.ID == id && b.OwnerID == userID && b.ArchivedAt == null);
            if(!brief) return Results.NotFound();

            var progress = await dbcontext.Quests.AsNoTracking()    //1. Get quests
                .Where(q => q.BriefID == id)                        //2. where briefID = id
                .GroupBy(q => q.Tier)                               //3. Create groups of tiers from the quests (You'll get a dictionary <Tier?, Quest>)
                .Select(groupOfTier => new TierProgressDTO(groupOfTier.Key, groupOfTier.Count(),    //For each group, create a TierProgressDTO
                                            groupOfTier.Count(q=> q.Status == QuestStatus.Done)) )
                .ToListAsync();

            return Results.Ok(progress);

        });

        //PATCH an existing brief quest with a new status
        briefGroup.MapPatch("/{briefID}/quests/{questID}", async (ClaimsPrincipal user, int briefID, int questID, UpdateQuestStatusDTO dto, BriefQuestDbContext dbcontext) =>
        {
            var userID = CurrentUserID(user);
            Quest? quest = await dbcontext.Quests.FirstOrDefaultAsync(q => q.ID == questID && q.BriefID == briefID && q.Brief.OwnerID == userID );
            if(quest == null) return Results.NotFound();

            quest.Status = dto.Status;

            await dbcontext.SaveChangesAsync();

            return Results.Ok( new QuestDTO (ID: quest.ID, Text: quest.Text, SourceSentence: quest.SourceSentence,
                                Tier: quest.Tier, Status: quest.Status, FirstStep: quest.FirstStep)
                            );

        });

        accountGroup.MapDelete("/", async (ClaimsPrincipal user, UserManager<AppUser> userManager) => {
            var appUser = await userManager.GetUserAsync(user);
            if (appUser == null) return Results.NotFound();

            var result = await userManager.DeleteAsync(appUser);
            return result.Succeeded ? Results.NoContent() : Results.Problem();
            
        }).RequireAuthorization();


        app.MapGet("/criterion", (ClaimsPrincipal user) =>
        {
            return Results.Ok(new CriterionDTO(1, Tier.Merit, "some text", "some sentence"));
        });

    }

}
