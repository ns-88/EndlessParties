using EndlessParties.Shared.Utils.Database.Settings;
using FluentValidation;

namespace EndlessParties.Events.Worker.Infrastructure;

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
        RuleFor(x => x.EventsDatabase)
            .NotEmpty()
            .SetValidator(new DatabaseSettingsValidator());

        RuleFor(x => x.KafkaEventBus)
            .NotEmpty();
    }
}