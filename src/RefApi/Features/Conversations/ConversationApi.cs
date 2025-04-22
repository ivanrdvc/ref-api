using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

using RefApi.Common;
using RefApi.Extensions;
using RefApi.Features.Conversations.Commands;
using RefApi.Features.Conversations.Queries;
using RefApi.Security;

namespace RefApi.Features.Conversations;

public static class ConversationApi
{
    public static IEndpointRouteBuilder MapConversationApi(this IEndpointRouteBuilder app)
    {
        var vApi = app.NewVersionedApi("conversations");
        var api = vApi.MapGroup("api/v{version:apiVersion}/conversations")
            .HasApiVersion(1, 0)
            .WithTags("Conversations")
            .RequireAuthorization(AuthorizationPolicies.RequireContributor);

        api.MapGet("/", GetConversations)
            .WithName("GetConversations")
            .WithSummary("Retrieves paginated conversation history.")
            .WithDescription("Returns a list of conversation metadata with pagination support via continuation token.")
            .WithGetDefaultResponses<GetConversationsResponse>()
            .ProducesProblem(StatusCodes.Status400BadRequest);

        api.MapGet("/{id:guid}", GetConversation)
            .WithName("GetConversation")
            .WithSummary("Retrieves a specific conversation.")
            .WithDescription("Returns the full conversation including all messages and responses.")
            .WithGetDefaultResponses<List<ConversationMessageDto>?>()
            .ProducesProblem(StatusCodes.Status400BadRequest);

        api.MapPost("/", SaveConversation)
            .WithName("SaveConversation")
            .WithSummary("Saves a new conversation.")
            .WithDescription("Stores a new conversation with its messages and responses.")
            .Produces(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest);

        api.MapDelete("/{id:guid}", DeleteConversation)
            .WithName("DeleteConversation")
            .WithSummary("Deletes a specific conversation.")
            .WithDescription("Permanently removes a conversation and all its associated messages.")
            .WithDeleteDefaultResponses()
            .ProducesProblem(StatusCodes.Status404NotFound);

        return app;
    }

    private static async Task<Results<Ok<GetConversationsResponse>, ValidationProblem>> GetConversations(
        [FromQuery] int count,
        [FromQuery] string? continuationToken,
        IRequestHandler<GetConversationsQuery, GetConversationsResponse> handler,
        CancellationToken cancellationToken)
    {
        var query = new GetConversationsQuery(count, continuationToken);
        var result = await handler.HandleAsync(query, cancellationToken);
        return TypedResults.Ok(result);
    }

    private static async Task<Results<Ok<List<ConversationMessageDto>>, NotFound>> GetConversation(
        Guid id,
        IRequestHandler<GetConversationQuery, List<ConversationMessageDto>?> handler,
        CancellationToken cancellationToken)
    {
        var result = await handler.HandleAsync(new GetConversationQuery(id), cancellationToken);
        return result is null ? TypedResults.NotFound() : TypedResults.Ok(result);
    }

    private static async Task<Results<Ok, ValidationProblem>> SaveConversation(
        [FromBody] SaveConversationCommand request,
        IRequestHandler<SaveConversationCommand, bool> handler,
        CancellationToken cancellationToken)
    {
        await handler.HandleAsync(request, cancellationToken);
        return TypedResults.Ok();
    }

    private static async Task<Results<Ok, NotFound>> DeleteConversation(
        Guid id,
        IRequestHandler<DeleteConversationCommand, bool> handler,
        CancellationToken cancellationToken)
    {
        var wasDeleted = await handler.HandleAsync(new DeleteConversationCommand(id), cancellationToken);
        return wasDeleted ? TypedResults.Ok() : TypedResults.NotFound();
    }
}