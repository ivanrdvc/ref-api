using System.Text;
using System.Text.Json;

using Asp.Versioning.Builder;

using MediatR;

using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

using RefApi.Extensions;
using RefApi.Features.Chat.Commands;
using RefApi.Features.Chat.Models;
using RefApi.Security;

namespace RefApi.Apis;

public static class ChatApi
{
    public static IEndpointRouteBuilder MapChatApiV1(this IEndpointRouteBuilder app, ApiVersionSet versionSet)
    {
        var api = app.MapGroup("api/v{version:apiVersion}/chat")
            .WithTags("Chat")
            .WithApiVersionSet(versionSet)
            .WithOpenApi();

        api.MapPost("/", SendChat)
            .WithName("SendChat")
            .WithSummary("Processes a chat conversation with AI.")
            .WithDescription("Handles a chat request and returns the AI response.")
            .WithGetDefaultResponses<ChatResponse>()
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .RequireAuthorization(AuthorizationPolicies.RequireContributor);

        api.MapPost("/stream", StreamChat)
            .WithName("StreamChat")
            .WithSummary("Streams a chat conversation with AI.")
            .WithDescription("Processes a chat request and streams the AI response in real-time.")
            .Produces(StatusCodes.Status200OK, contentType: "application/x-ndjson")
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .RequireAuthorization(AuthorizationPolicies.RequireContributor);

        return app;
    }

    private static async Task<Results<Ok<ChatResponse>, ProblemHttpResult>> SendChat(
        [FromBody] ChatRequest request,
        IMediator mediator,
        CancellationToken cancellationToken)
    {
        var command = new ChatCommand(
            request.Messages,
            request.Context,
            request.SessionState);

        var response = await mediator.Send(command, cancellationToken);

        return TypedResults.Ok(response);
    }

    private static IResult StreamChat(
        [FromBody] ChatRequest request,
        IMediator mediator,
        CancellationToken cancellationToken)
    {
        var command = new StreamChatCommand(
            request.Messages,
            request.Context,
            request.SessionState);

        return Results.Stream(async stream =>
        {
            await foreach (var response in mediator.CreateStream(command, cancellationToken))
            {
                var json = JsonSerializer.Serialize(response);
                var bytes = Encoding.UTF8.GetBytes($"{json}\n");
                await stream.WriteAsync(bytes, cancellationToken);
                await stream.FlushAsync(cancellationToken);
            }
        }, "application/x-ndjson");
    }
}