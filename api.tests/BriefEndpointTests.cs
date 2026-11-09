using System.Net;
using System.Net.Http.Json;

namespace BriefQuest.Tests;

[Collection("Integration")]
public class BriefEndpointTests
{
    private readonly ApiFactory _factory;
    public BriefEndpointTests(ApiFactory factory) => _factory = factory;

    [Fact]
    public async Task PostNewBriefThenGet_ReturnsTheBrief()
    {
        //Generate a new random email name, so it doesn't clash with other tests
        var email = $"test-{Guid.NewGuid():N}@example.com";

        //1. authenticate
        var token = await _factory.RegisterAndLoginAsync(email);
        var client = _factory.CreateAuthenticatedClient(token);

        //2. Create a new brief and check you get 201 status code
        var post = await client.PostAsJsonAsync("/briefs", new{title="Test brief", text="Some text."});
        Assert.Equal(HttpStatusCode.Created, post.StatusCode);

        //3. Get the brief and check you get 200 status code
        var get = await client.GetAsync(post.Headers.Location);
        Assert.Equal(HttpStatusCode.OK, get.StatusCode);
    }

    [Fact]
    public async Task GetBrief_ReturnsNotFound_ForMissingBrief()
    {
        //Generate a new random email name, so it doesn't clash with other tests
        var email = $"test-{Guid.NewGuid():N}@example.com";

        //1. Authenticate
        var token = await _factory.RegisterAndLoginAsync(email);
        var client = _factory.CreateAuthenticatedClient(token);

        //2. Get brief with impossible id
        var get = await client.GetAsync("/briefs/99999");

        //3 Check you get 404 status code
        Assert.Equal(HttpStatusCode.NotFound, get.StatusCode);
    }

}
