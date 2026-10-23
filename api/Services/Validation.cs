namespace BriefQuest.Services;

public class GeminiUnavailableException(int statusCode, string body)
    : Exception($"Gemini returned {statusCode}: {body}")
{
    public int StatusCode { get; } = statusCode;
}

public class InvalidAiOutputException(string message) : Exception(message);

/// <summary>
/// This (inspector) class fact checks if the sentence it has been given is actually in the brief ad verbum. 
/// </summary>
public static class SourceSentenceValidator
{
    public static bool IsPresent(string briefText, string? sentence)
    {
        if(string.IsNullOrWhiteSpace(sentence)) return false;
        return Normalise(briefText).Contains(Normalise(sentence), StringComparison.Ordinal);
    }

    /// <summary>
    /// This method squashes runs of spaces and line breaks, because models often change those. Comparison is otherwise exact and case-sensitive,
    /// which is deliberately strict. Too loose lets paraphrases through, too strict rejects honest answers.
    /// </summary>
    /// <returns>A string of text without runs of spaces and line breaks.</returns>
    private static string Normalise(string text)
    {
        return string.Join(' ', text.Split((char[]?)null, StringSplitOptions.RemoveEmptyEntries));
    }
}