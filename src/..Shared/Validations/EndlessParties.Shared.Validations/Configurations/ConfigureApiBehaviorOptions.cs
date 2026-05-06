using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace EndlessParties.Shared.Validations.Configurations;

/// <summary>
/// Конфигуратор настроек <see cref="ApiBehaviorOptions"/> для отключения встроенной валидации
/// </summary>
internal class ConfigureApiBehaviorOptions : IConfigureOptions<ApiBehaviorOptions>
{
    /// <inheritdoc />
    public void Configure(ApiBehaviorOptions options)
    {
        options.SuppressModelStateInvalidFilter = true;
    }
}