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
        db.briefs.ExecuteDelete();
        return host;
    }
}