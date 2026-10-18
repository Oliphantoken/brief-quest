using System.Collections;
using BriefQuest.DTOs;

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
        group.MapGet("/{id}", (int id) => {
            var brief = brieflist.FirstOrDefault(b =>b.ID == id);
            return brief == null ? Results.NotFound() : Results.Ok(brief);
        }).WithName("GetBrief");

        //POST a new brief
        group.MapPost("/", (CreateBriefDTO newDTO)=>
        {
           var dto = new BriefDTO(briefCounter, newDTO.Title, newDTO.Text);
           briefCounter++;
           brieflist.Add(dto);

           return Results.CreatedAtRoute("GetBrief", new {id = dto.ID}, dto);
        });
    }

}
