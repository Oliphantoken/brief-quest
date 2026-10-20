using System.Reflection;
using BriefQuest.Endpoints;
using BriefQuest.Properties;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

// The Configure<T> method creates and stores a GeminiOptions "settings" card that combines information
// from appsettings.json (Model) and user-settings or environment(ApiKey). This info is then retrievable
// from anywhere with IOptions<T>.Value
var apiKey = builder.Configuration.GetSection(GeminiOptions.SectionName);
builder.Services.Configure<GeminiOptions>(apiKey);

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
