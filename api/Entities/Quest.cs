using BriefQuest.Utils;
namespace BriefQuest.Entities;


public class Quest {
    public int ID {get; set; }

    public int BriefID {get; set; }
    public Brief Brief {get; set; } = null!;

    public int? CriterionID { get; set; }
    public Criterion? Criterion { get; set; }

    public required string Text { get; set; }
    public required string SourceSentence { get; set; }

    public Tier? Tier {get; set; }
    public QuestStatus Status { get; set; } = QuestStatus.TODO;

    public string? FirstStep { get; set; }
    public DateTimeOffset CreatedAt {get; set; } = DateTimeOffset.UtcNow;
    
}
