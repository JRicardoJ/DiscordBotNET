using MediatR;
using SHKBot.Application.Interfaces;
using SHKBot.Domain.Events;

namespace SHKBot.Application.EventHandlers;

public class MessageReceivedHandler : INotificationHandler<MessageReceivedEvent>
{
    private readonly ILlmService _llmService;
    private readonly IConversationRepository _conversationRepository;
    private readonly IMediator _mediator;

    public MessageReceivedHandler(
        ILlmService llmService,
        IConversationRepository conversationRepository,
        IMediator mediator)
    {
        _llmService = llmService;
        _conversationRepository = conversationRepository;
        _mediator = mediator;
    }

    public async Task Handle(MessageReceivedEvent notification, CancellationToken cancellationToken)
    {
        // Get conversation history for this user
        var history = await _conversationRepository.GetHistoryAsync(notification.UserId);

        // Call Ollama
        var response = await _llmService.ChatAsync(notification.Content, history, cancellationToken);

        // Save updated history
        await _conversationRepository.SaveMessageAsync(notification.UserId, notification.Content, response);

        // Publish response event for Discord layer to handle
        await _mediator.Publish(new ResponseGeneratedEvent(
            Response: response,
            OriginalMessage: notification.SocketMessage
        ), cancellationToken);
    }
}