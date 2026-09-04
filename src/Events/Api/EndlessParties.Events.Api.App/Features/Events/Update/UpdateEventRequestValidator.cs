using EndlessParties.Events.Api.App.Features.Events.Shared;
using FluentValidation;

namespace EndlessParties.Events.Api.App.Features.Events.Update;

/// <summary>
/// Валидатор <see cref="UpdateEventRequest"/>
/// </summary>
internal class UpdateEventRequestValidator : AbstractValidator<UpdateEventRequest>
{
    /// <inheritdoc />
    public UpdateEventRequestValidator()
    {
        Include(new EventDataValidator());
    }
}