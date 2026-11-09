using System.Net.Http.Json;
using System.Text.Json;
using BriefQuest.Data;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

public class ApiFactory : WebApplicationFactory<Program>
{
    public ApiFactory()
    {
        var connection = Environment.GetEnvironmentVariable("TEST_DB_CONNECTION")
            ?? "Host=127.0.0.1;Port=5432;Database=briefquest_test;Username=briefquest;Password=thebriefquest";
        Environment.SetEnvironmentVariable("ConnectionStrings__BriefQuest", connection);
    }

    protected override IHost CreateHost(IHostBuilder builder)
    {
        var host = base.CreateHost(builder);
        using var scope = host.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<BriefQuestDbContext>();
        db.Database.Migrate();
        db.Users.ExecuteDelete();
        return host;
    }

    //Tests need to log users in. Get the Access Token here
    public async Task<string> RegisterAndLoginAsync(string email, string password="Test-pass-123!")
    {
        using var reg = CreateClient();

        await reg.PostAsJsonAsync("/account/register", new {email, password}); //login

        var login = await reg.PostAsJsonAsync("/account/login?useCookies=false&useSessionCookies=false", new {email, password});

        var body = await login.Content.ReadFromJsonAsync<JsonElement>();

        return body.GetProperty("accessToken").GetString()!;
    }
    

    public HttpClient CreateAuthenticatedClient(string token)
    {
        var client = CreateClient();

        client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

        return client;
    }


}