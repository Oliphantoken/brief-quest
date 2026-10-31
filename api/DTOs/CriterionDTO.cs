using BriefQuest.Utils;
namespace BriefQuest.DTOs;

public record CriterionDTO( int ID, Tier Tier, string Text, string SourceSentence);
public record CreateCriterionDTO(Tier Tier, string Text, string SourceSentence);
public record UpdateCriterionDTO(Tier Tier, string Text, string SourceSentence);
