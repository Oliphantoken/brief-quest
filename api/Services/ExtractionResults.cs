using BriefQuest.Utils;
namespace BriefQuest.Services;

public record ExtractionResults(List<ExtractedRequirement> Requirements, List<ExtractedOutcome>? Outcomes);

public record ExtractedRequirement( string Text, string SourceSentence, Tier? Tier);

public record ExtractedCriterion(Tier? Tier, string Text, string SourceSentence);

public record ExtractedOutcome(string Code, string Text, string SourceSentence, List<ExtractedCriterion> Criteria);

public record ValidatedExtraction(List<ExtractedOutcome> Outcomes, List<ExtractedRequirement> Requirements, int Rejected);