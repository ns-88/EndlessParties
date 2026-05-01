using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace EndlessParties.Shared.Exceptions.Models;

/// <summary>
/// Исключение, возникающее при ошибках валидации
/// </summary>
public class ValidationException : Exception
{
    /// <summary>
    /// Словарь ошибок <see cref="ModelStateDictionary"/>
    /// </summary>
    public ModelStateDictionary Errors { get; }

    /// <inheritdoc />
    public ValidationException(ModelStateDictionary errors) : base("Ошибка валидации модели")
    {
        Errors = errors;
    }
}