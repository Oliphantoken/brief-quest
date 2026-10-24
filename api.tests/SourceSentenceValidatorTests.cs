using BriefQuest.Services;
namespace BriefQuest.Tests;

public class SourceSentenceValidatorTests
{
    [Theory]
    [InlineData("Include a design document.", true)]
    [InlineData("Include        a design document.", true)]
    [InlineData("include a design document.", false)]
    [InlineData("Include a document.", false)]
    [InlineData("", false)]
    [InlineData(" ", false)]
    public void IsPresent_ReturnsTrue_WhenSentenceIsInBrief(string sentence, bool expected)
    {
        //Arrange
        var brief = "Submit a video. Include a design document.";

        //Act
        var found = SourceSentenceValidator.IsPresent(brief, sentence);

        //Assert
        Assert.Equal(expected, found);
        
    }
}
