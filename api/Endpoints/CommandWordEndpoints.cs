using System.CodeDom;
using BriefQuest.Data;
using BriefQuest.DTOs;
using BriefQuest.Utils;
using Microsoft.EntityFrameworkCore;

namespace BriefQuest.Endpoints;

public static class CommandWordEndpoints
{

    public static void MapCommandWordEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/command-words").WithTags("CommandWords");

        //GET all commandwords in each bloom level
        group.MapGet("/", async (BriefQuestDbContext dbContext, BloomLevel? bloomLevel) =>
        {
            var query = dbContext.CommandWords.AsNoTracking().AsQueryable();

            if(bloomLevel != null)
            {
                query = query.Where(q => q.BloomLevel == bloomLevel);
            }

            var result = await query.OrderBy(c => c.BloomLevel)
                                    .ThenBy(c => c.Word)
                                    .Select(c => new CommandWordDTO(c.Word, c.PlainMeaning, c.BloomLevel, c.Example))
                                    .ToListAsync();
            
            return Results.Ok(result);
        })
        .WithSummary("List all command words, optionally filtered by bloom level")
        .Produces<List<CommandWordDTO>>(200);


        //GET command word explanation
        group.MapGet("/{word}", async (string word, BriefQuestDbContext dbContext) =>
        {
            var cw = await dbContext.CommandWords.AsNoTracking().FirstOrDefaultAsync(c => c.Word.ToLower() == word.ToLower());

            return cw == null ? Results.Problem(title: $"'{word}' was not found in the command-word list.", statusCode: 404)
                : Results.Ok(new CommandWordDTO(cw.Word, cw.PlainMeaning, cw.BloomLevel, cw.Example));
            
        })
        .WithSummary("Look up a command word's plain meaning and Bloom level")
        .Produces<CommandWordDTO>(200)
        .ProducesProblem(404)
        .WithName("GetCommandWord");


    }

}
