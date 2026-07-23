using EndlessParties.Application.Abstractions.Events.Models.Responses;
using FluentValidation;

namespace EndlessParties.Application.Events.Validations;

/// <summary>
/// Валидатор <see cref="GetAllEventsQueryFilter"/>
/// </summary>
internal class GetAllEventsFilterValidator : AbstractValidator<GetAllEventsQueryFilter>
{
    /// <inheritdoc />
    public GetAllEventsFilterValidator()
    {
        RuleFor(x => x.Page)
            .GreaterThan(0)
            .When(x => x.Page != null);

        RuleFor(x => x.PageSize)
            .GreaterThan(0)
            .LessThanOrEqualTo(GetAllEventsQueryFilter.MaxPageSize)
            .When(x => x.PageSize != null);
    }
}