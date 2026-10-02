using FluentValidation;

namespace EndlessParties.Shared.EventBus.Kafka.Settings;

/// <summary>
/// Валидатор <see cref="KafkaSettings"/>
/// </summary>
internal class KafkaSettingsValidator : AbstractValidator<KafkaSettings>
{
    /// <inheritdoc />
    public KafkaSettingsValidator()
    {
        RuleFor(x => x)
            .Must(x => x.Consumers.Count != 0 || x.Producers.Count != 0);

        RuleForEach(x => x.Producers.Values)
            .SetValidator(new KafkaProducerSettingsValidator());

        RuleForEach(x => x.Consumers.Values)
            .SetValidator(new KafkaConsumerSettingsValidator());
    }
}