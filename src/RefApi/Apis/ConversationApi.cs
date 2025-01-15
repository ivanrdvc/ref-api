using Asp.Versioning.Builder;

using MediatR;

using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

using RefApi.Extensions;
using RefApi.Features.Conversations;
using RefApi.Features.Conversations.Commands;
using RefApi.Features.Conversations.Queries;
using RefApi.Security;

namespace RefApi.Apis;

public static class ConversationApi
{
    public static IEndpointRouteBuilder MapConversationApiV1(this IEndpointRouteBuilder app, ApiVersionSet versionSet)
    {
        var api = app.MapGroup("api/v{version:apiVersion}/conversations")
            .WithTags("Conversations")
            .WithApiVersionSet(versionSet)
            .WithOpenApi()
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
            .WithGetDefaultResponses<List<ConversationMessage>?>()
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
        IMediator mediator,
        CancellationToken cancellationToken)
    {
        var query = new GetConversationsQuery(count, continuationToken);
        var result = await mediator.Send(query, cancellationToken);
        return TypedResults.Ok(result);
    }

    private static async Task<Results<Ok<List<ConversationMessage>>, NotFound>> GetConversation(
        Guid id,
        IMediator mediator,
        CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new GetConversationQuery(id), cancellationToken);
        return result is null ? TypedResults.NotFound() : TypedResults.Ok(result);
    }

    private static async Task<Results<Ok, ValidationProblem>> SaveConversation(
        [FromBody] SaveConversationCommand request,
        IMediator mediator,
        CancellationToken cancellationToken)
    {
        await mediator.Send(request, cancellationToken);
        return TypedResults.Ok();
    }

    private static async Task<Results<Ok, NotFound>> DeleteConversation(
        Guid id,
        IMediator mediator,
        CancellationToken cancellationToken)
    {
        var wasDeleted = await mediator.Send(new DeleteConversationCommand(id), cancellationToken);
        return wasDeleted ? TypedResults.Ok() : TypedResults.NotFound();
    }
}