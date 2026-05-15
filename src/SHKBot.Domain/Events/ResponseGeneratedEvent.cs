using Discord.WebSocket;
using MediatR;

namespace SHKBot.Domain.Events;

public record ResponseGeneratedEvent(
    string Response,
    SocketMessage OriginalMessage
) : INotification;