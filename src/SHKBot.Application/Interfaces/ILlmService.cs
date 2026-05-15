

using Discord;

namespace SHKBot.Application.Interfaces;

public interface ILlmService
{
    Task<string> ChatAsync(string userMessage, IEnumerable<IMessage> history, CancellationToken cancellationToken);
}