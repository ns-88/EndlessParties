using FluentValidation;

namespace EndlessParties.Shared.Cache.Redis.Settings;

/// <summary>
/// Валидатор <see cref="RedisClientSettings"/>
/// </summary>
internal class RedisClientSettingsValidator : AbstractValidator<RedisClientSettings>
{
    /// <inheritdoc />
    public RedisClientSettingsValidator()
    {
        RuleFor(x => x.Host)
            .NotEmpty();

        RuleFor(x => x.Port)
            .NotEmpty();

        RuleFor(x => x.ConnectTimeout)
            .NotEmpty();

        RuleFor(x => x.QueryTimeout)
            .NotEmpty();

        RuleFor(x => x.ClientName)
            .NotEmpty();
    }
}