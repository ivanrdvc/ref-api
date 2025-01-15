using MediatR;

using Microsoft.EntityFrameworkCore;

using RefApi.Common.Mapping;
using RefApi.Data;
using RefApi.Security;

namespace RefApi.Features.Conversations.Queries;

public record GetConversationQuery(Guid Id) : IRequest<List<ConversationMessage>?>;

public class GetConversationQueryHandler(AppDbContext dbContext, IUserContext userContext)
    : IRequestHandler<GetConversationQuery, List<ConversationMessage>?>
{
    public async Task<List<ConversationMessage>?> Handle(
        GetConversationQuery request,
        CancellationToken cancellationToken)
    {
        var conversation = await dbContext.Conversations
            .Include(c => c.Messages)
            .FirstOrDefaultAsync(c => c.Id == request.Id && c.UserId == userContext.UserId, cancellationToken);

        return conversation is null
            ? null
            : MessageMapper.ToConversationMessages(conversation.Messages, conversation.Id);
    }
}