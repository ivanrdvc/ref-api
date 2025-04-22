using Microsoft.EntityFrameworkCore;

using RefApi.Common;
using RefApi.Data;
using RefApi.Features.Chat;
using RefApi.Security;

namespace RefApi.Features.Conversations.Commands;

public record SaveConversationCommand(Guid Id, List<ConversationMessageDto> Messages);

public class SaveConversationCommandHandler(AppDbContext dbContext, IUserContext userContext)
    : IRequestHandler<SaveConversationCommand, bool>
{
    public async Task<bool> HandleAsync(
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

    private static string GenerateTitle(IEnumerable<ConversationMessageDto> messages)
    {
        var firstMessage = messages.FirstOrDefault();
        return firstMessage?.User.Length > 50
            ? $"{firstMessage.User[..50]}..."
            : firstMessage?.User ?? "Untitled";
    }

    private static IEnumerable<Message> MapToMessages(ConversationMessageDto conversationMessageDto)
    {
        yield return new Message { Role = MessageRole.User, Content = conversationMessageDto.User };

        yield return new Message
        {
            Role = MessageRole.Assistant, Content = conversationMessageDto.Response.Message?.Content ?? string.Empty
        };
    }
}