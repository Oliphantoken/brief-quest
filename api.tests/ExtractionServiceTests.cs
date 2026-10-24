using BriefQuest.Services;
namespace BriefQuest.Tests;

public class ExtractionServiceTests
{
    [Fact]
    public void Parse_ReadsTierWordsAsEnum()
    {
         var json = """
        { "requirements": [ { "text": "Include a design document",
          "sourceSentence": "Include a design document.", "tier": "Merit" } ] }
        """;

        var result = ExtractionService.Parse(json);

        Assert.Single(result.Requirements);
        Assert.Equal(Tier.Merit, result.Requirements[0].Tier);

    }

    [Fact]
    public void Parse_ThrowsOnBrokenJson()
    {
        Assert.Throws<InvalidAiOutputException>(() => ExtractionService.Parse("not Json"));
    }

}
