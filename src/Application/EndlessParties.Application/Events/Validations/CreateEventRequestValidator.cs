using EndlessParties.Application.Abstractions.Events.Models.Requests;
using EndlessParties.Domain.Models;
using FluentValidation;

namespace EndlessParties.Application.Events.Validations;

/// <summary>
/// Валидатор <see cref="CreateEventRequest"/>
/// </summary>
internal class CreateEventRequestValidator : AbstractValidator<CreateEventRequest>
{
    /// <inheritdoc />
    public CreateEventRequestValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty()
            .MaximumLength(Event.MaxTitleLength)
            .When(x => !string.IsNullOrWhiteSpace(x.Description), ApplyConditionTo.CurrentValidator);

        RuleFor(x => x.Description)
            .MaximumLength(Event.MaxDescriptionLength)
            .When(x => !string.IsNullOrWhiteSpace(x.Description));

        RuleFor(x => x.StartAt)
            .NotEmpty()
            .LessThan(x => x.EndAt);

        RuleFor(x => x.EndAt)
            .NotEmpty();
    }
}