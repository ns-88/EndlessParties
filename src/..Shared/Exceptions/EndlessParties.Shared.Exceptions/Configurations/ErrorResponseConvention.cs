using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationModels;

namespace EndlessParties.Shared.Exceptions.Configurations;

/// <summary>
/// Конфигуратор настроек <see cref="ApplicationModel"/>
/// </summary>
internal class ErrorResponseConvention : IApplicationModelConvention
{
    /// <inheritdoc />
    public void Apply(ApplicationModel application)
    {
        foreach (var controller in application.Controllers)
        {
            AddResponseType<ProblemDetails>(controller, StatusCodes.Status400BadRequest);
            AddResponseType<ProblemDetails>(controller, StatusCodes.Status500InternalServerError);
        }
    }

    /// <summary>
    /// Добавление типа ответа с указанием кода ответа HTTP
    /// </summary>
    private static void AddResponseType<T>(ControllerModel controllerModel, int statusCode)
    {
        if (controllerModel.Filters.Any(x => x is ProducesResponseTypeAttribute attr && attr.StatusCode == statusCode))
        {
            return;
        }

        controllerModel.Filters.Add(new ProducesResponseTypeAttribute(typeof(T), statusCode));
    }
}