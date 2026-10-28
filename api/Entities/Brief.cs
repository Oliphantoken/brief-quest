namespace BriefQuest.Entities;

/// <summary>
/// This class is like the "EntityModel" classes in Flutter where an object is mapped to a table.
/// It doesn't include toMap/FromMap as EF Core takes care of the API<->DB conversions.
/// </summary>
public class Brief
{
    public int ID {get; set;}
    public required string Title {get; set; }
    public required string Text{get; set; }
    public DateTimeOffset CreatedAt {get; set; }
    public DateTimeOffset? ArchivedAt {get; set; }

}
