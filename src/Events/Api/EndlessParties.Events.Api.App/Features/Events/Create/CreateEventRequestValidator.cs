using EndlessParties.Events.Api.App.Features.Events.Shared;
using FluentValidation;

namespace EndlessParties.Events.Api.App.Features.Events.Create;

/// <summary>
/// Валидатор <see cref="CreateEventRequest"/>
/// </summary>
internal class CreateEventRequestValidator : AbstractValidator<CreateEventRequest>
{
    /// <inheritdoc />
    public CreateEventRequestValidator()
    {
        Include(new EventDataValidator());
    }
}