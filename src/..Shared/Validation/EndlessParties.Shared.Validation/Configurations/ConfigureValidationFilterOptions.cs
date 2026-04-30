using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace EndlessParties.Shared.Validation.Configurations;

/// <summary>
/// Конфигуратор настроек <see cref="MvcOptions"/> для добавления <see cref="ValidationFilter"/>
/// </summary>
internal class ConfigureValidationFilterOptions : IConfigureOptions<MvcOptions>
{
    /// <inheritdoc />
    public void Configure(MvcOptions options)
    {
        options.Filters.Add<ValidationFilter>();
    }
}