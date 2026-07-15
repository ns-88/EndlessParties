using EndlessParties.Application.Abstractions.Bookings.Models.Requests;
using FluentValidation;

namespace EndlessParties.Application.Bookings.Validations;

/// <summary>
/// Валидатор <see cref="CreateBookingRequest"/>
/// </summary>
internal class CreateBookingRequestValidator : AbstractValidator<CreateBookingRequest>
{
    /// <inheritdoc />
    public CreateBookingRequestValidator()
    {
        RuleFor(x => x.EventId)
            .NotEmpty();
    }
}