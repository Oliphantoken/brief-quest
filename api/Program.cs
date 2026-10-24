using BriefQuest.Endpoints;
using BriefQuest.Options;
using BriefQuest.Services;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddOpenApi();

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

builder.Services.AddScoped<ExtractionService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};


app.MapGet("/", () => "Brief Quest API is running!");

/* Let IOptions retrieve the configured settings card (GeminiOptions) so its values can be checked
// Using IOptions to get the instance is called Dependency Injection, as we don't have to care about
// how that instance works, changes or the parameters it needs, just what it gives us access to. */
app.MapGet("/config-check", (IOptions<GeminiOptions> options) =>{
    var res = string.IsNullOrEmpty(options.Value.ApiKey) ? "Key missing" : "Key found";
    return $"Model: {options.Value.Model}, key: {res}";
    }
);

app.MapGet("/gemini-hello", async (GeminiClient client) =>
{
    return await client.GenerateAsync("say hello in five words.");
});

app.MapGet("/weatherforecast", () =>
{
    var forecast =  Enumerable.Range(1, 5).Select(index =>
        new WeatherForecast
        (
            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            Random.Shared.Next(-20, 55),
            summaries[Random.Shared.Next(summaries.Length)]
        ))
        .ToArray();
    return forecast;
})
.WithName("GetWeatherForecast");

app.MapBriefEndpoints();

app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}


//This partial class is needed for the WebApplicationFactory to find the entry point and perform an integration test
public partial class Program {}