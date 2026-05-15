using Discord;


namespace SHKBot.Application.Interfaces;

public interface IConversationRepository
{
    Task<IEnumerable<IMessage>> GetHistoryAsync(string userId);
    Task SaveMessageAsync(string userId, string userMessage, string botResponse);
}