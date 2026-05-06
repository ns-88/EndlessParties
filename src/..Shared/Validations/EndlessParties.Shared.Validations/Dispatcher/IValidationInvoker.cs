namespace EndlessParties.Shared.Validations.Dispatcher;

/// <summary>
/// Обработчик для вызова метода валидации типизированного валидатора
/// </summary>
internal interface IValidationInvoker
{
    /// <summary>
    /// Выполнение валидации
    /// </summary>
    Task<ValidationResult> ValidateAsync(object model, IServiceProvider serviceProvider);
}