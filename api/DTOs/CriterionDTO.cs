namespace BriefQuest.DTOs;

public record CriterionDTO( int ID, Tier Tier, string Text, string SrcSentence);
public record CreateCriterionDTO(Tier Tier, string Text, string SrcSentence);
public record UpdateCriterionDTO(Tier Tier, string Text, string SrcSentence);
