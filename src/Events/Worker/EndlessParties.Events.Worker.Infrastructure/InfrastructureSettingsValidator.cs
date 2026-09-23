using FluentValidation;

namespace EndlessParties.Events.Worker.Infrastructure;

/// <summary>
/// Валидатор <see cref="InfrastructureSettings"/>
/// </summary>
public class InfrastructureSettingsValidator : AbstractValidator<InfrastructureSettings>
{
    /// <inheritdoc />
    public InfrastructureSettingsValidator()
    {
        RuleFor(x => x.EventsDatabase)
            .NotEmpty();

        RuleFor(x => x.KafkaEventBus)
            .NotEmpty();
    }
}