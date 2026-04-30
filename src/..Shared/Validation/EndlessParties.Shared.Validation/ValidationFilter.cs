using EndlessParties.Shared.Validation.Dispatcher;
using FluentValidation;
using Microsoft.AspNetCore.Mvc.Filters;

namespace EndlessParties.Shared.Validation;

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
            throw new ValidationException("Ошибка валидации");
        }
        
        var serviceProvider = context.HttpContext.RequestServices;

        foreach (var argument in context.ActionArguments.Values)
        {
            if (argument == null)
            {
                continue;
            }

            var result = await _dispatcher.ValidateAsync(argument, serviceProvider);

            if (!result.IsValid)
            {
                throw new ValidationException("Ошибка валидации", result.Errors);
            }
        }

        await next();
    }
}