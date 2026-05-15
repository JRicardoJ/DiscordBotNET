using System.Text;
using Discord;
using OllamaSharp;
using OllamaSharp.Models.Chat;
using SHKBot.Application.Interfaces;
using Microsoft.Extensions.Configuration;

namespace SHKBot.Infrastructure.LLM;

public class OllamaService : ILlmService
{
    private readonly OllamaApiClient _client;
    private readonly string _model;
    private const string SystemPrompt = """
        You are SHKBot, a helpful and friendly assistant living in a Discord server.
        Keep responses concise and conversational.
        """;

    public OllamaService(IConfiguration configuration)
    {
        var baseUrl = configuration["Ollama:BaseUrl"] ?? "http://localhost:11434";
        _model = configuration["Ollama:Model"] ?? "llama3";
        _client = new OllamaApiClient(new Uri(baseUrl));
    }
    public async Task<string> ChatAsync(
        string userMessage,
        IEnumerable<IMessage> history,
        CancellationToken cancellationToken)
    {
        var messages = BuildMessages(userMessage, history);

        var sb = new StringBuilder();

        await foreach (var chunk in _client.ChatAsync(
            new ChatRequest
            {
                Model = _model,
                Messages = messages
            },
            cancellationToken: cancellationToken))
        {
            sb.Append(chunk.Message?.Content);
        }

        return sb.Length > 0
            ? sb.ToString()
            : "Sorry, I couldn't generate a response.";
    }

    private static List<Message> BuildMessages(string userMessage, IEnumerable<IMessage> history)
    {
        var messages = new List<Message>
        {
            // System prompt always first
            new Message { Role = ChatRole.System, Content = SystemPrompt }
        };

        // Inject conversation history
        foreach (var msg in history)
        {
            messages.Add(new Message { Role = ChatRole.User, Content = msg.Content });
            messages.Add(new Message { Role = ChatRole.Assistant, Content = msg.Content });
        }

        // Add the current message last
        messages.Add(new Message { Role = ChatRole.User, Content = userMessage });

        return messages;
    }


}