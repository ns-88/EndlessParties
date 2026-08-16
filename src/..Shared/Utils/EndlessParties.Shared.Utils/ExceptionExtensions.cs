namespace EndlessParties.Shared.Utils.Exceptions;

/// <summary>
/// Набор методов-расширений для класса <see cref="Exception"/>
/// </summary>
public static class ExceptionExtensions
{
    /// <summary>
    /// Проверяет, является ли исключение результатом штатной отмены со стороны пользователя или системы
    /// </summary>
    public static bool IsCancelled(this Exception exception, CancellationToken cancellationToken)
    {
        if (exception is not OperationCanceledException)
        {
            return false;
        }

        return cancellationToken.IsCancellationRequested;
    }
}