using System.Security.Claims;
using BriefQuest.Data;
using BriefQuest.Endpoints;
using BriefQuest.Entities;
using BriefQuest.Options;
using BriefQuest.Services;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);

// Add OpenApi services to the container.
builder.Services.AddOpenApi();
//Register AI configuration options and add AI client with the said options injected
builder.AddAiClientAndConfig();
//Add extraction service
builder.Services.AddScoped<ExtractionService>();
//Add database
builder.AddBriefQuestDB();
//Add authorisation and authentication/identity endpoints and stores
builder.Services.AddAuthorization();
builder.Services.AddIdentityApiEndpoints<AppUser>()
    .AddEntityFrameworkStores<BriefQuestDbContext>();

//-------------------------------------------------------------------------
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment()) { app.MapOpenApi(); }

app.UseHttpsRedirection();

app.UseAuthentication(); //ID check: who are you?
app.UseAuthorization(); //the guest list: are you allowed in?
app.MapGroup("/account").MapIdentityApi<AppUser>();

app.MapGet("/whoami", (ClaimsPrincipal user) => user.Identity?.Name)
    .RequireAuthorization();


app.MapGet("/", () => "Brief Quest API is running!");

/* Let IOptions retrieve the configured settings card (GeminiOptions) so its values can be checked
// Using IOptions to get the instance is called Dependency Injection, as we don't have to care about
// how that instance works, changes or the parameters it needs, just what it gives us access to. */
app.MapGet("/config-check", (IOptions<GeminiOptions> options) =>{
    var res = string.IsNullOrEmpty(options.Value.ApiKey) ? "Key missing" : "Key found";
    return $"Model: {options.Value.Model}, key: {res}";
    }
);

// app.MapGet("/gemini-hello", async (GeminiClient client) =>
// { return await client.GenerateAsync("say hello in five words."); });


app.MapBriefEndpoints();
//-------------------------------------------------------------------------

app.Run();



//This partial class is needed for the WebApplicationFactory to find the entry point and perform an integration test
public partial class Program {}