using System.Text.Json;
using BriefQuest.Entities;
using BriefQuest.Services;
using BriefQuest.Utils;
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


    private const string briefText = "Unit 4 Game Design. LO1: Understand how games are designed. LO2: Build a playable prototype. LO1 Pass: Describe the core mechanics of two games. LO1 Merit: Analyse how those mechanics motivate players. LO1 Distinction: Evaluate the mechanics using player research. LO2 Pass: Build a prototype with three levels. LO2 Merit: Test the prototype with five players. Submit a 1500-word design document and a 3-minute video by Friday 12 December.";

    //Helper methods to create extracted outcome and criterion objects
    private static ExtractedCriterion MakeCriterion(Tier? tier, string sentence)
    {
        return new ExtractedCriterion(tier, "placeholder", sentence);
    }
    private static ExtractedOutcome MakeOutcome(string code, string sentence, params ExtractedCriterion[] criteria)
    {
        return new ExtractedOutcome(code, "placeholder", sentence, criteria.ToList());
    }

    //1. an invalid outcome is dropped along with its criteria
    [Fact]
    public void Validate_DropsOutcomeWithInvalidSentence()
    {
       //Arrange: Prepare the invalid object
        var er = new ExtractionResults(
            [],
            [MakeOutcome("LO5", "LO5: invalid outcome.",
                MakeCriterion(Tier.Pass, "To pass describe your home's coziest place"),
                MakeCriterion(Tier.Merit, "To get merit, you have to justify why you've chosen that place"))  
            ]
        );

        //Act: call the function
        var res = ExtractionService.Validate(er, briefText);

        //Assert: check the result
        //extraction results' Outcomes AFTER Validate() is no longer the same count
        Assert.NotEqual(er.Outcomes!.Count, res.Outcomes.Count);
        //its Criteria is also dropped
        Assert.Empty(res.Outcomes);
        //Both the outcome and its criteria are rejected
        Assert.Equal(3, res.Rejected);

    }

    //2. a valid outcome keeps only its valid criteria
    [Fact]
    public void Validate_OutcomeDropsInvalidCriteria()
    {
        //Arrange: Prepare the invalid object
        var er = new ExtractionResults([],
            [MakeOutcome("LO1", "LO1: Understand how games are designed",
                MakeCriterion(Tier.Pass, "Invalid"),
                MakeCriterion(Tier.Distinction, "LO1 Distinction: Evaluate the mechanics using player research")
            )]
        );
        
        //Act: call the function
        var res = ExtractionService.Validate(er, briefText);

        //Assert
        Assert.Single(res.Outcomes);
        Assert.Equal(1, res.Rejected);
        Assert.NotEqual(er.Outcomes![0].Criteria.Count, res.Outcomes[0].Criteria.Count);
    }

    //3. an outcome with a duplicate code is dropped.
    [Fact]
    public void Validate_dropsDuplicateCode()
    {
        //Arrange
        var er = new ExtractionResults([],
            [MakeOutcome("LO1", "LO1: Understand how games are designed",
                MakeCriterion(Tier.Distinction, "LO1 Distinction: Evaluate the mechanics using player research")),
            MakeOutcome("LO1", "LO1: Understand how games are designed",
                MakeCriterion(Tier.Distinction, "LO1 Distinction: Evaluate the mechanics using player research") 
            )]
        );

        //Act
        ValidatedExtraction res = ExtractionService.Validate(er, briefText);

        //Assert
        Assert.Single(res.Outcomes);
        Assert.Equal(2, res.Rejected);
    }
}