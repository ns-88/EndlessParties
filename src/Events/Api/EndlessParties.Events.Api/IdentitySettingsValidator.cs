using FluentValidation;

namespace EndlessParties.Events.Api;

/// <summary>
/// Валидатор <see cref="IdentitySettings"/>
/// </summary>
public class IdentitySettingsValidator : AbstractValidator<IdentitySettings>
{
    /// <inheritdoc />
    public IdentitySettingsValidator()
    {
        RuleFor(x => x.SecretKey)
            .NotEmpty();

        RuleFor(x => x.Issuer)
            .NotEmpty();

        RuleFor(x => x.Audience)
            .NotEmpty();
    }
}