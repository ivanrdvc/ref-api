using MediatR;

using Microsoft.EntityFrameworkCore;

using RefApi.Data;
using RefApi.Features.Chat;
using RefApi.Security;

namespace RefApi.Features.Conversations.Commands;

public record SaveConversationCommand(Guid Id, List<ConversationMessage> Messages) : IRequest<bool>;

public class SaveConversationCommandHandler(AppDbContext dbContext, IUserContext userContext)
    : IRequestHandler<SaveConversationCommand, bool>
{
    public async Task<bool> Handle(
        SaveConversationCommand request,
        CancellationToken cancellationToken)
    {
        if (userContext.UserId == null)
        {
            return false;
        }

        var conversation = await dbContext.Conversations
            .Include(c => c.Messages)
            .FirstOrDefaultAsync(c => c.Id == request.Id && c.UserId == userContext.UserId, cancellationToken);

        if (conversation is null)
        {
            conversation = new Conversation
            {
                Id = request.Id,
                UserId = userContext.UserId,
                Title = GenerateTitle(request.Messages),
                Timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(),
                Messages = request.Messages.SelectMany(MapToMessages).ToList()
            };
            dbContext.Conversations.Add(conversation);
        }
        else
        {
            var newMessages = request.Messages
                .Skip(conversation.Messages.Count / 2)
                .SelectMany(MapToMessages)
                .ToList();

            if (newMessages.Count != 0)
            {
                conversation.Messages.AddRange(newMessages);
            }
        }

        await dbContext.SaveChangesAsync(cancellationToken);
        return true;
    }

    private static string GenerateTitle(IEnumerable<ConversationMessage> messages)
    {
        var firstMessage = messages.FirstOrDefault();
        return firstMessage?.User.Length > 50
            ? $"{firstMessage.User[..50]}..."
            : firstMessage?.User ?? "Untitled";
    }

    private static IEnumerable<Message> MapToMessages(ConversationMessage conversationMessage)
    {
        yield return new Message { Role = MessageRole.User, Content = conversationMessage.User };

        yield return new Message
        {
            Role = MessageRole.Assistant, Content = conversationMessage.Response.Message?.Content ?? string.Empty
        };
    }
}