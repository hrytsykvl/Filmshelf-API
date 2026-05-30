using System.Text;
using System.Text.Json;
using Azure;
using Azure.AI.OpenAI;
using FilmShelf.BAL.Interfaces;
using FilmShelf.BAL.Options;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using OpenAI.Chat;

namespace FilmShelf.BAL.Services;

public class LlamaApiService : ILlamaApiService
{
    private readonly HttpClient _httpClient;
    private readonly LlamaSettings _llamaSettings;
    private readonly ChatClient? _chatClient;
    private readonly ILogger<LlamaApiService> _logger;

    public LlamaApiService(
        HttpClient httpClient,
        IOptions<LlamaSettings> llamaSettings,
        IOptions<AzureOpenAiSettings> azureSettings,
        ILogger<LlamaApiService> logger
    )
    {
        _httpClient = httpClient;
        _llamaSettings = llamaSettings.Value;
        _logger = logger;

        if (_llamaSettings.UseAzureOpenAi)
        {
            var s = azureSettings.Value;
            var client = new AzureOpenAIClient(new Uri(s.Endpoint), new AzureKeyCredential(s.ApiKey));
            _chatClient = client.GetChatClient(s.ChatDeploymentName);
        }
    }

    public async Task<string> SendMessageAsync(string systemPrompt, string userMessage)
    {
        if (_llamaSettings.UseAzureOpenAi)
            return await SendViaAzureOpenAiAsync(systemPrompt, userMessage);

        return await SendViaOllamaAsync(systemPrompt, userMessage);
    }

    private async Task<string> SendViaAzureOpenAiAsync(string systemPrompt, string userMessage)
    {
        var options = new ChatCompletionOptions
        {
            Temperature = 0.3f,
            MaxOutputTokenCount = 4096,
            ResponseFormat = ChatResponseFormat.CreateJsonObjectFormat()
        };

        _logger.LogInformation("Sending request to Azure OpenAI chat deployment");

        var response = await _chatClient!.CompleteChatAsync(
            [
                new SystemChatMessage(systemPrompt),
                new UserChatMessage(userMessage)
            ],
            options
        );

        return response.Value.Content[0].Text;
    }

    private async Task<string> SendViaOllamaAsync(string systemPrompt, string userMessage)
    {
        var requestBody = new
        {
            model = _llamaSettings.Model,
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

        var url = $"{_llamaSettings.BaseUrl.TrimEnd('/')}/api/chat";

        using var request = new HttpRequestMessage(HttpMethod.Post, url);
        request.Content = new StringContent(
            JsonSerializer.Serialize(requestBody),
            Encoding.UTF8,
            "application/json"
        );

        _logger.LogInformation("Sending request to Ollama with model {Model}", _llamaSettings.Model);

        var response = await _httpClient.SendAsync(request);
        response.EnsureSuccessStatusCode();

        var responseBody = await response.Content.ReadAsStringAsync();

        using var doc = JsonDocument.Parse(responseBody);
        return doc.RootElement.GetProperty("message").GetProperty("content").GetString()!;
    }
}
