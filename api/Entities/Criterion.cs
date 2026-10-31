using BriefQuest.Utils;
namespace BriefQuest.Entities;

public class Criterion
{
    public int ID { get; set; }
    public int OutcomeID { get; set; }
    public Outcome Outcome { get; set; } = null!;
    public Tier Tier { get; set; }
    public required string Text { get; set; }
    public required string SourceSentence { get; set; }

    public List<Quest>? Quests;
}
