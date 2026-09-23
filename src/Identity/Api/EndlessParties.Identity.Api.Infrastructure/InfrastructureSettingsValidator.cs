using FluentValidation;

namespace EndlessParties.Identity.Api.Infrastructure;

/// <summary>
/// Валидатор <see cref="InfrastructureSettings"/>
/// </summary>
public class InfrastructureSettingsValidator : AbstractValidator<InfrastructureSettings>
{
    /// <inheritdoc />
    public InfrastructureSettingsValidator()
    {
        RuleFor(x => x.IdentityDatabase)
            .NotEmpty();

        RuleFor(x => x.JwtToken)
            .NotEmpty();
    }
}