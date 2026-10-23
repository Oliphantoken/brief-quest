using BriefQuest.Services;

namespace BriefQuest.DTOs;

public record class ExtractionResultDTO(List<ExtractedRequirement> validRequirements, int rejectedRequirements);
