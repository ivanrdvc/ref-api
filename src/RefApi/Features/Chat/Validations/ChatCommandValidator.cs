using FluentValidation;

using RefApi.Features.Chat.Commands;
using RefApi.Features.Chat.Models;

namespace RefApi.Features.Chat.Validations;

public class ChatCommandValidator : AbstractValidator<ChatCommand>
{
    public ChatCommandValidator()
    {
        RuleFor(x => x.Messages)
            .NotEmpty()
            .WithMessage("At least one message is required")
            .DependentRules(() =>
            {
                RuleForEach(x => x.Messages)
                    .SetValidator(new ChatMessageValidator());

                RuleFor(x => x.Messages)
                    .Must(messages => messages.Any(m =>
                        m.Role.Equals(MessageRole.User, StringComparison.OrdinalIgnoreCase)))
                    .WithMessage("At least one user message is required");
            });

        RuleFor(x => x.Context)
            .NotNull()
            .WithMessage("Context is required");

        When(x => x.SessionState != null, () =>
        {
            RuleFor(x => x.SessionState)
                .Must(BeValidSessionState)
                .WithMessage("Session state must be a valid GUID string");
        });
    }

    private static bool BeValidSessionState(string? sessionState)
    {
        return Guid.TryParse(sessionState, out _);
    }
}

public class ChatMessageValidator : AbstractValidator<ChatMessage>
{
    private static readonly HashSet<string> ValidRoles = new(StringComparer.OrdinalIgnoreCase)
    {
        MessageRole.System,
        MessageRole.User,
        MessageRole.Assistant,
        MessageRole.Tool
    };

    public ChatMessageValidator()
    {
        RuleFor(x => x.Content)
            .NotEmpty()
            .WithMessage("Message content cannot be empty");

        RuleFor(x => x.Role)
            .NotEmpty()
            .WithMessage("Message role is required")
            .Must(role => ValidRoles.Contains(role))
            .WithMessage($"Invalid role. Valid roles are: {string.Join(", ", ValidRoles)}");
    }
}