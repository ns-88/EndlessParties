using FluentValidation;

namespace EndlessParties.Events.Api;

/// <summary>
/// Валидатор <see cref="PresentationSettings"/>
/// </summary>
internal class PresentationSettingsValidator : AbstractValidator<PresentationSettings>
{
    /// <inheritdoc />
    public PresentationSettingsValidator()
    {
        RuleFor(x => x.Identity)
            .NotEmpty();
    }
}