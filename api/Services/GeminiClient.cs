using BriefQuest.Options;
using Microsoft.Extensions.Options;
namespace BriefQuest.Services;

public class GeminiClient
{
    private readonly HttpClient _httpClient;
    private string _geminiModel;
    private ILogger<GeminiClient> _logger;
    public GeminiClient(HttpClient httpClient, IOptions<GeminiOptions> options, ILogger<GeminiClient> logger) {
        _httpClient = httpClient;
        _geminiModel = options.Value.Model;
        _logger = logger;
    }
    
    public async Task<string> GenerateAsync(string prompt)
    {
        var request = new {
            contents = new[] {
                new {
                    parts = new[]{
                        new { text = prompt }
                    }
                } 
            }
        };

        return await SendAsync(request);
    }

    public async Task<string> SendAsync(object body)
    {
        const int maxCounter = 3;
        for(int attempt = 1; ; attempt++)
        {
            //The URL is relative here as the base URL is passed on from the Builder (Program.cs)
            using var response = await _httpClient.PostAsJsonAsync($"v1/models/{_geminiModel}:generateContent", body);
            var text = await response.Content.ReadAsStringAsync();
            if(response.IsSuccessStatusCode) return text;

            int status = (int)response.StatusCode;

            bool retryable = status == 429 || status >= 500;
           
            _logger.LogWarning("Gemini returned {Status} on attempt {Attempt}.", status, attempt);
           
            if (!retryable || attempt == maxCounter)
                throw new GeminiUnavailableException(status, text);

            var wait = response.Headers.RetryAfter?.Delta ?? (TimeSpan.FromSeconds(Math.Pow(2, attempt - 1))
                + TimeSpan.FromMilliseconds(Random.Shared.Next(0, 500)));
            
            await Task.Delay(wait);

        }
        
    }

}