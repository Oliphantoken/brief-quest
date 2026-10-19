namespace BriefQuest.DTOs;

public record QuestDTO(int ID, int briefID, int criterionID, string Text, string srcSentence, string status, string firstStep );
