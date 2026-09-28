using FluentValidation;

namespace EndlessParties.Shared.Utils.Database.Settings;

/// <summary>
/// Валидатор <see cref="DatabaseSettings"/>
/// </summary>
internal class DatabaseSettingsValidator : AbstractValidator<DatabaseSettings>
{
    /// <summary>
    /// Конструктор
    /// </summary>
    public DatabaseSettingsValidator()
    {
        RuleFor(x => x.ConnectionString)
            .NotEmpty();

        RuleFor(x => x.RetryReconnectDatabaseCount)
            .GreaterThanOrEqualTo(0);
    }
}