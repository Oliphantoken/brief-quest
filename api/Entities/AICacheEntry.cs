namespace BriefQuest.Entities;

/// <summary>
/// A cached memory of the extraction of a brief. The brief gets a hash,
/// and if identified, will return the cache instead of extracting again or
/// just returning an error message.
/// </summary>
public class AICacheEntry
{
    public required string OwnerID { get; set; }
    public AppUser Owner { get; set; } = null!;
    public required string BriefHash { get; set; }
    public required string ResponseJson { get; set; }
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;

}
