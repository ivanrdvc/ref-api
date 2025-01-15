using Microsoft.SemanticKernel.ChatCompletion;

namespace RefApi.Services;

/// <summary>
/// Service for handling chat-based interactions using a chat completion service.
/// </summary>
public class ChatService(
    IChatCompletionService chatCompletionService,
    ILogger<ChatService> logger) : IChatService
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
    public async Task<string> GetChatMessageContent(
        ChatHistory chatHistory,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(chatHistory);

        try
        {
            logger.LogInformation("Processing chat completion for chat history with {MessageCount} messages",
                chatHistory.Count);

            var response = await chatCompletionService.GetChatMessageContentsAsync(
                chatHistory: chatHistory,
                cancellationToken: cancellationToken);

            if (response.Any())
            {
                return response[0].ToString();
            }

            logger.LogWarning("Chat completion service returned empty response");
            return "No response was generated. Please try again.";
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unexpected error during chat completion");
            return "An unexpected error occurred. Please try again.";
        }
    }
}