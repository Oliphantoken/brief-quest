using System.Net.Http;
using System.Threading.Tasks;
namespace BriefQuest.Services;

public class GeminiService
{
    private readonly HttpClient _httpClient;
    private const string BaseUrl = "https://api.gemini.com/v1";
    public GeminiService(HttpClient httpClient) {
        _httpClient = httpClient;
    }
    public async Task GetTickerAsync(string symbol)
    {
        var response = await _httpClient.GetAsync($"{BaseUrl}/pubticker/{symbol}");
        response.EnsureSuccessStatusCode();
        var task = await response.Content.ReadAsStringAsync();
        //return task;
    }
}