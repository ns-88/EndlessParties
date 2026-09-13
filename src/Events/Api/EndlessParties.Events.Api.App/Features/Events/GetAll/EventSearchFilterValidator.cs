using FluentValidation;

namespace EndlessParties.Events.Api.App.Features.Events.GetAll;

/// <summary>
/// Валидатор <see cref="EventSearchFilter"/>
/// </summary>
internal class EventSearchFilterValidator : AbstractValidator<EventSearchFilter>
{
    /// <inheritdoc />
    public EventSearchFilterValidator()
    {
        RuleFor(x => x.Page)
            .GreaterThan(0)
            .When(x => x.Page != null);

        RuleFor(x => x.PageSize)
            .GreaterThan(0)
            .LessThanOrEqualTo(EventSearchFilter.MaxPageSize)
            .When(x => x.PageSize != null);
    }
}