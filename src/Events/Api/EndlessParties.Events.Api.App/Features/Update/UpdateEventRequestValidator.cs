using EndlessParties.Events.Api.App.Features.Shared;
using FluentValidation;

namespace EndlessParties.Events.Api.App.Features.Update;

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