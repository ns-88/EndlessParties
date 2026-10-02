using FluentValidation;

namespace EndlessParties.Shared.EventBus.Kafka.Settings;

/// <summary>
/// Валидатор <see cref="KafkaProducerSettings"/>
/// </summary>
internal class KafkaProducerSettingsValidator : AbstractValidator<KafkaProducerSettings>
{
    /// <inheritdoc />
    public KafkaProducerSettingsValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty();

        RuleFor(x => x.TopicName)
            .NotEmpty();
    }
}