using System.Net;
using System.Net.Http.Json;
using BriefQuest.DTOs;
using Microsoft.AspNetCore.Mvc.Testing;

namespace BriefQuest.Tests;

public class BriefEndpointTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;
    public BriefEndpointTests(WebApplicationFactory<Program> factory) => _client = factory.CreateClient();

    [Fact]
    public async Task PostThenGet_ReturnsTheBrief()
    {
        var post = await _client.PostAsJsonAsync("/briefs", new CreateBriefDTO("Test brief", "Some text."));
        Assert.Equal(HttpStatusCode.Created, post.StatusCode);

        var get = await _client.GetAsync(post.Headers.Location);
        Assert.Equal(HttpStatusCode.OK, get.StatusCode);
    }

    [Fact]
    public async Task Get_ReturnsNotFound_ForMissingBrief()
    {
        var get = await _client.GetAsync("/briefs/99999");
        Assert.Equal(HttpStatusCode.NotFound, get.StatusCode);
    }
}
