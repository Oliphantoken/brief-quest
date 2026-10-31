using BriefQuest.Utils;
namespace BriefQuest.DTOs;

public record QuestDTO(int ID, string Text, string SourceSentence,
                        Tier? Tier, QuestStatus Status, string? FirstStep);
