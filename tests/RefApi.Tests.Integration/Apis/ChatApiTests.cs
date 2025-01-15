using System.Net;
using System.Net.Http.Json;
using System.Text.Json;

using FluentAssertions;

using Microsoft.AspNetCore.Mvc;

using RefApi.Features.Chat.Models;

namespace RefApi.Tests.Integration.Apis;

public class ChatApiTests : IntegrationTestBase
{
    [Fact]
    public async Task SendChat_ShouldReturnOkResponse()
    {
        // Arrange
        var request = new ChatRequest
        {
            Messages = [new ChatMessage { Content = "hello", Role = "user" }],
            Context = new ChatRequestContext { Overrides = new ChatRequestOverrides() }
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/v1/chat", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<ChatResponse>();
        result.Should().NotBeNull();
    }

    [Fact]
    public async Task SendChat_ShouldReturnBadRequest()
    {
        // Arrange
        var request = new ChatRequest
        {
            Messages = [],
            Context = new ChatRequestContext { Overrides = new ChatRequestOverrides() }
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/v1/chat", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var problemDetails = await response.Content.ReadFromJsonAsync<ProblemDetails>();
        problemDetails.Should().NotBeNull();
        problemDetails!.Status.Should().Be(400);
    }

    [Fact]
    public async Task StreamChat_ShouldStreamResponses()
    {
        // Arrange
        var request = new ChatRequest
        {
            Messages = [new ChatMessage { Content = "hello", Role = "user" }],
            Context = new ChatRequestContext { Overrides = new ChatRequestOverrides() }
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/v1/chat/stream", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        response.Content.Headers.ContentType?.MediaType.Should().Be("application/x-ndjson");

        // Read and verify stream content
        using var stream = await response.Content.ReadAsStreamAsync();
        using var reader = new StreamReader(stream);
        var firstLine = await reader.ReadLineAsync();
        firstLine.Should().NotBeNull();
        var firstResponse = JsonSerializer.Deserialize<ChatResponse>(firstLine!);
        firstResponse.Should().NotBeNull();
    }
}