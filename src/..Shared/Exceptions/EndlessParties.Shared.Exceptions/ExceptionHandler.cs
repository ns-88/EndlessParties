using EndlessParties.Shared.Exceptions.Models;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;

namespace EndlessParties.Shared.Exceptions;

/// <summary>
/// Глобальный обработчик исключений приложения
/// </summary>
internal class ExceptionHandler : IExceptionHandler
{
    /// <summary>
    /// Ошибка выполнения приложения
    /// </summary>
    private const string ErrorTitle = "Ошибка выполнения приложения";

    /// <summary>
    /// Ошибка выполнения приложения
    /// </summary>
    private const string UnknownErrorTitle = "Неизвестная ошибка выполнения приложения";

    /// <summary>
    /// Ошибка валидации
    /// </summary>
    private const string ValidationDataErrorTitle = "Ошибка валидации данных";

    /// <summary>
    /// Ошибка отсутствия запрашиваемого объекта
    /// </summary>
    private const string NotFoundErrorTitle = "Запрашиваемый объект не найден";


    /// <summary>
    /// Фабрика <see cref="ProblemDetailsFactory"/>
    /// </summary>
    private readonly ProblemDetailsFactory _factory;


    /// <summary>
    /// Конструктор
    /// </summary>
    public ExceptionHandler(ProblemDetailsFactory factory)
    {
        _factory = factory;
    }


    /// <inheritdoc />
    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        var problemDetails = exception switch
        {
            LogicException logicException => HandleLogicException(logicException, httpContext),
            ValidationException validationException => HandleValidationException(validationException, httpContext),
            NotFoundException notFoundException => HandleNotFoundException(notFoundException, httpContext),
            _ => HandleUnknownException(exception, httpContext)
        };

        return await problemDetails.TryWriteToResponse(httpContext, cancellationToken);
    }

    /// <summary>
    /// Обработка исключения с типом <see cref="LogicException"/>
    /// </summary>
    private ProblemDetails HandleLogicException(LogicException exception, HttpContext httpContext)
    {
        return CreateProblemDetails(
            exception,
            ErrorTitle,
            StatusCodes.Status400BadRequest,
            httpContext);
    }

    /// <summary>
    /// Обработка исключения с типом <see cref="ValidationException"/>
    /// </summary>
    private ValidationProblemDetails HandleValidationException(ValidationException exception, HttpContext httpContext)
    {
        return _factory.CreateValidationProblemDetails(
            httpContext,
            exception.Errors,
            StatusCodes.Status422UnprocessableEntity,
            ValidationDataErrorTitle,
            instance: httpContext.Request.Path);
    }

    /// <summary>
    /// Обработка исключения с типом <see cref="NotFoundException"/>
    /// </summary>
    private ProblemDetails HandleNotFoundException(NotFoundException exception, HttpContext httpContext)
    {
        return _factory.CreateProblemDetails(
            httpContext,
            StatusCodes.Status404NotFound,
            NotFoundErrorTitle,
            detail: exception.Message,
            instance: httpContext.Request.Path);
    }

    /// <summary>
    /// Обработка неизвестного исключения
    /// </summary>
    private ProblemDetails HandleUnknownException(Exception exception, HttpContext httpContext)
    {
        return CreateProblemDetails(
            exception,
            UnknownErrorTitle,
            StatusCodes.Status500InternalServerError,
            httpContext);
    }

    /// <summary>
    /// Создание <see cref="ProblemDetails"/> с указанием общих полей
    /// </summary>
    private ProblemDetails CreateProblemDetails(Exception exception, string title, int statusCode, HttpContext httpContext)
    {
        var problemDetails = _factory.CreateProblemDetails(httpContext, statusCode, instance: httpContext.Request.Path);

        if (exception.InnerException != null)
        {
            problemDetails.Title = exception.Message;
            problemDetails.Detail = exception.InnerException.Message;
        }
        else
        {
            problemDetails.Title = title;
            problemDetails.Detail = exception.Message;
        }

        return problemDetails;
    }
}