using BriefQuest.Services;

namespace BriefQuest.DTOs;

public record ExtractionResultDTO(List<ExtractedRequirement> validRequirements, int rejectedRequirements);

public record ExtractionSummaryDTO(List<OutcomeDetailDTO> outcomes, List<QuestDTO> quests, int rejectedBriefItems);