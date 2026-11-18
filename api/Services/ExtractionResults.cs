using BriefQuest.Utils;
namespace BriefQuest.Services;
/// <summary>
/// Raw C# lists of extracted outcomes and requirements in record objects.
/// </summary>
public record ExtractionResults(List<ExtractedRequirement> Requirements, List<ExtractedOutcome>? Outcomes);

/// <summary>
/// Raw C# extracted requirement object
/// </summary>
public record ExtractedRequirement( string Text, string SourceSentence, Tier? Tier, bool IsDeadline = false);

/// <summary>
/// Raw C# extracted criterion object
/// </summary>
public record ExtractedCriterion(Tier? Tier, string Text, string SourceSentence);

/// <summary>
/// Raw C# extracted outcome object
/// </summary>
public record ExtractedOutcome(string Code, string Text, string SourceSentence, List<ExtractedCriterion> Criteria);

/// <summary>
/// The validated version of the C# extractions of outcomes and requirements,
/// and a count of how many of them that didn't make it to this stage (not validated)
/// </summary>
public record ValidatedExtraction(List<ExtractedOutcome> Outcomes, List<ExtractedRequirement> Requirements, int Rejected);