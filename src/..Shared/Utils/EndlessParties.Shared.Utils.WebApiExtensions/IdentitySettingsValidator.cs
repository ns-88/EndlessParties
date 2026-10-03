using FluentValidation;

namespace EndlessParties.Shared.Utils.WebApiExtensions;

/// <summary>
/// Валидатор <see cref="IdentitySettings"/>
/// </summary>
internal class IdentitySettingsValidator : AbstractValidator<IdentitySettings>
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