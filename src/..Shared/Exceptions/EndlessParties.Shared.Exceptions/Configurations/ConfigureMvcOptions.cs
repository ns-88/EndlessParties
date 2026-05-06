using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.Extensions.Options;

namespace EndlessParties.Shared.Exceptions.Configurations;

/// <summary>
/// Конфигуратор настроек <see cref="MvcOptions"/> для добавления реализации <see cref="IApplicationModelConvention"/>
/// </summary>
internal class ConfigureMvcOptions : IConfigureOptions<MvcOptions>
{
    /// <inheritdoc />
    public void Configure(MvcOptions options)
    {
        options.Conventions.Add(new ErrorResponseConvention());
    }
}