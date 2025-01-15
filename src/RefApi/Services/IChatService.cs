using Microsoft.SemanticKernel.ChatCompletion;

namespace RefApi.Services;

/// <summary>
/// Defines the contract for a service that handles chat-based interactions.
/// </summary>
public interface IChatService
{
    /// <summary>
    /// Gets the content of a chat message based on the provided chat history.
    /// </summary>
    /// <param name="chatHistory">The history of chat messages to process.</param>
    /// <param name="cancellationToken">A token to cancel the operation if needed.</param>
    /// <returns>
    /// The generated chat message content. If an error occurs or no response is generated,
    /// returns an appropriate error message.
    /// </returns>
    /// <exception cref="ArgumentNullException">Thrown when chatHistory is null.</exception>
    Task<string> GetChatMessageContent(ChatHistory chatHistory, CancellationToken cancellationToken);
}