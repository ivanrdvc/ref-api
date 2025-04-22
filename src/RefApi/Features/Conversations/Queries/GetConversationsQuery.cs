using Microsoft.EntityFrameworkCore;

using RefApi.Common;
using RefApi.Data;
using RefApi.Security;

namespace RefApi.Features.Conversations.Queries;

public record GetConversationsQuery(int Count, string? ContinuationToken = null);

public class GetConversationsQueryHandler(AppDbContext dbContext, IUserContext userContext)
    : IRequestHandler<GetConversationsQuery, GetConversationsResponse>
{
    public async Task<GetConversationsResponse> HandleAsync(
        GetConversationsQuery request,
        CancellationToken cancellationToken)
    {
        if (userContext.UserId is null)
        {
            return new GetConversationsResponse([], null);
        }

        var offset = string.IsNullOrEmpty(request.ContinuationToken)
            ? 0
            : int.Parse(request.ContinuationToken);

        var items = await dbContext.Conversations
            .AsNoTracking()
            .Where(c => c.UserId == userContext.UserId)
            .OrderByDescending(c => c.Timestamp)
            .Skip(offset)
            .Take(request.Count + 1)
            .Select(c => new ConversationDto(c.Id, c.Title, c.Timestamp))
            .ToListAsync(cancellationToken);

        var hasMore = items.Count > request.Count;
        if (hasMore)
        {
            items.RemoveAt(items.Count - 1);
        }

        return new GetConversationsResponse(
            items,
            hasMore ? (offset + request.Count).ToString() : null
        );
    }
}

public record GetConversationsResponse(
    IEnumerable<ConversationDto> Items,
    string? ContinuationToken
);