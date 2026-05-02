using EndlessParties.Application.Abstractions.Models.Requests;
using EndlessParties.Domain.Models;
using FluentValidation;

namespace EndlessParties.Application.Validations;

/// <summary>
/// Валидатор <see cref="EventRequestModel"/>
/// </summary>
internal class EventRequestModelValidator : AbstractValidator<EventRequestModel>
{
    /// <inheritdoc />
    public EventRequestModelValidator()
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