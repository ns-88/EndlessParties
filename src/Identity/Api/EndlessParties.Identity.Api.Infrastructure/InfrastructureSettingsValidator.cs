using EndlessParties.Shared.Utils.Database.Settings;
using FluentValidation;

namespace EndlessParties.Identity.Api.Infrastructure;

/// <summary>
/// Валидатор <see cref="InfrastructureSettings"/>
/// </summary>
public class InfrastructureSettingsValidator : AbstractValidator<InfrastructureSettings>
{
    /// <summary>
    /// Конструктор
    /// </summary>
    public InfrastructureSettingsValidator()
    {
        RuleFor(x => x.IdentityDatabase)
            .NotEmpty()
            .SetValidator(new DatabaseSettingsValidator());
    }
}