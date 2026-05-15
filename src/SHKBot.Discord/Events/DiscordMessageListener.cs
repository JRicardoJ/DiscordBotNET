using Discord.WebSocket;
using MediatR;
using SHKBot.Domain.Events;

namespace SHKBot.Discord.Events;

public class DiscordMessageListener
{
    private readonly DiscordSocketClient _client;
    private readonly IMediator _mediator;

    public DiscordMessageListener(DiscordSocketClient client, IMediator mediator)
    {
        _client = client;
        _mediator = mediator;
    }

    public Task InitializeAsync()
    {
        _client.MessageReceived += OnMessageReceived;
        return Task.CompletedTask;
    }

    private async Task OnMessageReceived(SocketMessage message)
    {
        // Ignore bots
        if (message.Author.IsBot) return;

        await _mediator.Publish(new MessageReceivedEvent(
            UserId: message.Author.Id.ToString(),
            ChannelId: message.Channel.Id.ToString(),
            Content: message.Content,
            SocketMessage: message
        ));
    }
}