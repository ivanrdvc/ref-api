using FluentValidation;

using RefApi.Features.Conversations.Commands;

namespace RefApi.Features.Conversations.Validations;

public class DeleteConversationCommandValidator : AbstractValidator<DeleteConversationCommand>
{
    public DeleteConversationCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage("Conversation Id cannot be empty");
    }
}