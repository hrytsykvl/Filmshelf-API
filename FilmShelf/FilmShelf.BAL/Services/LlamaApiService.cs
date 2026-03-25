using System.Text;
using System.Text.Json;
using FilmShelf.BAL.Interfaces;
using FilmShelf.BAL.Options;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace FilmShelf.BAL.Services;

public class LlamaApiService : ILlamaApiService
{
    private readonly HttpClient _httpClient;
    private readonly LlamaSettings _settings;
    private readonly ILogger<LlamaApiService> _logger;

    public LlamaApiService(
        HttpClient httpClient,
        IOptions<LlamaSettings> settings,
        ILogger<LlamaApiService> logger
    )
    {
        _httpClient = httpClient;
        _settings = settings.Value;
        _logger = logger;
    }

    public async Task<string> SendMessageAsync(string systemPrompt, string userMessage)
    {
        var requestBody = new
        {
            model = _settings.Model,
            messages = new[]
            {
                new { role = "system", content = systemPrompt },
                new { role = "user", content = userMessage },
            },
            stream = false,
            format = "json",
            options = new
            {
                temperature = 0.3,
                num_predict = 4096
            }
        };

        var url = $"{_settings.BaseUrl.TrimEnd('/')}/api/chat";

        using var request = new HttpRequestMessage(HttpMethod.Post, url);
        request.Content = new StringContent(
            JsonSerializer.Serialize(requestBody),
            Encoding.UTF8,
            "application/json"
        );

        _logger.LogInformation("Sending request to Ollama with model {Model}", _settings.Model);

        var response = await _httpClient.SendAsync(request);
        response.EnsureSuccessStatusCode();

        var responseBody = await response.Content.ReadAsStringAsync();

        using var doc = JsonDocument.Parse(responseBody);
        return doc.RootElement.GetProperty("message").GetProperty("content").GetString()!;
    }
}
