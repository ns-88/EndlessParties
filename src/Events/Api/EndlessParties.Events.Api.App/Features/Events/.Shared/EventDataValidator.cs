using EndlessParties.Events.Domain.Models;
using FluentValidation;

namespace EndlessParties.Events.Api.App.Features.Events.Shared;

/// <summary>
/// Валидатор <see cref="EventData"/>
/// </summary>
internal class EventDataValidator : AbstractValidator<EventData>
{
    /// <inheritdoc />
    public EventDataValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty()
            .MaximumLength(Event.MaxTitleLength)
            .When(x => !string.IsNullOrWhiteSpace(x.Title), ApplyConditionTo.CurrentValidator);

        RuleFor(x => x.TotalSeats)
            .GreaterThan(0);

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