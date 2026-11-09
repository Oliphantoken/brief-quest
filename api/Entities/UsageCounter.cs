namespace BriefQuest.Entities;

public class UsageCounter
{
    public required string OwnerID { get; set; }
    public AppUser Owner { get; set; } = null!;
    public DateOnly Day { get; set; }
    public int Count { get; set; }

}
