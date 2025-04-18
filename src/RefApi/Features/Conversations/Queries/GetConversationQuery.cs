using MediatR;

using Microsoft.EntityFrameworkCore;

using RefApi.Data;
using RefApi.Security;

namespace RefApi.Features.Conversations.Queries;

public record GetConversationQuery(Guid Id) : IRequest<List<ConversationMessageDto>?>;

public class GetConversationQueryHandler(AppDbContext dbContext, IUserContext userContext)
    : IRequestHandler<GetConversationQuery, List<ConversationMessageDto>?>
{
    public async Task<List<ConversationMessageDto>?> Handle(
        GetConversationQuery request,
        CancellationToken cancellationToken)
    {
        var conversation = await dbContext.Conversations
            .Include(c => c.Messages)
            .FirstOrDefaultAsync(c => c.Id == request.Id && c.UserId == userContext.UserId, cancellationToken);

        return conversation?.Messages.ToConversationMessages(conversation.Id);
    }
}