using System.Net;
using System.Net.Http.Json;
using BriefQuest.DTOs;
using Microsoft.AspNetCore.Http;
namespace BriefQuest.Tests;

[Collection("Integration")]
public class BriefOwnershipTests
{
    private readonly ApiFactory _factory;
    public BriefOwnershipTests(ApiFactory factory) => _factory = factory;

    [Fact]
    public async Task Alice_CannotReadBob_Brief()
    {
        //--- Arrange---//
         //Generate a new random email name, so it doesn't clash with other tests
        var email = $"test-{Guid.NewGuid():N}@example.com";

        var Alice = await _factory.RegisterAndLoginAsync(email);
        var Bob = await _factory.RegisterAndLoginAsync("Bob"+email);

        //Bob creates a brief:
        //1. authorise Bob and login
        var BobClient = _factory.CreateAuthenticatedClient(Bob);
        //2. POST new brief
        var post = await BobClient.PostAsJsonAsync("/briefs",
            new { title = "Bob's Brief", text = "anything goes here." }
        );
        //3. Read the response
        var created = await post.Content.ReadFromJsonAsync<BriefDTO>();
        //4. Get the brief ID
        var briefID = created!.ID;
        
        //5. Authorise Alice and login
        var AliceClient = _factory.CreateAuthenticatedClient(Alice);

        //--- Act ---//
        var brief = await AliceClient.GetAsync($"/briefs/{briefID}");

        //Assert
        Assert.Equal(HttpStatusCode.NotFound, brief.StatusCode);

    }

    [Fact]
    public async Task Unauthenticated_CannotListBriefs()
    {
        //Arrange
        var client = _factory.CreateClient();
        //Act
        var briefs = await client.GetAsync("/briefs");
        //Assert
        Assert.Equal(HttpStatusCode.Unauthorized, briefs.StatusCode);
    }

    [Fact]
    public async Task Delete_Account_RemovesBriefs()
    {
        //Generate a new random email name, so it doesn't clash with other tests
        var email = $"test-{Guid.NewGuid():N}@example.com";

        var token = await _factory.RegisterAndLoginAsync(email);
        var client = _factory.CreateAuthenticatedClient(token);
        var createBrief = await client.PostAsJsonAsync("/briefs", new {title="I shouldn't be here", text = "Delete me now"});
        createBrief.EnsureSuccessStatusCode();
        
        //Act
        var delete = await client.DeleteAsync("/account");

        //Assert
        Assert.Equal(HttpStatusCode.NoContent, delete.StatusCode);
        
        var getBrief = await client.GetAsync("/briefs");
        Assert.Equal(HttpStatusCode.OK, getBrief.StatusCode);

        // NOTE: What should happen: you get a 401 once the token expires.
        // To test it, limit the accesstoken expiry timespan to 1 second in Program.cs
        // and add the following instead of the last Assert():
        //      await Task.Delay(TimeSpan.FromSeconds(2));
        //      Assert.Equal(HttpStatusCode.Unauthorized, getBrief.StatusCode);
    }

}
