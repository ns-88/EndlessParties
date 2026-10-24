using FluentValidation;

namespace EndlessParties.Shared.Utils.WebApiExtensions;

/// <summary>
/// Валидатор <see cref="ObservabilitySettings"/>
/// </summary>
internal class ObservabilitySettingsValidator : AbstractValidator<ObservabilitySettings>
{
    /// <inheritdoc />
    public ObservabilitySettingsValidator()
    {
        RuleFor(x => x.OltpEndpoint)
            .NotEmpty();

        RuleFor(x => x.ServiceName)
            .NotEmpty();
    }
}