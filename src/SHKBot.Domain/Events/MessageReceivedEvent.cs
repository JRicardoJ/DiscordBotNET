using Discord.WebSocket;
using MediatR;

namespace SHKBot.Domain.Events;

public record MessageReceivedEvent(
    string UserId,
    string ChannelId,
    string Content,
    SocketMessage SocketMessage
) : INotification;