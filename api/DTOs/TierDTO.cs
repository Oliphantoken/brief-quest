using BriefQuest.Utils;

namespace BriefQuest.DTOs;

public record TierProgressDTO(Tier? Tier, int Total, int Done);
