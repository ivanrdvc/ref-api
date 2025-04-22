using Microsoft.EntityFrameworkCore;

using RefApi.Common;
using RefApi.Data;
using RefApi.Security;

namespace RefApi.Features.Conversations.Commands;

public record DeleteConversationCommand(Guid Id);

public class DeleteConversationCommandHandler(AppDbContext dbContext, IUserContext userContext)
    : IRequestHandler<DeleteConversationCommand, bool>
{
    public async Task<bool> HandleAsync(DeleteConversationCommand request, CancellationToken cancellationToken)
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
            return false;
        }

        dbContext.Conversations.Remove(conversation);
        await dbContext.SaveChangesAsync(cancellationToken);

        return true;
    }
}