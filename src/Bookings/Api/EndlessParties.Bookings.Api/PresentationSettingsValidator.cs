using FluentValidation;

namespace EndlessParties.Bookings.Api;

/// <summary>
/// Валидатор <see cref="PresentationSettings"/>
/// </summary>
public class PresentationSettingsValidator : AbstractValidator<PresentationSettings>
{
    /// <inheritdoc />
    public PresentationSettingsValidator()
    {
        RuleFor(x => x.Identity)
            .NotEmpty();
    }
}