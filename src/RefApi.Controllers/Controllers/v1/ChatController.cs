using Asp.Versioning;

using Microsoft.AspNetCore.Mvc;

using RefApi.Controllers.Common;
using RefApi.Features.Chat.Models;

namespace RefApi.Controllers.Controllers.v1;

[Route(ApiConstants.StandardRoute)]
[ApiVersion(ApiVersions.V1)]
public class ChatController() : ApiControllerBase
{
    /// <summary>
    /// Sends a message within a conversation.
    /// </summary>
    /// <param name="conversationId">The ID of the conversation.</param>
    /// <param name="request">The request containing the chat messages and context.</param>
    /// <returns>The chat response.</returns>
    /// <remarks>
    /// Sample request:
    /// 
    ///     POST /api/v1/chat/{conversationId}
    ///     {
    ///         "messages": [
    ///             {
    ///                 "role": "user",
    ///                 "content": "Hello, how are you?"
    ///             }
    ///         ],
    ///         "context": {
    ///             "overrides": {
    ///                 "suggestFollowupQuestions": false
    ///             }
    ///         }
    ///     }
    /// 
    /// </remarks>
    /// <response code="200">Returns the chat response.</response>
    [HttpPost("{conversationId:guid}")]
    [ProducesResponseType(typeof(ChatResponse), StatusCodes.Status200OK)]
    public async Task<ActionResult<ChatResponse>> SendMessage(
        Guid conversationId,
        [FromBody] ChatRequest request)
    {
        throw new NotImplementedException();
        ;
    }
}