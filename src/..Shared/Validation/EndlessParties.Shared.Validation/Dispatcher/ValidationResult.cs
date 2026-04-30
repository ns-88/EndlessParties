using FluentValidation.Results;

namespace EndlessParties.Shared.Validation.Dispatcher;

/// <summary>
/// Результат валидации
/// </summary>
internal readonly struct ValidationResult
{
    /// <summary>
    /// Пустой результат
    /// </summary>
    public static ValidationResult Empty = new(true, []);

    /// <summary>
    /// Признак успешности выполнения
    /// </summary>
    public bool IsValid { get; }

    /// <summary>
    /// Список ошибок
    /// </summary>
    public IReadOnlyList<ValidationFailure> Errors { get; }

    /// <summary>
    /// Конструктор
    /// </summary>
    public ValidationResult(bool isValid, IReadOnlyList<ValidationFailure> errors)
    {
        IsValid = isValid;
        Errors = errors;
    }
}