using System.Text;
using System.Text.Json;

using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

using RefApi.Common;
using RefApi.Extensions;
using RefApi.Features.Chat.Commands;
using RefApi.Features.Chat.Models;
using RefApi.Security;

namespace RefApi.Features.Chat;

public static class ChatApi
{
    public static IEndpointRouteBuilder MapChatApi(this IEndpointRouteBuilder app)
    {
        var vApi = app.NewVersionedApi("chat");
        var api = vApi.MapGroup("api/v{version:apiVersion}/chat")
            .HasApiVersion(1, 0)
            .WithTags("Chat")
            .RequireAuthorization(AuthorizationPolicies.RequireContributor);

        api.MapPost("/", SendChat)
            .WithName("SendChat")
            .WithSummary("Processes a chat conversation with AI.")
            .WithDescription("Handles a chat request and returns the AI response.")
            .WithDefaultResponses()
            .Produces<ChatResponse>()
            .ProducesProblem(StatusCodes.Status400BadRequest);

        api.MapPost("/stream", StreamChat)
            .WithName("StreamChat")
            .WithSummary("Streams a chat conversation with AI.")
            .WithDescription("Processes a chat request and streams the AI response in real-time.")
            .WithDefaultResponses()
            .Produces(StatusCodes.Status200OK, contentType: "application/x-ndjson")
            .ProducesProblem(StatusCodes.Status400BadRequest);

        return app;
    }

    private static async Task<Results<Ok<ChatResponse>, ProblemHttpResult>> SendChat(
        [FromBody] ChatRequest request,
        IRequestHandler<ChatCommand, ChatResponse> handler,
        CancellationToken cancellationToken)
    {
        var command = new ChatCommand(
            request.Messages,
            request.Context,
            request.SessionState);

        var response = await handler.HandleAsync(command, cancellationToken);

        return TypedResults.Ok(response);
    }

    private static IResult StreamChat(
        [FromBody] ChatRequest request,
        IStreamRequestHandler<StreamChatCommand, ChatResponse> handler,
        CancellationToken cancellationToken)
    {
        var command = new StreamChatCommand(
            request.Messages,
            request.Context,
            request.SessionState);

        return Results.Stream(async stream =>
        {
            await foreach (var response in handler.HandleStreamAsync(command, cancellationToken))
            {
                var json = JsonSerializer.Serialize(response);
                var bytes = Encoding.UTF8.GetBytes($"{json}\n");
                await stream.WriteAsync(bytes, cancellationToken);
                await stream.FlushAsync(cancellationToken);
            }
        }, "application/x-ndjson");
    }
}