using Asp.Versioning;

using Microsoft.AspNetCore.Mvc;

using RefApi.Controllers.Common;
using RefApi.Features.Conversations;
using RefApi.Features.Conversations.Commands;
using RefApi.Features.Conversations.Queries;

namespace RefApi.Controllers.Controllers.v1;

[Route(ApiConstants.StandardRoute)]
[ApiVersion(ApiVersions.V1)]
public class ConversationController() : ApiControllerBase
{
    /// <summary>
    /// Retrieves paginated conversation history.
    /// </summary>
    /// <param name="count">Number of conversations to retrieve (1-100).</param>
    /// <param name="continuationToken">Token for retrieving the next page.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Returns paginated list of conversations.</returns>
    /// <remarks>
    /// Sample request:
    ///     GET /api/v1/conversation?count=10&amp;continuationToken=eyJwYWdlIjoy
    /// 
    /// </remarks>
    /// <response code="200">Successfully retrieved conversations.</response>
    /// <response code="400">Invalid count parameter or continuation token.</response>
    [HttpGet]
    [ProducesResponseType(typeof(GetConversationsResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<GetConversationsResponse>> GetConversations(
        [FromQuery] int count,
        [FromQuery] string? continuationToken,
        CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Retrieves a specific conversation by ID.
    /// </summary>
    /// <param name="id">The conversation ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Returns the full conversation including messages.</returns>
    /// <remarks>
    /// Sample request:
    ///
    ///     GET /api/v1/conversation/3fa85f64-5717-4562-b3fc-2c963f66afa6
    ///
    /// </remarks>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(List<ConversationMessageDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<List<ConversationMessageDto>>> GetConversation(
        Guid id,
        CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Saves a new conversation or updates an existing one.
    /// </summary>
    /// <param name="request">The conversation data containing ID and messages.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Returns 200 OK on success.</returns>
    /// <remarks>
    /// Sample request:
    ///
    ///     POST /api/v1/conversation
    ///     {
    ///         "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    ///         "messages": [
    ///             {
    ///                 "user": "Hello.",
    ///                 "response": {
    ///                     "content": "Hello! How can I help you today?",
    ///                     "role": "assistant"
    ///                 }
    ///             }
    ///         ]
    ///     }
    /// </remarks>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> SaveConversation(
        [FromBody] SaveConversationCommand request,
        CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Deletes a specific conversation.
    /// </summary>
    /// <param name="id">The conversation ID (GUID).</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Returns 200 OK if deleted, 404 if not found.</returns>
    /// <remarks>
    /// Sample request:
    ///
    ///     DELETE /api/v1/conversation/3fa85f64-5717-4562-b3fc-2c963f66afa6
    /// </remarks>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteConversation(
        Guid id,
        CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}