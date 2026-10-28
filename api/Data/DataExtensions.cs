namespace BriefQuest.Data;

using BriefQuest.Options;
using BriefQuest.Services;
using Microsoft.Extensions.Options;

public static class DataExtensions
{
    public static void AddBriefQuestDB(this WebApplicationBuilder builder)
    {
        var connectionString = builder.Configuration.GetConnectionString("BriefQuest");
        builder.Services.AddNpgsql<BriefQuestDbContext>(connectionString);
        
    }

    public static void AddAiClientAndConfig(this WebApplicationBuilder builder)
    {
        /* The Configure<T> method creates and stores a GeminiOptions "settings" card that combines information
        // from appsettings.json (Model) and user-settings or environment(ApiKey). This info is then retrievable
        // from anywhere (through DI!) with IOptions<T>.Value. Note: This card isn't created until someone first
        // asks for it, then it can be reused.*/
        var geminiSection = builder.Configuration.GetSection(GeminiOptions.SectionName);
        builder.Services.Configure<GeminiOptions>(geminiSection);

        //Register the GeminiClient (the phone operator)
        builder.Services.AddHttpClient<GeminiClient>( (sp, client) =>
        {
            //The staff register (sp) is getting the "settings card" and sets up the number and the PIN.
            var gemini = sp.GetRequiredService<IOptions<GeminiOptions>>().Value;
            client.BaseAddress = new Uri("https://generativelanguage.googleapis.com/");
            client.DefaultRequestHeaders.Add("x-goog-api-key", gemini.ApiKey);    
        });
    }

}
