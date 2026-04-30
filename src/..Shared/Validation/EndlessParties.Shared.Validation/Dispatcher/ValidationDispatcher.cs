using System.Collections.Concurrent;

namespace EndlessParties.Shared.Validation.Dispatcher;

/// <summary>
/// Сервис для кэширования типизированных обработчиков и выполнения асинхронной валидации моделей
/// </summary>
internal class ValidationDispatcher
{
    /// <summary>
    /// Словарь <see cref="IValidationInvoker"/> для типов валидируемых моделей 
    /// </summary>
    private readonly ConcurrentDictionary<Type, IValidationInvoker> _invokers;

    /// <summary>
    /// Конструктор
    /// </summary>
    public ValidationDispatcher()
    {
        _invokers = new ConcurrentDictionary<Type, IValidationInvoker>();
    }

    /// <summary>
    /// Выполнение валидации
    /// </summary>
    public Task<ValidationResult> ValidateAsync(object model, IServiceProvider serviceProvider)
    {
        var modelType = model.GetType();

        var invoker = _invokers.GetOrAdd(modelType, type =>
        {
            var genericType = typeof(ValidationInvoker<>).MakeGenericType(type);

            return (IValidationInvoker)Activator.CreateInstance(genericType)!;
        });

        return invoker.ValidateAsync(model, serviceProvider);
    }
}