using FluentValidation;

using RefApi.Features.Conversations.Commands;

namespace RefApi.Features.Conversations.Validations;

public class SaveConversationCommandValidator : AbstractValidator<SaveConversationCommand>
{
    public SaveConversationCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty();

        RuleFor(x => x.Messages)
            .NotNull()
            .Must(messages => messages.Count != 0)
            .WithMessage("At least one message is required");

        RuleForEach(x => x.Messages)
            .ChildRules(message =>
            {
                message.RuleFor(m => m.User)
                    .NotEmpty()
                    .MaximumLength(100);

                message.RuleFor(m => m.Response)
                    .NotNull();
            });
    }
}