using FluentValidation;

using RefApi.Features.Conversations.Queries;

namespace RefApi.Features.Conversations.Validations;

public class GetConversationsQueryValidator : AbstractValidator<GetConversationsQuery>
{
    public GetConversationsQueryValidator()
    {
        RuleFor(x => x.Count)
            .GreaterThan(0)
            .LessThanOrEqualTo(100)
            .WithMessage("Count must be between 1 and 100");

        RuleFor(x => x.ContinuationToken)
            .Must(token => string.IsNullOrEmpty(token) || int.TryParse(token, out _))
            .WithMessage("Continuation token must be a valid integer or empty");
    }
}