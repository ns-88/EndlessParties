using FluentValidation;

namespace EndlessParties.Identity.Cryptography.Settings;

/// <summary>
/// Валидатор <see cref="JwtTokenSettings"/>
/// </summary>
internal class JwtTokenSettingsValidator : AbstractValidator<JwtTokenSettings>
{
    /// <inheritdoc />
    public JwtTokenSettingsValidator()
    {
        RuleLevelCascadeMode = CascadeMode.Stop;

        RuleFor(x => x.SecretKey)
            .NotEmpty();

        RuleFor(x => x.Issuer)
            .NotEmpty();

        RuleFor(x => x.Audiences)
            .NotEmpty()
            .ForEach(x => x.NotEmpty())
            .Must(x => x.Distinct().Count() == ((IReadOnlyList<string>)x).Count);

        RuleFor(x => x.Expires)
            .NotEmpty();
    }
}