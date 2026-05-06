using EndlessParties.Shared.Exceptions.Models;
using EndlessParties.Shared.Validations.Dispatcher;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace EndlessParties.Shared.Validations;

/// <summary>
/// Фильтр асинхронного действия для вызова валидации модели запроса
/// </summary>
internal class ValidationFilter : IAsyncActionFilter
{
    /// <summary>
    /// Диспетчер <see cref="ValidationDispatcher"/>
    /// </summary>
    private readonly ValidationDispatcher _dispatcher;

    /// <summary>
    /// Конструктор
    /// </summary>
    public ValidationFilter(ValidationDispatcher dispatcher)
    {
        _dispatcher = dispatcher;
    }

    /// <inheritdoc />
    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        if (!context.ModelState.IsValid)
        {
            throw new ValidationException(context.ModelState);
        }

        var serviceProvider = context.HttpContext.RequestServices;
        
        foreach (var argument in context.ActionArguments.Values)
        {
            if (argument == null)
            {
                continue;
            }

            var result = await _dispatcher.ValidateAsync(argument, serviceProvider);

            if (result.IsValid)
            {
                continue;
            }

            var errors = new ModelStateDictionary();

            foreach (var error in result.Errors)
            {
                errors.SetModelValue(error.PropertyName, error.AttemptedValue, error.AttemptedValue?.ToString());
                errors.AddModelError(error.PropertyName, error.ErrorMessage);
            }

            throw new ValidationException(errors);
        }

        await next();
    }
}