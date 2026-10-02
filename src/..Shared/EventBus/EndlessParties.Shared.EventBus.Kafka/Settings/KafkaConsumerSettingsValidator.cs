using FluentValidation;

namespace EndlessParties.Shared.EventBus.Kafka.Settings;

/// <summary>
/// Валидатор <see cref="KafkaConsumerSettings"/>
/// </summary>
internal class KafkaConsumerSettingsValidator : AbstractValidator<KafkaConsumerSettings>
{
    /// <inheritdoc />
    public KafkaConsumerSettingsValidator()
    {
        RuleFor(x => x.TopicName)
            .NotEmpty();

        RuleFor(x => x.GroupId)
            .NotEmpty();

        RuleFor(x => x.EventType)
            .NotEmpty();

        RuleFor(x => x.ProxyType)
            .NotEmpty();

        RuleFor(x => x.TargetInterfaceType)
            .NotEmpty();

        RuleFor(x => x.TargetImplementationType)
            .NotEmpty();
    }
}